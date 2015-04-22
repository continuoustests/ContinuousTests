using System;
using AutoTest.VM.Messages;
using System.Threading;
using System.Diagnostics;
using AutoTest.VM.Messages.Communication;
using System.IO;
using AutoTest.VM.FileSystem;
using AutoTest.Messages;
using AutoTest.VM.Messages.License;

namespace AutoTest.VM
{
    class Program
    {
        private static ATEngine _engine;
        private static readonly object _padLock = new object();
        private static bool _exit = false;
        private static IValidateLicense _license;
        private static ServerFeedback _feedback;
        private static System.Timers.Timer _exitTimer = null;

        private static bool _shouldExit
        {
            get
            {
                lock (_padLock)
                {
                    return _exit;
                }
            }
            set
            {
                lock (_padLock)
                {
                    _exit = value;
                }
            }
        }

        static TcpServer server;

        static void Main(string[] args)
        {
            //args = new string[] { "3456", @"C:\Users\ack\src\Autotest.Net\AutoTest.Net.sln", Guid.NewGuid().ToString(), "debug", "0", "global" };
            LaunchArguments arguments = null;
            try
            {
                arguments = parseArguments(args);
                Logger.SetDebugMode(arguments.Debug);
                Logger.WriteInfo("Starting application");

                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                _feedback = new ServerFeedback();
                _feedback.ClientDisconnected += _feedback_ClientDisconnected;
                _feedback.ClientConnect += _feedback_ClientConnect;
                server = new TcpServer(_feedback);

                server.MessageReceived += OnRecieve;
                server.StartServer("127.0.0.1", arguments.Port);
                var writeLocator = new DefaultConfigFileLocator(server.Port.ToString(), arguments.IsGlobal);
                initializeConfiguration(writeLocator.GetConfigurationFile());
                if (arguments.IsGlobal)
                    _license = new RhinoValidator(Environment.SpecialFolder.CommonApplicationData);
                else
                    _license = new RhinoValidator(Environment.SpecialFolder.LocalApplicationData);

                _engine = new ATEngine(server, writeLocator, arguments.WatchPath, _license, arguments.ConfigPath);
                _engine.Start();

                Logger.WriteInfo(string.Format("VM listening on 127.0.0.1:{0}", server.Port));
                startExitTimer();
                SendVmInitializedMessage(arguments.CorrelationId, server, arguments.OwnerPort);
                int clientsConnected = 0;
                while (!_shouldExit)
                {
                    if (clientsConnected == 0 && server.ClientCount > 0)
                        _engine.ValidateConfiguration();
                    clientsConnected = server.ClientCount;
                    Thread.Sleep(1000);
                    if (isMasterDead(arguments.MasterProcessId))
                        shutdown();
                }
                Logger.WriteDebug("We were told to shut down so now we'll send off a vm terminating message and die");
                if (server.ClientCount > 0)
                    SendVmTerminatingMessage(server, arguments.OwnerPort);
            }
            catch (Exception ex)
            {
                if (arguments != null)
                    Logger.Write(ex);
                else
                    Console.WriteLine(ex.ToString());
            }
            finally
            {
                AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            }
            Logger.WriteDebug("Screw you guys I'm going home!");
        }

        static bool isMasterDead(int processId)
        {
            if (processId == 0)
                return false;
            foreach (var proc in Process.GetProcesses()) {
                if (proc.Id == processId)
                    return false;
            }
            return true;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.WriteError(e.ExceptionObject.ToString());
        }

        private static void initializeConfiguration(string configFile)
        {
            var builder = new ConfigBuilder(configFile, new FSProxy());
            if (builder.IsConfigured())
            {
                Logger.WriteInfo(string.Format("Loading configuration from {0}", configFile));
                return;
            }
            builder.BuildConfiguration();
            Logger.WriteInfo(string.Format("Creating configuration in {0}", configFile));
            File.WriteAllText(configFile, builder.ToString());
        }

        static void _feedback_ClientConnect(object sender, ClientDisconnectedArgs e)
        {
            Logger.WriteDebug("Client connected stopping timer");
            stopTimer();
        }

        static void _feedback_ClientDisconnected(object sender, ClientDisconnectedArgs e)
        {
            Logger.WriteDebug(string.Format("Client disconnected connected clients: {0}", e.ClientCount));
            if (e.ClientCount < 1)
                startExitTimer();
        }

        private static void startExitTimer()
        {
            if (_shouldExit)
                return;
            stopTimer();
            Logger.WriteDebug("Starting timer");
            _exitTimer = new System.Timers.Timer(20000);
            _exitTimer.Enabled = true;
            _exitTimer.Elapsed += _exitTimer_Elapsed;
            _exitTimer.Start();
        }

