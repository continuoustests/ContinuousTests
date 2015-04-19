using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AutoTest.Client.Handlers;
using AutoTest.VM.Messages;
using AutoTest.Client.Logging;
using AutoTest.Client.Caching;
using AutoTest.Client.UI;
using AutoTest.Messages;
using System;
using AutoTest.Client.Config;
using AutoTest.VM.Messages.Communication;
using AutoTest.Client.VersionCheck;
using System.Net;
using System.Threading;
using System.ComponentModel;
using AutoTest.VM.Messages.License;
using AutoTest.Client.HTTP;
namespace AutoTest.Client
{
    public class ATEClient
    {
        private readonly SynchronizationContext _syncContext;
        private readonly Host _host = new Host();
        private readonly VM _vm = new VM();
        private StandaloneVMCreator _standaloneVmCreator;
        private IStartupHandler _startupHandler;
        private readonly SystemCache _systemCache;
        private IValidateLicense _license;
        private StartupParams _startupParams = new StartupParams("");
        private readonly VMConnectHandle _vmHandle = new VMConnectHandle();
        private List<OnDemandRun> _lastOnDemandRun;
        private string _lastRelatedTestRun;
        private bool _supressUI;

        public MMConfiguration MMConfiguration { get { return _vm.MMConfiguration; } }
        public bool IsRunning { get { return _vm.IsConnected; } }
        public bool HasLastOnDemandRun { get { return _lastOnDemandRun != null || _lastRelatedTestRun != null; } }

        public List<OnDemandRun> LastOnDemandRun { get { return _lastOnDemandRun; } }

        public ATEClient()
        {
            _syncContext = AsyncOperationManager.SynchronizationContext;
            _host.VMSpawned +=HostVmSpawned;
            _vm.RecievedSystemMessage += VmRecievedSystemMessage;
            _vm.RecievedMessage += VmRecievedMessage;
            _systemCache = new SystemCache();
            _license = new RhinoValidator(Environment.SpecialFolder.LocalApplicationData);
            Analytics.CanSendEventsWhen(() => MMConfiguration.AnonFeedback == true);
        }

        public void Start(StartupParams startup, IStartupHandler handler)
        {
            _startupParams = startup;
            _startupHandler = handler;
            ConnectToVM();
            _license = new RhinoValidator(GetAppDataFolder());
        }

        private void ConnectToVM()
        {
            if (_startupParams.IP == null)
                SpawnVM(_startupParams, _startupHandler);
            else
                ConnectToVM(_startupParams.Port, false);
        }

        public Environment.SpecialFolder GetAppDataFolder()
        {
            if (!_vm.IsConnected)
                return Environment.SpecialFolder.LocalApplicationData;
            if (_standaloneVmCreator == null)
                return Environment.SpecialFolder.CommonApplicationData;
            return Environment.SpecialFolder.LocalApplicationData;
        }

        public void SupressUI()
        {
            _supressUI = true;
        }

        private void SpawnVM(StartupParams startup, IStartupHandler handler)
        {
            if (!ConnectToHost(startup, handler))
                SpawnStandaloneVM(startup, handler);
        }

        private void SpawnStandaloneVM(StartupParams startup, IStartupHandler handler)
        {
            ShutdownCreator();
            _standaloneVmCreator = new StandaloneVMCreator(startup, handler);
            _standaloneVmCreator.VMSpawned += StandaloneVmCreatorVmSpawned;
            _standaloneVmCreator.Create();
        }

        private bool ConnectToHost(StartupParams startup, IStartupHandler handler)
        {
            return _host.SpawnVM(startup, handler);
        }

        public void Stop()
        {
            _vmHandle.Remove();
            _vm.Disconnect();
            if (_standaloneVmCreator != null)
                _standaloneVmCreator.Dispose();
        }

        public void AbortRun()
        {
            _vm.AbortRun();
        }

        public void PauseEngine()
        {
            Analytics.SendEvent("PauseEngine");
            _vm.PauseEngine();
        }

        public void ResumeEngine()
        {
            Analytics.SendEvent("ResumeEngine");
            _vm.ResumeEngine();
        }

        public void TestProfilerCorrupted()
        {
            Analytics.SendEvent("TestProfilerCorrupted");
            _vm.TestProfilerCorrupted();
        }

        
        public void RunAll()
        {
            Analytics.SendEvent("RunAll");
            _vm.RunAll();
        }

        public void RunPartial(IEnumerable<string> projects)
        {
            Analytics.SendEvent("RunPartial");
            _vm.RunPartial(projects);
        }

        public void RunRecursiveRunDetection()
        {
            _vm.RunRecursiveRunDetection();
        }

        public void GetGraphFor(string name)
        {
            Logger.Write("Client getting graph");
            _vm.GetCouplingGraph(name);
        }

