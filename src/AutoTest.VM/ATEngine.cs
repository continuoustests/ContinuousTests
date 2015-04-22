using System;
using System.Collections.Generic;
using System.Linq;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Configuration;
using AutoTest.Core.TestRunners;
using AutoTest.VM.Messages.Communication;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using System.IO;
using AutoTest.Messages;
using AutoTest.Core.Messaging;
using AutoTest.VM.Messages;
using Castle.MicroKernel.Registration;
using AutoTest.Core.Caching.RunResultCache;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.DebugLog;
using AutoTest.VM.FileSystem;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Launchers;
using AutoTest.VM.RealtimeChanges;
using AutoTest.Core.Presenters;
using AutoTest.Messages.FileStorage;
using AutoTest.Minimizer;

namespace AutoTest.VM
{
    class ATEngine
    {
        private readonly object _manualMinimizationLock = new object();
        private readonly object _padlock = new object();
        private readonly TcpServer _server;
        private readonly ILocateWriteLocation _writeLocator;
        private readonly string _watchPath;
        private bool _isPaused;
        private string _currentBuildProvider = null;
        
        private string _localConfigLocation;
        private IMessageProxy _proxy;
        private IDirectoryWatcher _watcher;
        private IConfiguration _configuration;
        private readonly IValidateLicense _license;
        private ChangeTracker _realtimeChangeTracker;
        private string _configuredCustomOutput = "";
        private bool _resumeMinimizerOnNextRunCompleted;
        private bool _setRecursiveRunDetectorOnRunStarted;
        private bool _setRecursiveRunDetectorStarted;
        private bool _disableOnDemandRunnerOnNextRunCompleted;
        private bool _profilerCorrupted;
        private bool _gotProfilerCorrupted;
        public bool StartedPaused { get; private set; }
        public bool IsRunning { get; private set; }

        public ATEngine(TcpServer server, ILocateWriteLocation writeLocator, string watchPath, IValidateLicense license, string localConfigLocation)
        {
            _server = server;
            _writeLocator = writeLocator;
            _watchPath = watchPath;
            _license = license;
            StartedPaused = false;
            IsRunning = false;
            _isPaused = StartedPaused;
            _localConfigLocation = localConfigLocation;
        }