        static void _exitTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_shouldExit)
                return;
            stopTimer();
            Logger.WriteDebug("Client seems to have left so we are shutting down");
            _shouldExit = true;
        }

        private static void stopTimer()
        {
            if (_shouldExit)
                return;
            if (_exitTimer == null)
                return;
            Logger.WriteDebug("Stopping timer");
            _exitTimer.Stop();
            _exitTimer.Elapsed -= _exitTimer_Elapsed;
            _exitTimer = null;
        }

        private static void SendVmInitializedMessage(Guid correlationId, ITcpServer server, int port)
        {
            var client = new NetClient(new ClientFeedback());
            client.Connect("127.0.0.1", port);
            Logger.WriteDebug(string.Format("About to send VMInitializedMessage for 127.0.0.1:{0}", server.Port));
            client.SendAndWait(new VMInitializedMessage(correlationId, Process.GetCurrentProcess().Id, server.Port, _engine.GetNUnitTestRunner(), _engine.GetMSTestRunner(), _engine.IsLoggingEnabled(), _engine.StartedPaused));
            client.Disconnect();
        }

        private static void SendVmTerminatingMessage(ITcpServer server, int port)
        {
            var client = new NetClient(new ClientFeedback());
            client.Connect("127.0.0.1", port);
            client.SendAndWait(new VMTerminating(server.Port));
            client.Disconnect();
        }

        private static LaunchArguments parseArguments(string[] args)
        {
            int port;
            if (args == null || args.Length < 6)
                throw new Exception("The minimum number of passed arguments are SERVERPORT WATCHPATH CALLBACKCORRELATIONGUID DEBUGMODE OWNERPORT RUNPROFILE [OWNERPID]");
            if (!Int32.TryParse(args[0], out port))
                throw new Exception("Invalid port");
            int ownerPort;
            if (!Int32.TryParse(args[4], out ownerPort))
                throw new Exception("Invalid owner port");
            var debug = args[3];
            int masterProcessId;
            if (args.Length > 6) {
                if (!Int32.TryParse(args[6], out masterProcessId))
                    masterProcessId = 0;
            } else {
                masterProcessId = 0;
            }
            var configPath = args[1];
            if (args.Length > 7) {
                configPath = args[7];
            }
            return new LaunchArguments(new Guid(args[2]), port, args[1], debug, ownerPort, args[5], masterProcessId, configPath);
        }

        private static void shutdown()
        {
            Logger.WriteDebug("Stopping engine and exiting");
            _engine.Stop();
            _shouldExit = true;
        }

        static void OnRecieve(object sender, MessageReceivedEventArgs e)
        {
            Logger.WriteDebug("received A message of type " + e.Message.GetType());

            if (e.Message == null) Logger.WriteDebug("message is null?");
            if (_engine == null) Logger.WriteDebug("ENG is null?");
            
            if (e.Message.GetType() == typeof(RunRecursiveRunDetectorMessage))
            {
                Logger.WriteDebug("Runing recursive run detector");
                _engine.SetupRecursiveCauseDetectorAsNextTrackerType();
                return;
            }
            if (e.Message.GetType() == typeof(ForceFullRunMessage))
            {
                Logger.WriteDebug("Forcing full test run");
                _engine.DoFullRun();
                return;
            }
            if (e.Message.GetType() == typeof(PauseVMMessage))
            {
                Logger.WriteDebug("Pausing VM");
                _engine.Pause();
                return;
            }
            if (e.Message.GetType() == typeof(RequestVisualGraphMessage))
            {
                var m = e.Message as RequestVisualGraphMessage;
                Logger.WriteDebug("Building graph for: " + m.Item);
                var graph = _engine.GetCouplingGraph(m.Item);
                Logger.WriteDebug("Returning graph for: " + m.Item);
                server.Send(graph);
                return;
            }
            if (e.Message.GetType() == typeof(RequestProfiledGraphMessage))
            {
                var m = e.Message as RequestProfiledGraphMessage;
                Logger.WriteDebug("Building profile graph for: " + m.Item);
                var graph = _engine.GetProfiledGraph(m.Item);
                Logger.WriteDebug("Returning profiled graph for: " + m.Item);
                server.Send(graph);
                return;
            }
            if (e.Message.GetType() == typeof(RequestRuntimeTestInformationMessage))
            {
                var m = e.Message as RequestRuntimeTestInformationMessage;
                Logger.WriteDebug("Building runtime test info for: " + m.Item);
                var runtimeTestInformation = _engine.GetRuntimeTestInformation(m.Item);
                Logger.WriteDebug("Returning runtime test info for: " + m.Item);
                server.Send(runtimeTestInformation);
                return;
            }
            if (e.Message.GetType() == typeof(RequestRiskMetricsMessage))
            {
                var m = e.Message as RequestRiskMetricsMessage;
                Logger.WriteDebug("Building metrics for: " + m.Item);
                if(_engine == null) Logger.WriteDebug("engine was null");
                var ret = _engine.GetRiskMetricFor(m.Item);
                Logger.WriteDebug("Returning metrics for: " + m.Item);
                server.Send(ret);
                return;
            }
            if (e.Message.GetType() == typeof(ResumeVMMessage))
            {
                Logger.WriteDebug("Resuming VM");
                if (!_engine.IsRunning)
                    _engine.Start();
                _engine.Resume();
                return;
            }
            if (e.Message.GetType() == typeof(TerminateMessage))
            {
                shutdown();
                return;
            }
            if (e.Message.GetType() == typeof(TestProfilerCorruptedMessage))
            {
                var corrupted = _engine.TestProfilerIsCorrupted();
                if(corrupted != null) server.Send(corrupted);
                return;
            }
            if (e.Message.GetType() == typeof(OnDemandTestRunMessage))
            {
                Logger.WriteDebug("Starting on demand tests");
                var message = (OnDemandTestRunMessage)e.Message;
                _engine.StartOnDemandTestRun(message.Runs);
            }
            if (e.Message.GetType() == typeof(RunRelatedTestsMessage))
            {
                Logger.WriteDebug("Starting run related tests");
                var message = (RunRelatedTestsMessage)e.Message;
                _engine.RunRelatedTests(message.Member);
            }
            if (e.Message.GetType() == typeof(ConfigurationUpdateMessage))
            {
                var message = (ConfigurationUpdateMessage)e.Message;
                if (message.RequiresRestart)
                {
                    Logger.WriteDebug("Restarting engine");
                    _engine.Stop();
                    _engine.Start();
                }
                else
                {
                    Logger.WriteDebug("Reloading configuration");
                    _engine.RefreshConfiguration();
                }
            }
            if (e.Message.GetType() == typeof(ManualMinimizationRequestMessage))
            {
                Logger.WriteDebug("Manual minimization requested");
                _engine.PerformManualMinimization();
            }
            if (e.Message.GetType() == typeof(UpdateCustomOutputpathMessage))
            {
                Logger.WriteDebug("Output path update requested");
                _engine.SetCustomOutputPath(((UpdateCustomOutputpathMessage)e.Message).Outputpath);
            }
            if (e.Message.GetType() == typeof(GoToFileAndLineMessage))
            {
                Logger.WriteDebug("Go to file requested");
                _engine.GoTo(((GoToFileAndLineMessage)e.Message));
            }
            if (e.Message.GetType() == typeof(FocusEditorMessage))
            {
                Logger.WriteDebug("Focus editor requested");
                _engine.Focus();
            }
            if (e.Message.GetType() == typeof(RequestMessage<AssemblyPathRequest, AssemblyPathResponse>))
            {
                Logger.WriteDebug("Assembly path for project requested");
                var msg = (RequestMessage<AssemblyPathRequest, AssemblyPathResponse>)e.Message;
                server.Send(new AssemblyPathResponse(msg.CorrelationID, _engine.GetAssemblyFromProject(msg.Request.Project)));
            }
            if (e.Message.GetType() == typeof(RequestMessage<IsSolutionInitializedRequest, IsSolutionInitializedResponse>))
            {
                Logger.WriteDebug("Assembly path for project requested");
                var msg = (RequestMessage<IsSolutionInitializedRequest, IsSolutionInitializedResponse>)e.Message;
                server.Send(new IsSolutionInitializedResponse(msg.CorrelationID, _engine.IsSolutionInitialized()));
            }
            if (e.Message.GetType() == typeof(PartialRunMessage))
            {
                Logger.WriteDebug("Assembly path for project requested");
                var msg = (PartialRunMessage)e.Message;
                _engine.DoPartialRun(msg.Projects);
            }
            if (e.Message.GetType() == typeof(RealtimeChangeList))
            {
                Logger.WriteDebug("Realtime change received");
                var msg = (RealtimeChangeList)e.Message;
                _engine.QueueRealtimeRun(msg);
            }
            if (e.Message.GetType() == typeof(RequestMessage<OrderedBuildList, OrderedBuildList>))
            {
                Logger.WriteDebug("Building ordered build list");
                var msg = (RequestMessage<OrderedBuildList, OrderedBuildList>)e.Message;
                server.Send(new OrderedBuildList(msg.CorrelationID, _engine.BuildOrderedProjectList(msg.Request)));
            }
            if (e.Message.GetType() == typeof(GetLastAffectedGraphMessage))
            {
                Logger.WriteDebug("Getting last affected graph");
                server.Send(_engine.GetLastRunCouplingGraph());
            }
            if (e.Message.GetType() == typeof(AbortMessage))
            {
                Logger.WriteDebug("Aborting current run");
                _engine.AbortRun();
            }
            //TODO SERIOUSLY WTF WE NEED TO REFACTOR THIS!!!
            //LOL! yeah :)
        }
    }
}