        public void GetProfiledGraphFor(string name)
        {
            Logger.Write("Client getting graph");
            _vm.GetProfiledGraph(name);
        }


        public void RunRelatedTestsFor(string member)
        {
            Analytics.SendEvent("RunRelatedTests");
            _vm.RunRelatedTestsFor(member);
            setLastRun(member);
        }

        public void UpdateConfiguration(ConfigurationUpdateMessage message)
        {
            Analytics.SendEvent("UpdateConfiguration");
            _host.UpdateConfiguration(message);
        }

        public void SetCustomOutputpath(string path)
        {
            _vm.SetCustomOutputpath(path);
        }

        public bool IsRegistered()
        {
            return _license.IsInitialized;
        }

        private bool PassesTestQuestion()
        {
            //TODO GFY IS THIS STILL USED ANYHERE OR JUST COPY/PASTED
            return _license.Register("Lay it on me bro", null)
                .Equals("Honestly if you are so bad off that you have to hack this product to get it why didn't you just get in touch? " +
                        "We would probably have sponsored you with a license until you're back on track. " +
                        "Have some guts and be honest, don't just go off and steal everything you want");
        }

        public void ShowAboutBox()
        {
            _syncContext.Post(state =>
                {
                    Logger.Write("Showing about box");
                    var form = new AboutForm(this);
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.ShowDialog();
                }, null);
        }

        public void ShowLicenseRegistration()
        {
            _syncContext.Post(state => showLicenseRegistration(), null);
        }

        private void showLicenseRegistration()
        {
            Logger.Write("Registering license");
            var form = new RegisterLicenseFrom(GetAppDataFolder()) {StartPosition = FormStartPosition.CenterScreen};
            if (form.ShowDialog() == DialogResult.OK)
                HasValidLicense();
        }

        private void HasValidLicense()
        {
            Logger.Write("Sending license valid message");
            if (_startupHandler != null)
                _startupHandler.Consume(new ValidLicenseMessage());
            _vm.ResumeEngine();
        }

        public void ShowMessageForm()
        {
            _syncContext.Post((state) =>
                {
                    var form = new SystemMessageForm();
                    foreach (var message in _systemCache.Messages)
                    {
                        if (message.GetType() == typeof(InformationMessage))
                            form.AddInformation(((InformationMessage)message).Message);
                        if (message.GetType() == typeof(WarningMessage))
                            form.AddWarning(((WarningMessage)message).Warning);
                        if (message.GetType() == typeof(ErrorMessage))
                            form.AddError(((ErrorMessage)message).Error);
                    }
                    form.Show();
                }, null);
        }

        public void ShowConfiguration(bool isLocal)
        {
            _syncContext.Post((state) =>
                {
                    var form = new ConfigurationForm(_startupParams.WatchToken, _vm, isLocal);
                    if (isLocal)
                        form.OnWhateverDo(() => ShowConfiguration(false));
                    form.Show();
                }, null);
        }

        public void RequestManualMinimize()
        {
            _vm.RequestManualMinimize();
        }

        public void StartOnDemandRun(OnDemandRun run)
        {
            Analytics.SendEvent("StartOnDemandRun");
            _vm.StartOnDemandRun(run);
            SetLastRun(run);
        }

        public void StartOnDemandRun(IEnumerable<OnDemandRun> runs)
        {
            Analytics.SendEvent("StartOnDemandRun");
            _vm.StartOnDemandRun(runs);
            SetLastRun(runs);
        }

        public void RunLastOnDemandRun()
        {
            Analytics.SendEvent("RunLastOnDemandRun");
            if (_lastOnDemandRun != null)
                _vm.StartOnDemandRun(_lastOnDemandRun);
            if (_lastRelatedTestRun != null)
                _vm.RunRelatedTestsFor(_lastRelatedTestRun);
        }

        public void GoTo(string file, int line, int column)
        {
            _vm.GoTo(file, line, column);
        }

        public void FocusEditor()
        {
            _vm.FocusEditor();
        }

        public string GetAssemblyFromProject(string projectPath)
        {
            return _vm.GetAssemblyFromProject(projectPath);
        }

        public bool IsSolutionInitialized()
        {
            return _vm.IsSolutionInitialized();
        }

        public void QueueRealtimeChange(RealtimeChangeList list)
        {
            _vm.QueueRealtimeChange(list);
        }

        public void RefreshConfig()
        {
            _startupHandler.Consume(MMConfiguration);
        }

        public void SetLastRun(OnDemandRun run)
        {
            _lastOnDemandRun = new List<OnDemandRun> {run};
            _lastRelatedTestRun = null;
        }

        public void SetLastRun(IEnumerable<OnDemandRun> run)
        {
            _lastOnDemandRun = new List<OnDemandRun>();
            _lastOnDemandRun.AddRange(run);
            _lastRelatedTestRun = null;
        }