        public void Start()
        {
            lock (_padlock)
            {
                Logger.WriteDebug("Starting autotest.net engine");
                BootStrapper.SetBuildConfiguration(
                    new BuildConfiguration((original, @new) => {
                        var detector = new PublicContractChangeDetector();
                        var changes = detector.GetAllPublicContractChangesBetween(original, @new).ToArray();
                        var optimisticBuildPossible = changes.Length == 0;
                        if (!optimisticBuildPossible)
                        {
                            Debug.WriteDebug("Optimistic build changes");
                            foreach (var change in changes)
                                Debug.WriteDebug("\t" + change.ItemChanged);
                        }
                        return optimisticBuildPossible;
                    }));
                BootStrapper.Configure(_writeLocator);
                Logger.WriteDebug("Setting up log writer");
                Logger.SetWriter(BootStrapper.Services.Locate<IWriteDebugInfo>());
                BootStrapper.Container.Register(Component.For<IMessageProxy>()
                                                                .Forward<IConsumerOf<AssembliesMinimizedMessage>>()
                                                                .Forward<IConsumerOf<AbortMessage>>()
                                                                .ImplementedBy<AutoTestMessageProxy>().LifeStyle.Singleton);
                BootStrapper.Container.Register(Component.For<IPreProcessTestruns>().ImplementedBy<MinimizingPreProcessor>().LifeStyle.Singleton);
                BootStrapper.Container.Register(Component.For<IPreProcessBuildruns>().ImplementedBy<MinimizingBuildPreProcessor>());
                BootStrapper.Container.Register(Component.For<IPreProcessBuildruns>().Forward<IPreProcessTestruns>().ImplementedBy<OnDemanTestrunPreprocessor>().LifeStyle.Singleton);
                BootStrapper.Container.Register(Component.For<IPreProcessBuildruns>().ImplementedBy<RealtimeChangePreProcessor>().LifeStyle.Singleton);
                BootStrapper.Container.Register(Component.For<IConsumerOf<FileChangeMessage>>().ImplementedBy<RecursiveRunCauseConsumer>().Named("RecursiveRunConsumer"));
                BootStrapper.Container.Register(Component.For<ICustomIgnoreProvider>().ImplementedBy<IgnoreProvider>());
                Logger.WriteDebug("Setting up message proxy");
                _proxy = BootStrapper.Services.Locate<IMessageProxy>();
                _proxy.SetMessageForwarder(_server);
                _proxy.RunStarted += _proxy_RunStarted;
                _proxy.RunFinished += _proxy_RunFinished;
                _configuration = BootStrapper.Services.Locate<IConfiguration>();
                if (_configuration.DebuggingEnabled)
                    Logger.EnableWriter();
                Logger.WriteDebug("Checking license");
                if (licenseIsInvalid())
                    return;

                _realtimeChangeTracker = new ChangeTracker(getRealtimeRunPreprocessor(), _configuration, BootStrapper.Services.Locate<IMessageBus>(), BootStrapper.Services.Locate<IGenerateBuildList>());
                Logger.WriteDebug("Setting up cache");
                var runCache = BootStrapper.Services.Locate<IRunResultCache>();
                runCache.EnabledDeltas();
                BootStrapper.InitializeCache(_watchPath);
                _watcher = BootStrapper.Services.Locate<IDirectoryWatcher>();
                Logger.WriteDebug("Looking for config in "+_localConfigLocation);
                _watcher.LocalConfigurationIsLocatedAt(_localConfigLocation);
                _watcher.Watch(_watchPath);
                _configuration.ValidateSettings();
                _configuredCustomOutput = _configuration.CustomOutputPath;
                var disableAll = _configuration.AllSettings("mm-AllDisabled").ToLower().Equals("true");
                StartedPaused = _configuration.StartPaused || disableAll;
                _isPaused = StartedPaused;
                setCustomOutputPath();
                var minimizer = getMinimizer();
                minimizer.ProfilerCompletedUpdate += minimizer_ProfilerCompletedUpdate;
                minimizer.ProfilerInitialized += minimizer_ProfilerInitialized;
                minimizer.MinimizerInitialized += minimizer_MinimizerInitialized;
                minimizer.ProfilingStarted += (sender, e) => _server.Send(new ProfiledTestRunStarted());
                minimizer.ProfilerLoadError += profiler_profilercorrupted;
                minimizer.SetManualUpdateProvider(() => _isPaused);
                initializeAllForPreProcessor(minimizer);
                if (disableAll)
                    Pause();
                IsRunning = true;
            }
        }

        private void profiler_profilercorrupted(object sender, EventArgs e)
        {
            Logger.WriteDebug("PROFILER IS CORRUPTED.");
            _profilerCorrupted = true;
            if(_gotProfilerCorrupted)
                _server.Send(new ProfilerLoadErrorOccurredMessage());
        }

        void minimizer_MinimizerInitialized(object sender, EventArgs e)
        {
            _server.Send(new MinimizerInitializedMessage());
        }

        private void minimizer_ProfilerInitialized(object sender, EventArgs e)
        {
            Logger.WriteDebug("Profiler is initialized");
            _profilerCorrupted = false;
            _server.Send(new ProfilerInitializedMessage());
        }

        private void minimizer_ProfilerCompletedUpdate(object sender, EventArgs e)
        {
            _profilerCorrupted = false;
            _server.Send(new ProfilerCompletedMessage());
        }

