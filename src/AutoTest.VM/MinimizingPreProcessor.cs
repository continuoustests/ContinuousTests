//using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.TestRunners;
using AutoTest.Graphs;
using AutoTest.Messages;
using AutoTest.Core.Caching;
using AutoTest.Core.Configuration;
using AutoTest.Core.Caching.Projects;
using System.IO;
using AutoTest.Core.Caching.RunResultCache;
using AutoTest.Core.Messaging;
using AutoTest.Minimizer;
using AutoTest.Minimizer.RiskClassifiers;
using AutoTest.Profiler;
using AutoTest.VM.Messages;
using System.Diagnostics;
using AutoTest.TestRunners.Shared.Targeting;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Messages.FileStorage;
using AutoTest.Core.BuildRunners;
using Castle.Core;
using Mono.Cecil;

namespace AutoTest.VM
{
    class MinimizingPreProcessor : IPreProcessTestruns
    {
        private readonly ProfilerData _profilerData;
        private TestMinimizer _minimizer;
        private bool _minimizerPaused;
        private readonly ICache _cache;
        private readonly IMessageBus _bus;
        private readonly IRunResultCache _resultCache;
        private readonly IConfiguration _configuration;
        private readonly IOptimizeBuildConfiguration _buildOptimizer;
        private readonly AutoTestTestRunner _runner;
        private readonly List<TestFailure> _lastFailures = new List<TestFailure>();
        private int _runCount = 1;
        private bool _isFullRun;
        private Func<bool> _isManualMode;

        private bool _abortProfileRun = true;

        private List<TestRunInfo> _runsToProfile = new List<TestRunInfo>();
        private Action<Platform, Version, Action<ProcessStartInfo, bool>> _profilerWrapper;

        public event EventHandler<EventArgs> ProfilingStarted;
        public event EventHandler<EventArgs> ProfilerInitialized;
        public event EventHandler<EventArgs> ProfilerLoadError;

