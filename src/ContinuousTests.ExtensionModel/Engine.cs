using System;
using System.Collections.Generic;
using System.Linq;
using AutoTest.VM.Messages.Communication;
using AutoTest.Client;
using AutoTest.Client.Handlers;
using AutoTest.VM.Messages;
using ContinuousTests.ExtensionModel.Arguments;
using System.Threading;
using System.ComponentModel;
using AutoTest.UI;

namespace ContinuousTests.ExtensionModel
{
    class Engine : IEngine, IStartupHandler
    {
        private readonly string _watchToken;
        private readonly string _continuousTestsPath;
        private readonly VMHandle _handle;
        private bool _hasConnected;

        private SynchronizationContext _syncCtx;
        private readonly object _feedbackLock = new object();
        private ATEClient _client;

        public event EventHandler EngineConnecting;
        public event EventHandler EngineDisconnecting;
        public event EventHandler SessionStarted;
        public event EventHandler<BuildFinishedArgs> BuildFinished;
        public event EventHandler<TestsFinishedArgs> TestsFinished;
        public event EventHandler<ImmediateTestFeedbackArgs> TestProgress;
        public event EventHandler<SessionFinishedArgs> SessionFinished;

        public bool IsReady { get { return _hasConnected; } }

        public Engine(string watchToken, string continuousTestsPath)
        {
            _watchToken = watchToken;
            _continuousTestsPath = continuousTestsPath;
            _handle = null;
            initialize();
        }

        public Engine(VMHandle handle)
        {
            _handle = handle;
            _watchToken = null;
            initialize();
        }

        private void initialize()
        {
            _syncCtx = AsyncOperationManager.SynchronizationContext;
            _client = new ATEClient();
            _client.SupressUI();
        }

        public void Connect()
        {
            connect(_continuousTestsPath);
        }

        private void connect(string continuousTestsPath)
        {
            var currentDir = Environment.CurrentDirectory;
            try
            {
                if (continuousTestsPath != null)
                    Environment.CurrentDirectory = continuousTestsPath;
                if (_handle == null)
                    _client.Start(new StartupParams(_watchToken), this);
                else
                    _client.Start(new StartupParams(_handle.IP, _handle.Port, _handle.Token), this);
            }
            finally
            {
                Environment.CurrentDirectory = currentDir;
            }
        }

        public void Disconnect()
        {
            if (_handle == null)
                _client.Stop();
        }

        public void WaitForConnection()
        {
            while (!_hasConnected)
                Thread.Sleep(10);
        }

        public void RunAll()
        {
            _client.RunAll();
        }

        public void RunPartial(string project)
        {
            RunPartial(new[] { project });
        }

        public void RunPartial(IEnumerable<string> projects)
        {
            _client.RunPartial(projects);
        }

        public void RunTests(TestScope scope)
        {
            _client.StartOnDemandRun(scope.ToInternal());
        }

        public void RunTests(IEnumerable<TestScope> scopes)
        {
            _client.StartOnDemandRun(scopes.Select(x => x.ToInternal()));
        }

        public void VMStarted(VMSpawnedMessage message)
        {
        }

        public void Connecting(int port, bool startPaused)
        {
            lock (_feedbackLock)
            {
                _syncCtx.Post((s) =>
                    {
                        if (EngineConnecting != null)
                            EngineConnecting(this, new EventArgs());
                    }, null);
            }
            ThreadPool.QueueUserWorkItem((state) => { Thread.Sleep(100); _hasConnected = true; });
        }

        public void Disconnecting(int port)
        {
            lock (_feedbackLock)
            {
                _syncCtx.Post((s) =>
                    {
                        if (EngineDisconnecting != null)
                            EngineDisconnecting(this, new EventArgs());
                    }, null);
            }
        }

        public void RunStarted(string text)
        {
            lock (_feedbackLock)
            {
                _syncCtx.Post((s) =>
                    {
                        if (SessionStarted != null)
                            SessionStarted(this, new EventArgs());
                    }, null);
            }
        }

        public void RunFinished(RunFinishedInfo information)
        {
            lock (_feedbackLock)
            {
                _syncCtx.Post((o) =>
                    {
                        var info = (RunFinishedInfo)o;
                        if (SessionFinished != null)
                            SessionFinished(this, SessionFinishedArgs.FromInternal(info));
                    }, information);
            }
        }

        public void InformationMessage(string text)
        {
        }

        public void WarningMessage(string text)
        {
        }

        public void ErrorMessage(string text)
        {
        }

        public void RunInformationMessage(string text)
        {
        }

        public void Consume(object o)
        {
            lock (_feedbackLock)
            {
                _syncCtx.Post((message) =>
                    {
                        if (message.GetType().Equals(typeof(AutoTest.Messages.BuildRunMessage)))
                        {
                            if (BuildFinished == null)
                                return;
                            var msg = (AutoTest.Messages.BuildRunMessage)message;
                            BuildFinished(this, BuildFinishedArgs.FromInternal(msg));
                        }
                        else if (message.GetType().Equals(typeof(AutoTest.Messages.TestRunMessage)))
                        {
                            if (TestsFinished == null)
                                return;
                            var msg = (AutoTest.Messages.TestRunMessage)message;
                            TestsFinished(this, TestsFinishedArgs.FromInternal(msg));
                        }
                        else if (message.GetType().Equals(typeof(AutoTest.Messages.LiveTestStatusMessage)))
                        {
                            if (TestProgress == null)
                                return;
                            var msg = (AutoTest.Messages.LiveTestStatusMessage)message;
                            TestProgress(this, ImmediateTestFeedbackArgs.FromInternal(msg));
                        }
                    }, o);
            }
        }
    }
}