        public bool IsSolutionInitialized()
        {
            if (Environment.OSVersion.Platform != PlatformID.MacOSX && Environment.OSVersion.Platform != PlatformID.Unix && _configuration.AllSettings("mm-ProfilerSetup") != "DONTRUN")
            {
                var minimizer = getMinimizer();
                if (!File.Exists(minimizer.GetDBName()))
                    return false;
                var dbInfo = new FileInfo(minimizer.GetDBName());
                if (dbInfo.Length == 0 && DateTime.Now.Subtract(dbInfo.CreationTime).TotalMinutes <= 2)
                    return false;
            }
            var hasValidMMFile = false;
            var cache = BootStrapper.Services.Locate<ICache>();
            var projects = cache.GetAll<Project>();
            foreach (var project in projects)
            {
                Logger.WriteDebug("Preparing existence check for project: " + project.Key);
                var assembly = project.GetAssembly(_configuration.CustomOutputPath);
                if (assembly == "")
                {
                    Logger.WriteDebug("Could not build assembly path");
                    continue;
                }
                var extension = Path.GetExtension(assembly);
                try
                {
                    var mm = Path.Combine(Path.GetDirectoryName(assembly), Path.GetFileNameWithoutExtension(assembly) + ".mm" + extension);
                    mm = new PathTranslator(_configuration.WatchToken).Translate(mm);
                    Logger.WriteDebug("Checking for the existence of " + mm);
                    if (File.Exists(mm))
                    {
                        hasValidMMFile = true;
                        continue;
                    }
                    if (!File.Exists(assembly))
                    {
                        if (project.Value != null)
                            project.Value.RebuildOnNextRun();
                        continue;
                    }
                    File.Copy(assembly, mm);
                    if (project.Value != null)
                        project.Value.RebuildOnNextRun();
                }
                catch (Exception ex)
                {
                    Logger.WriteError("Failed to check for " + assembly);
                    Logger.WriteError(ex.ToString());
                }
            }
            return hasValidMMFile;
        }

        private void setCustomOutputPath()
        {
            if (_isPaused)
                SetCustomOutputPath(Path.Combine("bin", "Debug"));
            else
                SetCustomOutputPath(_configuredCustomOutput);
        }

        public void SetCustomOutputPath(string newPath)
        {
            if (_configuration.CustomOutputPath.Equals(newPath))
                return;

            Debug.WriteDebug("Setting custom output folder to " + newPath);
            _configuration.SetCustomOutputPath(newPath);
            var minimizer = getMinimizer();
            minimizer.Initialize();
        }

        private bool licenseIsInvalid()
        {
            var isValid = _license.IsValid && passesTestQuestion();
            if (!isValid)
                _server.Send(new InvalidLicenseMessage());
            return !isValid;
        }

        private bool passesTestQuestion()
        {
            return _license.Register("Lay it on me bro", null)
                .Equals("Honestly if you are so bad off that you have to hack this product to get it why didn't you just get in touch? " +
                        "We would probably have sponsored you with a license until you're back on track. " +
                        "Have some guts and be honest, don't just go off and steal everything you want");
        }

        void _proxy_RunStarted(object sender, EventArgs e)
        {
            getMinimizer().RunStarted();
            if (_setRecursiveRunDetectorOnRunStarted)
            {
                var bus = BootStrapper.Services.Locate<IMessageBus>();
                _currentBuildProvider = bus.BuildProvider;
                bus.SetBuildProvider("RecursiveRunConsumer");
                _setRecursiveRunDetectorOnRunStarted = false;
                _setRecursiveRunDetectorStarted = true;
            }
        }

        void _proxy_RunFinished(object sender, EventArgs e)
        {
            _realtimeChangeTracker.Consume();
            if (_resumeMinimizerOnNextRunCompleted)
                resumeMinimizer();
            if (_disableOnDemandRunnerOnNextRunCompleted)
                disableOnDemandRunner();
            if (_setRecursiveRunDetectorStarted)
            {
                Logger.WriteDebug("Recursive run detection completed");
                _setRecursiveRunDetectorStarted = false;
                var files = ((RecursiveRunCauseConsumer)BootStrapper.Services.Locate<IConsumerOf<FileChangeMessage>>("RecursiveRunConsumer")).Files;
                Logger.WriteDebug("Recursive run detection found changes in " + files.Length.ToString() + " files");
                _server.Send(new RecursiveRunResultMessage(files));
                if (_currentBuildProvider == null)
                {
                    Stop();
                    Start();
                }
                else
                    BootStrapper.Services.Locate<IMessageBus>().SetBuildProvider(_currentBuildProvider);
                _currentBuildProvider = null;
            }
        }