        public void OnProfilerLoadError(EventArgs e)
        {
            var handler = ProfilerLoadError;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<EventArgs> MinimizerInitialized;

        public void InvokeMinimizerInitialized(EventArgs e)
        {
            var handler = MinimizerInitialized;
            if (handler != null) handler(this, e);
        }

        public void InvokeProfilerInitialized(EventArgs e)
        {
            var handler = ProfilerInitialized;
            if (handler != null) handler(this, e);
        }

        public void InvokeProfilerLoadError(EventArgs e)
        {
            var handler = ProfilerLoadError;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<EventArgs> ProfilerCompletedUpdate;

        public void InvokeProfilerCompletedUpdate(EventArgs e)
        {
            EventHandler<EventArgs> handler = ProfilerCompletedUpdate;
            if (handler != null) handler(this, e);
        }

        public void SetManualUpdateProvider(Func<bool> isManualMode)
        {
            _isManualMode = isManualMode;
        }

        public MinimizingPreProcessor(ICache cache, IRunResultCache resultCache, IConfiguration configuration, IOptimizeBuildConfiguration buildOptimizer, IMessageBus bus, ITestRunner[] runners)
        {
            _cache = cache;
            _resultCache = resultCache;
            _configuration = configuration;
            _buildOptimizer = buildOptimizer;
            _bus = bus;
            _runner = (AutoTestTestRunner)runners.First(x => x.GetType().Name.Equals("AutoTestTestRunner"));
            _minimizer = new TestMinimizer(configuration.AllSettings("mm-MinimizerDebug") == "true", 1);
            _profilerData = new ProfilerData(GetDBName(), new BinaryFileProfilerDataParser(), new ForEveryFixtureConstructorOrFixtureChangeContextChangeFinder());
            _profilerData.DebugMessage += _profilerData_DebugMessage;
            Logger.WriteDebug("Minimizing PreProc Created");
        }

        public string GetDBName()
        {
            var file = Path.Combine(_configuration.WatchPath, Path.GetFileNameWithoutExtension(_configuration.WatchToken) + "_mm_cache.bin");
            return new PathTranslator(_configuration.WatchToken).Translate(file);
        }

        void LoadProfilerData()
        {
            try
            {
                Logger.WriteInfo("Loading up the profiler database.");
                _profilerData.Load();
                Logger.WriteInfo("Profiler Loaded.");
                InvokeProfilerInitialized(new EventArgs());
            }
            catch (Exception ex)
            {
                InvokeProfilerLoadError(new EventArgs());
                Logger.WriteError("Error occured loading profiler database! " + ex);
            }
        }

        void _profilerData_DebugMessage(object sender, ProfilerLogEventArgs e)
        {
            if (_configuration.AllSettings("mm-MinimizerDebug") == "true")
                Logger.WritePreprocessor("Profiler:" + e.Message);
        }

        private void MinimizerMinimizerMessage(object sender, MessageArgs e)
        {
            if (_configuration.AllSettings("mm-MinimizerDebug") == "true")
                Logger.WritePreprocessor(e.MessageType + " " + e.Message);
        }

        public void PauseMinimizer()
        {
            _minimizerPaused = true;
        }

        public void ResumeMinimizer()
        {
            _minimizerPaused = false;
        }

        public void PrepareForFullRun()
        {
            _isFullRun = true;
            if (_profilerData == null) return;
            try
            {
                _profilerData.DeleteAllData();
            }
            catch (Exception ex)
            {
                Logger.WriteDebug("Failed when trying to delete profiler database");
                Logger.Write(ex);
            }
        }

        public void Initialize()
        {
            // If minimizer is off skip all minimizer and profiler stuff
            if (minimizerGraphsAndRiskIsOff())
                return;

            try
            {
                var assemblies = GetAssemblies();
                Logger.WriteDebug("minimizer isdebug = " + _configuration.AllSettings("mm-MinimizerDebug"));
                _minimizer = new TestMinimizer(_configuration.AllSettings("mm-MinimizerDebug") == "true", 1);
                _minimizer.TranslateHistoryFilePathsWith((s) => { return new PathTranslator(_configuration.WatchToken).Translate(s); });
                _minimizer.MinimizerMessage += MinimizerMinimizerMessage;
                
                _minimizer.DoInitialIndexOf(assemblies);
                InvokeMinimizerInitialized(new EventArgs());
                GC.Collect();
                GC.WaitForPendingFinalizers();
                //if (!minimizerGraphsAndRiskIsOff())
                ThreadPool.QueueUserWorkItem(x => LoadProfilerData());
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private List<string> GetAssemblies()
        {
            Logger.WritePreprocessor("Locating assemblies for preprocessor");
            var projects = _cache.GetAll<Project>();
            Logger.WriteDebug("Assembly build configuration");
            var infos = _buildOptimizer.AssembleBuildConfiguration(projects, true);
            var existingAssemblies = new List<string>();
            foreach (var info in infos)
            {
                if (File.Exists(info.Assembly))
                {
                    if (existingAssemblies.Contains(info.Assembly))
                        continue;
                    existingAssemblies.Add(info.Assembly);
                    Logger.WriteDetails(string.Format("\t{0}", info.Assembly));
                    continue;
                }
                Logger.WriteDetails(string.Format("\tAssembly not present: {0}", info.Assembly));
            }
            existingAssemblies.AddRange(getAdditionalAssemblies().Where(x => !existingAssemblies.Contains(x)));
            return existingAssemblies;
        }

        private List<string> getAdditionalAssemblies()
        {
            var assemblies = new List<string>();
            var setting = _configuration.AllSettings("mm-MinimizerAssemblies");
            if (setting == null)
                return assemblies;
            foreach (var item in setting.Split(new string[] { "</Assembly>" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var assembly = item.Replace("<Assembly>", "").Replace(Environment.NewLine, "");
                if (File.Exists(assembly))
                {
                    assemblies.Add(assembly);
                    Logger.WriteDetails(string.Format("\t{0}", assembly));
                    continue;
                }
                Logger.WriteDetails(string.Format("\tAssembly not present: {0}", assembly));
            }
            return assemblies;
        }

        private RunInfo GetRunInfoFromAssembly(string assembly)
        {
            var infos = _buildOptimizer.AssembleBuildConfiguration(_cache.GetAll<Project>());
            foreach (var info in from i in infos where i.Assembly.Equals(assembly) select i)
                return info;
            return null;
        }

        public PreProcessedTesRuns PreProcess(PreProcessedTesRuns preProcessed)
        {
            // Clean up profiler logs before we start
            var logFiles = getProfilerOutput();
            cleanUpFiles(logFiles);

            var runMinimized = !(_isFullRun || minimizerIsDisabled() || _minimizerPaused);
            Logger.WriteDebug("Run minimized is " + runMinimized.ToString());
            if (runMinimized)
                _bus.Publish(new RunInformationMessage(InformationType.PreProcessing, "", "", typeof(MinimizingPreProcessor)));
            var finalDetails = new List<RunInfo>();
            var details = preProcessed.RunInfos;
            if (!minimizerGraphsAndRiskIsOff())
            {
                try
                {
                    var assemblies = GetAssemblies();
                    //var hash = details.ToDictionary(current => current.Assembly);
                    if (!runMinimized || _runCount % 10 == 0)
                        _minimizer.LoadOldCachedFiles(assemblies);
                    _runCount++;
                    var tests = _minimizer.GetTestsFor(assemblies);
                    Logger.WriteDebug("minimizer returns " + tests.Count + " tests");

                    if (runMinimized)
                    {
                        var profiled = GetProfiledEntries();
                        Logger.WriteDebug("profiler returns " + profiled.Count() + " tests");
                        var all = Combine(tests, profiled);

                        //TODO THIS IS A HACK TO ENRICH PROFILED TESTS, REFACTOR ME
                        foreach (var t in all)
                        {
                            var original = t.TestAssembly
                                .Replace(".mm.dll", ".dll")
                                .Replace(".mm.exe", ".exe");
                            var testAssembly = original;
                            testAssembly = new PathTranslator(_configuration.WatchToken).TranslateFrom(testAssembly);
                            if (testAssembly == null)
                                testAssembly = original;
                            Logger.WriteDebug("Translated TestAssembly is: " + testAssembly + " original is: " + t.TestAssembly);
                            var current = details.FirstOrDefault(x => x.Assembly.Equals(testAssembly));
                            if (current == null)
                            {
                                current = finalDetails.FirstOrDefault(x => x.Assembly.Equals(testAssembly));
                                if (current == null)
                                    current = GetRunInfoFromAssembly(testAssembly);
                                if (current == null)
                                    throw new Exception("unable to match assembly for test. - assembly is " + testAssembly);
                                if (!finalDetails.Exists(x => x.Assembly.Equals(testAssembly)))
                                    finalDetails.Add(current);
                            }

                            foreach (var s in t.TestRunners)
                            {
                                var runner = ConvertStringToTestRunnerEnum(s);
                                var test = getTestSignature(t, runner);
                                Logger.WriteDetails(string.Format("Adding test {0} to runner {1} on run info {2}", test,
                                                                  runner, current.Assembly));
                                current.AddTestsToRun(runner, test);
                            }
                        }

                        if (runMinimized)
                            addCurrentBrokenTests(details);

                        foreach (var detail in details)
                        {
                            detail.ShouldOnlyRunSpcifiedTestsFor(TestRunner.Any);
                        }
                        Logger.WritePreprocessor(string.Format("Found {0} affected tests", tests.Count));
                        _bus.Publish(new AssembliesMinimizedMessage());
                        finalDetails.AddRange(details);
                    }

                }
                catch (Exception ex)
                {
                    Logger.WriteError(ex.ToString());
                    var newDetails = new List<RunInfo>();
                    foreach (var detail in details)
                    {
                        var newDetail = new RunInfo(detail.Project);
                        newDetail.SetAssembly(detail.Assembly);
                        if (detail.ShouldBeBuilt)
                            newDetail.ShouldBuild();
                        newDetails.Add(newDetail);
                    }
                }

                Logger.WriteDebug("Running 2nd generation garbage collection");
                GC.Collect(2);
                Logger.WriteDebug("Waiting for finalizers");
                GC.WaitForPendingFinalizers();
                Logger.WriteDebug("GC done");
            }

            Logger.WriteDebug("Getting profiler wrapper");
            _profilerWrapper = getProfilerWrapper();
            if (!runMinimized)
            {
                var wrapper = _profilerWrapper;
                _profilerWrapper = null;
                Logger.WriteDebug("Returning original runinfos");
                return new PreProcessedTesRuns(wrapper, preProcessed.RunInfos); // return original runInfos
            }
            finalDetails.ForEach(x => _runsToProfile.Add(x.CloneToTestRunInfo()));
            Logger.WriteDebug("Returning modified details");
            return new PreProcessedTesRuns(_profilerWrapper, finalDetails.ToArray());
        }

        private bool minimizerGraphsAndRiskIsOff()
        {
            return _configuration.AllSettings("mm-MinimizerLevel").ToLower().Equals("off") ||
                   _configuration.AllSettings("mm-AllDisabled").ToLower().Equals("true");
        }

        private bool minimizerIsDisabled()
        {
            return _configuration.AllSettings("mm-MinimizerLevel").ToLower().Equals("off") ||
                _configuration.AllSettings("mm-MinimizerLevel").ToLower().Equals("notrun") ||
                _configuration.AllSettings("mm-AllDisabled").ToLower().Equals("true");
        }

        private IEnumerable<TestEntry> Combine(IEnumerable<TestEntry> tests, IEnumerable<TestEntry> profiled)
        {
            var hash = tests.ToSafeDictionary(x => x.Key);
            var sent = new Dictionary<string, bool>();

            foreach (var t in tests) yield return t;
            foreach(var p in profiled)
            {
                if (!hash.ContainsKey(p.Key) && !sent.ContainsKey(p.Key))
                {
                    //Logger.WriteDebug("adding test from profiler " + p.Key);
                    sent.Add(p.Key, true);
                    yield return p;
                }
                //{
                    //Logger.WriteDebug("profiled test " + p.Key + " already found.");
                //}
            }
        }

        public TestInformationGeneratedMessage GetTestInformationFor(string name)
        {
            var ret = new TestInformationGeneratedMessage(name) {Test = new Chain("GeneratedTest", name)};
            var info = _profilerData.GetTestInformationFor(name);
            if (info == null) return ret;
            foreach(var setup in info.Setups) {ret.Test.Children.Add(BuildChain(setup, true, false, false)); }
            ret.Test.Children.Add(BuildChain(info.TestChain, false, true, false));
            foreach (var teardown in info.Teardowns) { ret.Test.Children.Add(BuildChain(teardown, false, false, true)); }
            return ret;
        }

        private static Chain BuildChain(CallChain callChain, bool isSetup, bool isTest, bool isTeardown )
        {
            var chain = new Chain(callChain.Runtime, callChain.Name);
            chain.IsSetup = isSetup;
            chain.IsTest = isTest;
            chain.IsTeardown = isTeardown;
            chain.TimeStart = callChain.StartTime;
            chain.TimeEnd = callChain.EndTime;
            foreach(var child in callChain.Children)
            {
                chain.Children.Add(BuildChain(child, false, false, false));
            }
            return chain;
        }

        private IEnumerable<TestEntry> GetProfiledEntries()
        {
            try
            {
                Logger.WriteDebug("Getting profiled entries");
                var ret = new List<TestEntry>();
                var changes = _minimizer.GetLastChanges();
                Logger.WriteDebug("Got last changes");
                foreach (var change in changes)
                {
                    if (change.ChangeType == ChangeType.Remove)
                    {
                        _profilerData.Remove(change.ItemChanged);
                    }
                }
                Logger.WriteDebug("Removed all with change type remove");
                foreach (var change in changes)
                {
                    if (change.ChangeType == ChangeType.Modify)
                    {
                        var profiled = _profilerData.GetTestsFor(change.ItemChanged);
                        //foreach (var p in profiled) Logger.WriteDebug("got profiled test of " + p);
                        var entries = _minimizer.GetTestEntriesFor(profiled);
                        //Logger.WriteDebug("got " + entries.Count() + " test entries back for change of " + change.ItemChanged);
                        ret.AddRange(entries);
                    }
                }
                Logger.WriteDebug("Profiler tests added");
                return ret;
            }
            catch (Exception ex)
            {
                Logger.WriteError("Failed while getting profiled tests");
                Logger.WriteError(ex.ToString());
            }
            return new TestEntry[] {};
        }

        private Action<Platform, Version, Action<ProcessStartInfo, bool>> getProfilerWrapper()
        {
            if (_configuration.AllSettings("mm-ProfilerSetup")  == "DONTRUN" || minimizerGraphsAndRiskIsOff())
                return null;

            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
                return null;

            long writtenSize = 0;
            var abortWhenTooBig = _configuration.AllSettings("mm-ProfilerSetup") == "RUNANDAUTODETECT" ||
                                  _configuration.AllSettings("mm-ProfilerSetup").Trim() == "";

            return (platform, framework, process) =>
                {
					try
					{
	                    var fileName = generateProfilerOutputName(platform, framework);
	                    var fs = File.Create(fileName);

	                    Profiler.RunProcess(new Filter() { BufferSize = 1024 * 1024, ThresholdSize = 2 * 1024, Includes = getProjectReferences() },
	                        (environment) =>
	                        {
	                            var startInfo = new ProcessStartInfo();
	                            if (platform == Platform.x86)
	                                environment(startInfo.EnvironmentVariables, ProfilerRuntime.Runtime32);
	                            else
	                                environment(startInfo.EnvironmentVariables, ProfilerRuntime.Runtime64);
	                            Logger.WriteDebug("Launching test runner. Logging to " + fileName);
	                            process.Invoke(startInfo, true);
	                        },
	                        (length, buffer) => 
                                {
                                    fs.Write(buffer, 0, length);

                                    writtenSize += length;
                                    if (abortWhenTooBig && writtenSize > 1000000000) // If more than 1 gig
                                    {
                                        abortWhenTooBig = false; // Stop duplicate messages from happening
                                        _bus.Publish(new AbortMessage("ProfilerAborted"));
                                    }
                                },
                            exception => Logger.WriteError(exception.ToString()),
	                        fs.Close);
					}
					catch (Exception ex)
					{
						Logger.WriteError(ex.ToString());
                        // If everything fails just run tests as normal
                        process.Invoke(new ProcessStartInfo(), false);
					}
                };
        }

        private string getProjectReferences()
        {
            var references = new List<string>();
            var list = "";
            _cache.GetAll<Project>()
                .Where(x => x.Value != null).ToList()
                .ForEach(proj => references.Add(proj.Value.DefaultNamespace));
            getAdditionalNamespaces().ForEach(references.Add);
            references.ForEach(x => list += x + ", ");
            if (list.Length != 0)
                list = list.Substring(0, list.Length - 2);
            Logger.WriteDebug("Including in profiling: " + list);
            if (list.Length == 0)
                return null;
            return list;
        }

        private List<string> getAdditionalNamespaces()
        {
            var namespaces = new List<string>();
            var setting = _configuration.AllSettings("mm-ProfilerNamespaces");
            if (setting == null)
                return namespaces;
            foreach (var item in setting.Split(new[] { "</Namespace>" }, StringSplitOptions.RemoveEmptyEntries))
                namespaces.Add(item.Replace("<Namespace>", "").Replace(Environment.NewLine, ""));
            return namespaces;
        }

        public List<string> getProfilerOutput()
        {
            var dir = Path.GetTempPath();
            var filepattern = string.Format("mm_output_{0}*.log", Process.GetCurrentProcess().Id);
            return Directory.GetFiles(dir, filepattern).ToList();
        }

        private string generateProfilerOutputName(Platform platform, Version framework)
        {
            var activeRunGuid = Guid.NewGuid();
            var dir = Path.GetTempPath();
            var filename = string.Format("mm_output_{0}_{1}_v{2}_{4}_{3}.log", Process.GetCurrentProcess().Id, platform.ToString(), framework.ToString(), activeRunGuid.ToString(), Path.GetFileNameWithoutExtension(Path.GetTempFileName()));
            return Path.Combine(dir, filename);
        }

        private static string getTestSignature(TestEntry t, TestRunner runner)
        {
            var test = t.TestClass.Replace("/", "+") + "." + t.TestName;
            if (runner == TestRunner.MSpec)
                test = t.TestName.Replace("/", "+");
            Logger.WriteDebug("Adding translated test " + test);
            return test;
        }

        private void addCurrentBrokenTests(RunInfo[] details)
        {
            foreach (var info in details)
            {
                info.AddTestsToRun(getTestsFor(TestRunner.NUnit, info, _resultCache.Failed));
                info.AddTestsToRun(getTestsFor(TestRunner.NUnit, info, _resultCache.Ignored));
                info.AddTestsToRun(getTestsFor(TestRunner.XUnit, info, _resultCache.Failed));
                info.AddTestsToRun(getTestsFor(TestRunner.XUnit, info, _resultCache.Ignored));
                info.AddTestsToRun(getTestsFor(TestRunner.MSTest, info, _resultCache.Failed));
                info.AddTestsToRun(getTestsFor(TestRunner.MSTest, info, _resultCache.Ignored));
                info.AddTestsToRun(getTestsFor(TestRunner.MSpec, info, _resultCache.Failed));
                info.AddTestsToRun(getTestsFor(TestRunner.MSpec, info, _resultCache.Ignored));
                info.AddTestsToRun(getTestsFor(TestRunner.MbUnit, info, _resultCache.Failed));
                info.AddTestsToRun(getTestsFor(TestRunner.MbUnit, info, _resultCache.Ignored));
            }
        }

        public RiskMetricGeneratedMessage GetRiskMetricsFor(string cacheName)
        {
            var ret = new RiskMetricGeneratedMessage();
            var graph = _minimizer.GetGraphFor(cacheName, false);
            Logger.WriteDebug("risk graph generated with " + graph.AllNodes().Count() + " nodes " +
                              graph.AllConnections().Count() + " connections.");
            ret.Signature = cacheName;
            var root = graph.AllNodes().FirstOrDefault(x => x.IsRootNode);
            ret.Found = root != null;
            var profilerEntries = new List<string>();
            if (_configuration.AllSettings("mm-ProfilerSetup") == "DONTRUN")
            {
                ret.NumberOfTests = graph.AllNodes().Count(x => x.IsTest);
                graph.AllNodes().Where(x => x.IsTest).ForEach(x => x.MarkAsProfiled());
            }
            else
            {
                EnrichGraphWithProfilerInformation(cacheName, graph);
                profilerEntries = _profilerData.GetTestsFor(cacheName).ToList();
                ret.NumberOfTests = profilerEntries.Count;
            }
            ret.NodeCount = graph.AllNodes().Count();
            Logger.WriteDebug("node count = " + ret.NodeCount + " tests = " + ret.NumberOfTests); 
            if(root != null)
            {
                ret.Descriptors = new List<string>();
                if (root.TestDescriptors != null)
                {
                    foreach (var desc in root.TestDescriptors)
                    {
                        ret.Descriptors.Add(desc.TestRunner);
                    }
                }
                ret.Complexity = root.Complexity;
            }
            var counts = _profilerData.GetCountsAndTimesFor(cacheName);
            if (counts != null)
            {
                ret.AverageTime = counts.AverageTime;
                ret.CalledCount = counts.TimesCalled;
                ret.AverageTimeUnder = counts.AverageTimeUnder;
            }
            else
            {
                Logger.WriteDebug("null for timing counts");
            }
            var testsScore = _coverageClassifier.CalculateRiskFor(graph);
            var graphScore = _graphPathsClassifier.CalculateRiskFor(graph);
            ret.RiskMetric = (int)(testsScore * .7m + graphScore * .3m);
            ret.TestsScore = 100 - testsScore;
            ret.GraphScore = 100 - graphScore;
            return ret;
        }

        private void EnrichGraphWithProfilerInformation(string name, AffectedGraph graph)
        {
            var profilerOff = _configuration.AllSettings("mm-ProfilerSetup") == "DONTRUN";
            var items = _profilerData.GetTestsFor(name);
            foreach (var entry in items)
            {
                var node = graph.GetNode(entry);
                if (node != null)
                {
                    node.MarkAsProfiled();
                }
            }
        }

        private readonly IGraphRiskClassifier _coverageClassifier = new CoverageDistanceAndComplexityGraphRiskClassifier();
        private readonly IGraphRiskClassifier _graphPathsClassifier = new TestPathsGraphRiskClassifier();
        public VisualGraphGeneratedMessage GetLastAffectedGraph()
        {

            var ret = new VisualGraphGeneratedMessage();
            var graph = _minimizer.GetLastAffectedGraph();
            if (graph == null) return ret;
            Logger.WriteDebug("Getting last affected graph from the minimizer");
            ConvertToGraphMessage(ret, graph);
            Logger.WriteDebug("Graph has " + graph.AllNodes().Count() +  "nodes.");
            return ret;
        }

        public VisualGraphGeneratedMessage GetProfiledGraphFor(string cacheName)
        {

            var ret = new VisualGraphGeneratedMessage();
            if (minimizerGraphsAndRiskIsOff())
                return ret;
            Logger.WriteDebug("Generating graph from profiler for '" + cacheName + "'");
            var graph = _profilerData.GetProfiledGraphFor(cacheName);
            Logger.WriteDebug("graph generated with " + graph.AllNodes().Count() + " nodes " + graph.AllConnections().Count() + " connections.");
            _minimizer.EnrichGraph(graph);
            ConvertToGraphMessage(ret, graph);
            return ret;
        }

            
        
        public VisualGraphGeneratedMessage GetVisualizationGraphFor(string cacheName)
        {
            var ret = new VisualGraphGeneratedMessage();
            if (minimizerGraphsAndRiskIsOff())
                return ret;
            Logger.WriteDebug("Generating graph from minimizer for '" + cacheName + "'");
            var graph = _minimizer.GetGraphFor(cacheName, _configuration.AllSettings("mm-MinimizerDebug") == "true");
            Logger.WriteDebug("graph generated with " + graph.AllNodes().Count() + " nodes " + graph.AllConnections().Count() + " connections.");
            EnrichGraphWithProfilerInformation(cacheName, graph);
            ConvertToGraphMessage(ret, graph);
            return ret;
        }

        private static void ConvertToGraphMessage(VisualGraphGeneratedMessage message, AffectedGraph graph)
        {
            foreach (var node in graph.AllNodes())
            {
                message.Nodes.Add(new GraphNode
                                  {
                                      Assembly = node.Assembly,
                                      DisplayName = node.DisplayName,
                                      FullName = node.FullName,
                                      IsInterface = node.IsInterface,
                                      IsChange = node.IsChange,
                                      IsRootNode = node.IsRootNode,
                                      IsTest = node.IsTest,
                                      Name = node.Name,
                                      Type = node.Type,
                                      InTestAssembly = node.InTestAssembly,
                                      IsProfiledTest = node.Profiled,
                                      Complexity = node.Complexity,
                                  });
            }
            foreach (var c in graph.AllConnections())
            {
                message.Connections.Add(new Connection {From = c.From, To = c.To});
            }
        }

        private TestToRun[] getTestsFor(TestRunner runner, RunInfo info, TestItem[] cachedTests)
        {
            var tests = new List<TestToRun>();
            foreach (var test in cachedTests)
            {
                if (test.Key.Equals(info.Assembly) && test.Value.Runner.Equals(runner) && !infoContainsTest(info, test))
                    tests.Add(new TestToRun(runner, test.Value.Name));
            }
            return tests.ToArray();
        }

        private static bool infoContainsTest(RunInfo info, TestItem test)
        {
            return info.GetTestsFor(test.Value.Runner).Any(infoTest => infoTest.Equals(test.Value.Name));
        }

        private static TestRunner ConvertStringToTestRunnerEnum(string s)
        {
            if (s == "nunit") return TestRunner.NUnit;
            if (s == "mstest") return TestRunner.MSTest;
            if (s == "xunit") return TestRunner.XUnit;
            if (s == "mspec") return TestRunner.MSpec;
            if (s == "simpletesting") return TestRunner.SimpleTesting; 
            return TestRunner.Any;
        }

        public void RunStarted()
        {
            _abortProfileRun = true;
        }

        public void RunFinished(TestRunResults[] resultset)
        {
            _isFullRun = false; // Reset always. Will be set again if needed
            _lastFailures.Clear();
            ThreadPool.QueueUserWorkItem(x => LoadProfilerData(null));
        }

        private void LoadProfilerData(object state)
        {
            
            var logFiles = getProfilerOutput();
            _abortProfileRun = false;
            if (_profilerWrapper != null)
            {
                /*Logger.WriteDebug("About to launch profiled run");
                if (_runner == null)
                {
                    cleanUpFiles(logFiles);
                    return;
                }*/

                if (ProfilingStarted != null)
                    ProfilingStarted(this, new EventArgs());

                /*Logger.WriteDebug("Runner is present, starting run");
                _runner.DisableRunnerFeedback();
                var result = _runner.RunTests(_runsToProfile.ToArray(), _profilerWrapper, () => { return _abortProfileRun; });
                Logger.WriteDebug("Run finished continuing to profile information");

                logFiles = getProfilerOutput();

                if (!_abortProfileRun)
                    _runsToProfile.Clear();
                _profilerWrapper = null;

                if (_abortProfileRun)
                {
                    cleanUpFiles(logFiles);
                    return;
                }*/
            }
            else
            {
                if (ProfilingStarted != null)
                    ProfilingStarted(this, new EventArgs());
            }

            if (_abortProfileRun)
            {
                cleanUpFiles(logFiles);
                return;
            }

            try
            {
                foreach (var logFile in logFiles)
                {
                    if (File.Exists(logFile))
                    {
                        Logger.WriteDebug("Handling profiler info for " + logFile);
                        GetProfilerInfo(logFile);
                    }
                    else
                        Logger.WriteDebug("Profiler log file " + logFile + " does not exist.");
                }
                InvokeProfilerCompletedUpdate(new EventArgs());
                if (_profilerData.Waste > _profilerData.TotalSize * .5)
                {
                    _profilerData.Compress();
                }
                Logger.WriteDebug("Profiling completed");
            }
            catch (Exception ex)
            {
                Logger.WriteError("Error occurred loading profiler data.");
                Logger.WriteError(ex.Message + " \n\n " + ex);
                InvokeProfilerCompletedUpdate(new EventArgs());
            }
            finally
            {
                cleanUpFiles(logFiles);
            }
            GC.Collect(2);
            killProfilerLogsLeftBehind();
        }

        private void killProfilerLogsLeftBehind()
        {
            var dir = Path.GetTempPath();
            var filepattern = "mm_output_*.log";
            var files = Directory.GetFiles(dir, filepattern).ToList();
            foreach (var file in files) {
                try {
                    var pid = new ProfilerLogFileNameParser(file).GetProcessID();
                    if (!Process.GetProcesses().Any(x => x.Id == pid)) {
                        Logger.WriteDebug("Killing profiler log belonging to terminated process: " + file);
                        File.Delete(file);
                    }
                } catch (Exception ex) {
                    Logger.WriteError(ex.ToString());
                }
            }
        }

        private static void cleanUpFiles(IEnumerable<string> logFiles)
        {
            Logger.WriteDebug(string.Format("About to delete {0} profiler log files", logFiles.Count()));
            logFiles.ToList().ForEach(x =>
            {
                Logger.WriteDebug("Deleting log file " + x);
                try
                {
                    File.Delete(x);
                }
                catch (Exception ex)
                {
                    Logger.WriteError(ex.ToString());
                }
            });
        }

        private void GetProfilerInfo(string logFile)
        {
            Logger.WriteDebug("Updating profiler information from " + logFile);
            _profilerData.UpdateInfo(logFile, _minimizer.GetTestEnricher());
            Logger.WriteDebug("Updates profiler database now: " + _profilerData.TotalEntries + " entries. Waste is " + _profilerData.Waste);
            var dbentries = _profilerData.GetProfiledMethods();
            Logger.WriteDebug("there are " + dbentries.Count() + " items in count database.");
        }

        
    }

    public class MinimizingBuildPreProcessor : IPreProcessBuildruns
    {
        public RunInfo[] PostProcess(RunInfo[] details, ref RunReport runReport)
        {
            return details;
        }

        public BuildRunResults PostProcessBuildResults(BuildRunResults runResults)
        {
            return runResults;
        }

        public RunInfo[] PreProcess(RunInfo[] details)
        {
            Logger.WriteDebug("Collection loose ends to prevent locked files before build runs.");
            ModuleDefinition.KillAllReadModules();
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
            return details;
        }
    }
}