        private void setLastRun(string run)
        {
            _lastOnDemandRun = null;
            _lastRelatedTestRun = run;
        }

        void HostVmSpawned(object sender, VMSpawnedArgs e)
        {
            ConnectToVM(e.Message.Port, e.Message.StartedPaused);
        }

        void VmRecievedSystemMessage(object sender, SystemMessageArgs e)
        {
            _systemCache.Add(e.Message);
        }

        void VmRecievedMessage(object sender, MessageArgs e)
        {
            if (e.Message.GetType() == typeof(AbortMessage))
            {
                if (((AbortMessage)e.Message).Reason.Equals("ProfilerAborted"))
                    AskUserToEnableOrDisableProfiler();
            }
        }

        void StandaloneVmCreatorVmSpawned(object sender, VMSpawnedArgs e)
        {
            ConnectToVM(e.Message.Port, e.Message.StartedPaused);
            _startupHandler.VMStarted(e.Message);
            // This is ugly I tell you, UGLY!!
            if (!_supressUI)
                InitializeSolution();
        }

        private void InitializeSolution()
        {
            // TODO GFY WE SHOULD RAISE A MESSAGE FOR CONNECTED SO WE CAN DO THIS.
            Thread.Sleep(500);
            _syncContext.Post((state) =>
            {
                var initializingLicense = !_license.IsInitialized;
                if (initializingLicense)
                    showLicenseRegistration();
                
                if (_vm.IsConnected && MMConfiguration.BuildExecutables.Any() && MMConfiguration.RealtimeFeedback)
                    Analytics.SendEvent("InitializeRealtime");
                else if (_vm.IsConnected && MMConfiguration.BuildExecutables.Any())
                    Analytics.SendEvent("InitializeMighty");
                else if (_vm.IsConnected && !MMConfiguration.BuildExecutables.Any())
                    Analytics.SendEvent("InitializeAuto");
                else
                    Analytics.SendEvent("InitializeManual");

                new Thread(CheckForNewVersion).Start();

                if (_vm.IsSolutionInitialized() && !initializingLicense)
                    return;

                if (!MMConfiguration.IgnoreWarmup)
                {
                    Analytics.SendEvent("WarmUp");
                    var reply = new WarmupForm(_startupParams.WatchToken, _vm).ShowDialog();
                    if (reply == DialogResult.OK)
                    {
                        _vm.RunRecursiveRunDetection();
                        Thread.Sleep(100);
                        _vm.RunAll();
                    }
                }
            }, null);
        }

        private void CheckForNewVersion()
        {
            if (MMConfiguration.IgnoreThisUpgrade.ToLower() == "all")
                return;
            var checker = new DidWeReleaseYet(() =>
                {
                    try
                    {
                        var client = new WebClient();
                        return client.DownloadString("http://www.continuoustests.com/version.xml");
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                    }
                    return "";
                });
            
            if (checker.Released())
            {
                if (MMConfiguration.IgnoreThisUpgrade.ToLower() != checker.Version)
                    _syncContext.Post(state => new NewReleaseForm(checker).ShowDialog(), null);
            }
        }

        private void ConnectToVM(int port, bool startPaused)
        {
            _vm.Connect(port, _startupHandler, _startupHandler, startPaused, _startupParams.WatchToken);
            _vmHandle.Write("127.0.0.1", port, _startupParams.WatchToken);
            if (startPaused)
                _vm.PauseEngine();
        }
         
        private void ShutdownCreator()
        {
            if (_standaloneVmCreator != null)
            {
                _standaloneVmCreator.VMSpawned -= StandaloneVmCreatorVmSpawned;
                _standaloneVmCreator.Dispose();
                _standaloneVmCreator = null;
            }
        }

        private void AskUserToEnableOrDisableProfiler()
        {
            if (_supressUI)
                return;
            Analytics.SendEvent("ProfileDataTooLarge");
            _syncContext.Post((state) =>
              {
                    MessageBox.Show(
                        string.Format("The profiler log of the tests seems to be very large (>1gb). Generally it is not recommended to run profiling in such an environment.{0}{0}You can go to configuration in the ContinuousTests menu and on the minimizer test select to either force profiling or to disable it", Environment.NewLine),
                        "Session aborted",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }, null);
        }

        public void GetRuntimeTestInformationFor(string methodName)
        {
            Logger.Write("Client getting diagram");
            _vm.GetRuntimeTestInformation(methodName);
        }

        public IEnumerable<string> GetProjectBuildList(IEnumerable<string> projects)
        {
            return _vm.GetProjectBuildList(projects);
        }

        public void GetLastAffectedGraph()
        {
            _vm.GetLastAffectedGraph();
        }
    }
}