        public VisualGraphGeneratedMessage GetCouplingGraph(string symbolName)
        {
            Logger.WriteDebug("Getting graph for " + symbolName);
            var minimizer = getMinimizer();
            return minimizer.GetVisualizationGraphFor(symbolName);
        }

        public VisualGraphGeneratedMessage GetProfiledGraph(string symbolName)
        {
            Logger.WriteDebug("Getting profiled graph for " + symbolName);
            var minimizer = getMinimizer();
            return minimizer.GetProfiledGraphFor(symbolName);
        }

        public VisualGraphGeneratedMessage GetLastRunCouplingGraph()
        {
            Logger.WriteDebug("Getting last affected graph");
            var minimizer = getMinimizer();
            return minimizer.GetLastAffectedGraph();
        }

        public ProfilerLoadErrorOccurredMessage TestProfilerIsCorrupted()
        {
            Logger.WriteDebug("Testing profiler is corrupted: " + _profilerCorrupted);
            if (_profilerCorrupted)
            {
                _gotProfilerCorrupted = true;
                Logger.WriteDebug("returning corrupted message");
                return new ProfilerLoadErrorOccurredMessage();
            }
            return null;
        }

        public TestInformationGeneratedMessage GetRuntimeTestInformation(string testName)
        {
            Logger.WriteDebug("Getting runtime information for " + testName);
            var minimizer = getMinimizer();
            return minimizer.GetTestInformationFor(testName);
        }

        public RiskMetricGeneratedMessage GetRiskMetricFor(string symbolName)
        {
            Logger.WriteDebug("Getting Risk Metric for " + symbolName);
            var minimizer = getMinimizer();
            return minimizer.GetRiskMetricsFor(symbolName);
        }

        public void RunRelatedTests(string symbolName)
        {
            var configuration = BootStrapper.Services.Locate<IConfiguration>();
            var projects = BootStrapper.Services.Locate<ICache>().GetAll<Project>();

            Logger.WriteDebug("From at");
            foreach (var project in projects)
                Logger.WriteDebug(project.GetAssembly(configuration.CustomOutputPath));

            var minimizer = getMinimizer();
            var graph = minimizer.GetVisualizationGraphFor(symbolName);
            var runs = graph.Nodes.GroupBy(x => x.Assembly).Select(x =>
                new OnDemandRun(projects.Where(p => p.GetAssembly(configuration.CustomOutputPath).Equals(x.Key)).Select(p => p.Key).FirstOrDefault(),
                    x.Where(t => t.IsTest).Select(t => string.Format("{0}.{1}", t.Type, t.Name)).ToArray(),
                    new string[] { },
                    new string[] { }));

            Logger.WriteDebug("From minimizer");
            foreach (var node in graph.Nodes.GroupBy(x => x.Assembly).Select(x => x.Key))
                Logger.WriteDebug(node);

            StartOnDemandTestRun(runs);
        }

        public void Stop()
        {
            if (licenseIsInvalid())
                return;

            lock (_padlock)
            {
                Logger.WriteDebug("Shutting down");
                BootStrapper.ShutDown();
            }
        }

        public void Pause()
        {
            if (licenseIsInvalid())
                return;

            lock (_padlock)
            {
                var cacheHandler = BootStrapper.Services.Locate<IMergeRunResults>();
                cacheHandler.Clear();
                BootStrapper.PauseFilWatcher();
            }
            _isPaused = true;
            setCustomOutputPath();
        }

        public void Resume()
        {
            if (licenseIsInvalid())
                return;

            lock (_padlock)
            {
                BootStrapper.ResumeFileWatcher();
            }
            _isPaused = false;
            setCustomOutputPath();
        }

        public void DoFullRun()
        {
            if (licenseIsInvalid())
                return;

            var message = new ProjectChangeMessage();
            var cache = BootStrapper.Services.Locate<ICache>();
            var configuration = BootStrapper.Services.Locate<IConfiguration>();
            var bus = BootStrapper.Services.Locate<IMessageBus>();
            var projects = cache.GetAll<Project>();
            foreach (var project in projects)
            {
                if (project.Value == null)
                {
                    Logger.WriteDebug("Invalid project does not contain.Value " + project.Key);
                    continue;
                }
                project.Value.RebuildOnNextRun();
                message.AddFile(new ChangedFile(project.Key));
            }
            _resumeMinimizerOnNextRunCompleted = true;
            pauseMinimizer();
            notifyMinimizerAboutFullRun();
            bus.Publish(message);
        }

        public void DoPartialRun(IEnumerable<string> projects)
        {
            var message = new ProjectChangeMessage();
            projects.ToList().ForEach(x => message.AddFile(new ChangedFile(x)));
            var bus = BootStrapper.Services.Locate<IMessageBus>();
            bus.Publish(message);
        }

        public void SetupRecursiveCauseDetectorAsNextTrackerType()
        {
            if (licenseIsInvalid())
                return;

            _setRecursiveRunDetectorOnRunStarted = true;
        }

        private void pauseMinimizer()
        {
            var minimizer = getMinimizer();
            if (minimizer == null)
                return;
            minimizer.PauseMinimizer();
        }

        private void notifyMinimizerAboutFullRun()
        {
            var minimizer = getMinimizer();
            if (minimizer == null)
                return;
            minimizer.PrepareForFullRun();
        }

        private void resumeMinimizer()
        {
            var minimizer = getMinimizer();
            if (minimizer == null)
                return;
            minimizer.ResumeMinimizer();
        }

        private static MinimizingPreProcessor getMinimizer()
        {
            var preProcessors = BootStrapper.Services.LocateAll<IPreProcessTestruns>();
            foreach (var preProcessor in preProcessors)
            {
                if (preProcessor.GetType() == typeof(MinimizingPreProcessor))
                    return (MinimizingPreProcessor)preProcessor;
            }
            return null;
        }

        public string GetNUnitTestRunner()
        {
            return _configuration.NunitTestRunner("");
        }

        public string GetMSTestRunner()
        {
            return _configuration.MSTestRunner("");
        }

        public bool IsLoggingEnabled()
        {
            return _configuration.DebuggingEnabled;
        }

        private static void initializeAllForPreProcessor(MinimizingPreProcessor preProcessor)
        {
            preProcessor.Initialize();
        }

        public void PerformManualMinimization()
        {
            // Only perform manual minimization when engine is running and is in paused state
            if (!IsRunning)
                return;
            if (!_isPaused)
                return;
            lock (_manualMinimizationLock)
            {
                Logger.WriteDebug("Performing manual minimization");
                var cache = BootStrapper.Container.Resolve<ICache>();
                var minimizer = getMinimizer();
                var infos = cache.GetAll<Project>().Select(x => getRunInfo(x)).ToArray();
                minimizer.PreProcess(new PreProcessedTesRuns(null, infos));
            }
        }

        private RunInfo getRunInfo(Project x)
        {
            var info = new RunInfo(x);
            info.SetAssembly(x.GetAssembly(_configuration.CustomOutputPath));
            return info;
        }

        public void RefreshConfiguration()
        {
            if (licenseIsInvalid())
                return;

            var config = BootStrapper.Services.Locate<IConfiguration>();
            string file = new ConfigurationLocator().GetConfiguration(_localConfigLocation);
            Logger.WriteDetails("Reloading configuration with local config " + file);
            config.Reload(file);
            ValidateConfiguration();
        }

        public void ValidateConfiguration()
        {
            if (licenseIsInvalid())
                return;

            _configuration.ValidateSettings();
        }

        public string GetAssemblyFromProject(string projectPath)
        {
            if (licenseIsInvalid())
                return null;

            var cache = BootStrapper.Services.Locate<ICache>();
            var config = BootStrapper.Services.Locate<IConfiguration>();
            var project = cache.Get<Project>(projectPath);
            if (project == null)
                return null;
            return project.GetAssembly(config.CustomOutputPath);
        }

        public void StartOnDemandTestRun(IEnumerable<OnDemandRun> runs)
        {
            if (licenseIsInvalid())
                return;

            lock (_manualMinimizationLock)
            {
                var message = new ProjectChangeMessage();
                var cache = BootStrapper.Services.Locate<ICache>();
                var bus = BootStrapper.Services.Locate<IMessageBus>();
                var projects = cache.GetAll<Project>();
                Logger.WriteDebug(string.Format("Recieved {0} runs", runs.Count()));
                addProjects(runs, message, projects);
                var onDemandPreProcessor = getOnDemandPreProcessor();
                foreach (var run in runs)
                {
                    Logger.WriteDebug("Adding test run to preprocessor " + run.Project);
                    onDemandPreProcessor.AddRuns(run);
                }
                onDemandPreProcessor.Activate();
                _disableOnDemandRunnerOnNextRunCompleted = true;
                _resumeMinimizerOnNextRunCompleted = true;
                pauseMinimizer();
                bus.Publish(message);
            }
        }

        public void GoTo(GoToFileAndLineMessage location)
        {
            if (licenseIsInvalid())
                return;

            BootStrapper.Services.Locate<IApplicatonLauncher>().LaunchEditor(location.File, location.Line, location.Column);
        }

        public void Focus()
        {
            if (licenseIsInvalid())
                return;

            BootStrapper.Services.Locate<IApplicatonLauncher>().FocusEditor();
        }

        public void QueueRealtimeRun(RealtimeChangeList msg)
        {
            _realtimeChangeTracker.Enqueue(msg);
        }

        public IEnumerable<string> BuildOrderedProjectList(OrderedBuildList msg)
        {
            return BootStrapper.Services.Locate<IGenerateOrderedBuildLists>().Generate(msg.Projects);
        }

        public void AbortRun()
        {
            BootStrapper.Services.Locate<IMessageBus>().Publish(new AbortMessage(""));
        }

        private RealtimeChangePreProcessor getRealtimeRunPreprocessor()
        {
            var preProcessors = BootStrapper.Services.LocateAll<IPreProcessBuildruns>();
            foreach (var preProcessor in preProcessors)
            {
                if (preProcessor.GetType() == typeof(RealtimeChangePreProcessor))
                    return (RealtimeChangePreProcessor)preProcessor;
            }
            return null;
        }

        private OnDemanTestrunPreprocessor getOnDemandPreProcessor()
        {
            var preProcessors = BootStrapper.Services.LocateAll<IPreProcessBuildruns>();
            foreach (var preProcessor in preProcessors)
            {
                if (preProcessor.GetType() == typeof(OnDemanTestrunPreprocessor))
                    return (OnDemanTestrunPreprocessor)preProcessor;
            }
            return null;
        }

        private void addProjects(IEnumerable<OnDemandRun> runs, ProjectChangeMessage message, Project[] projects)
        {
            foreach (var run in runs)
            {
                var project = projects.FirstOrDefault(x => x.Key.Equals(run.Project));
                if (project == null)
                {
                    Logger.WriteError(string.Format("Did not find matching project for run {0}", run.Project));
                    continue;
                }
                message.AddFile(new ChangedFile(run.Project));
            }
        }

        private void disableOnDemandRunner()
        {
            var preProcessor = getOnDemandPreProcessor();
            preProcessor.Deactivate();
        }
    }
}
