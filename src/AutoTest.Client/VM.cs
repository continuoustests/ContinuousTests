using System;
using System.Linq;
using System.Collections.Generic;
using AutoTest.VM.Messages.Communication;
using AutoTest.Client.Logging;
using AutoTest.Client.Handlers;
using AutoTest.VM.Messages;
using AutoTest.Messages;
using System.IO;
using AutoTest.Client.Config;
using System.Threading;
using AutoTest.UI;

namespace AutoTest.Client
{
    class SystemMessageArgs : EventArgs
    {
        public IMessage Message { get; private set; }

        public SystemMessageArgs(IMessage message)
        {
            Message = message;
        }
    }

    class MessageArgs : EventArgs
    {
        public IMessage Message { get; private set; }

        public MessageArgs(IMessage message)
        {
            Message = message;
        }
    }

    class VM : IClientFeedbackProvider
    {
        private NetClient _client;
        private int _port;
        private bool _connected = false;
        private IConnectHandlers _connectHandler;
        private IMessageHandlers _messageHandler;
        private string _watchToken;
        private List<QueuedReply> _responses = new List<QueuedReply>();

        public MMConfiguration MMConfiguration { get; private set; }
        public bool IsConnected { get { return _connected; } }

        public event EventHandler<SystemMessageArgs> RecievedSystemMessage;
        public event EventHandler<MessageArgs> RecievedMessage;

        public VM()
        {
            MMConfiguration = new MMConfiguration();
        }

        public void Connect(int port, IConnectHandlers handler, IMessageHandlers messageHandler, bool startPaused, string watchToken)
        {
            _watchToken = watchToken;
            MMConfiguration.Reload(_watchToken);
            _connectHandler = handler;
            _messageHandler = messageHandler;
            _messageHandler.Consume(MMConfiguration);
            _client = new NetClient(this);
            _port = port;
            _client = new NetClient(this);
            _client.MessageReceived += new EventHandler<MessageReceivedEventArgs>(_client_MessageReceived);
            _connectHandler.Connecting(port, startPaused);
            var thread = new Thread(connect);
            thread.Start();
        }

        private void connect()
        {
            var now = DateTime.Now;
            while (DateTime.Now.Subtract(now) < new TimeSpan(0, 0, 60))
            {
                try
                {
                    _client.Connect("127.0.0.1", _port);
                    _connected = true;
                    TestProfilerCorrupted();
                    break;
                }
                catch
                {
                    System.Threading.Thread.Sleep(500);
                }
            }
        }

        public void Disconnect()
        {
            if (isConnected())
            {
                _client.SendAndWait(new TerminateMessage());
                _client.Disconnect();
            }
            if (_connectHandler != null)
                _connectHandler.Disconnecting(_port);
            _connected = false;
        }

        private bool isConnected()
        {
            if (_client == null)
                return false;
            return _client.IsConnected;
        }

        public void PauseEngine()
        {
            if (isConnected())
                _client.Send(new PauseVMMessage());
            _connected = false;
        }

        public void ResumeEngine()
        {
            if (isConnected())
                _client.Send(new ResumeVMMessage());
            _connected = true;
        }

        public void RunAll()
        {
            try
            {
                if (isConnected())
                    _client.Send(new ForceFullRunMessage());
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public void RunPartial(IEnumerable<string> projects)
        {
            try
            {
                if (isConnected())
                    _client.Send(new PartialRunMessage(projects));
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public void RunRecursiveRunDetection()
        {
            if (isConnected())
                _client.Send(new RunRecursiveRunDetectorMessage());
        }

        public void SendConfigurationUpdate(ConfigurationUpdateMessage message)
        {
            try
            {
                MMConfiguration.Reload(_watchToken);
                if (isConnected())
                    _client.Send(message);

                // Publish configuration to handlers (See feedback window)
                _messageHandler.Consume(MMConfiguration);

                if (!message.RequiresRestart)
                    return;
                
                setConnectedFromConfiguration(message.LocalFile);
                notifyAboutConnectedState();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public string GetAssemblyFromProject(string project)
        {
            var response = requestFromServer<AssemblyPathRequest, AssemblyPathResponse>(new AssemblyPathRequest(project));
            if (response == null)
                return null;
            return response.Assembly;
        }

        public bool IsSolutionInitialized()
        {
            if (!isConnected())
                return true;
            var response = requestFromServer<IsSolutionInitializedRequest, IsSolutionInitializedResponse>(new IsSolutionInitializedRequest());
            if (response == null)
                return true;
            return response.IsInitialized;
        }

        private TReply requestFromServer<TRequest, TReply>(TRequest message)
            where TRequest : ICustomBinarySerializable, new()
            where TReply : ReplyMessage, ICustomBinarySerializable, new()
        {
            var request = new RequestMessage<TRequest, TReply>(message);
            _client.Send(request);
            var sentAt = DateTime.Now;
            while (DateTime.Now < sentAt.AddSeconds(10))
            {
                lock (_responses)
                {
                    var response = _responses.FirstOrDefault(x => ((ReplyMessage)x.Message).CorrelationID.Equals(request.CorrelationID));
                    if (response != null)
                    {
                        var reply = (TReply)response.Message;
                        _responses.Remove(response);
                        return reply;
                    }
                    _responses.RemoveAll(x => x.QueuedAt.AddSeconds(11) < DateTime.Now);
                }
                Thread.Sleep(20);
            }
            return null;
        }

        private void notifyAboutConnectedState()
        {
            if (_connected)
                _connectHandler.Connecting(0, !_connected);
            else
                _connectHandler.Disconnecting(0);
        }

        private void setConnectedFromConfiguration(string path)
        {
            var reader = new AutoTest.Client.Config.ConfigParser(path);
            var localConfig = reader.ParseLocal();
            var globalConfig = reader.ParseGlobal();

            if (localConfig.StartPaused.Exists)
                _connected = !localConfig.StartPaused.Item;
            else
                _connected = !(globalConfig.StartPaused.Exists && globalConfig.StartPaused.Item);
        }

        public void GetCouplingGraph(string name)
        {
            if (!isConnected()) return;
            Logger.Write("sending get graph message");
            _client.Send(new RequestVisualGraphMessage(name));
        }

        public void GetProfiledGraph(string name)
        {
            if (!isConnected()) return;
            Logger.Write("sending get profiled graph message");
            _client.Send(new RequestProfiledGraphMessage(name));
        }

        public void GetRuntimeTestInformation(string methodName)
        {

            if (!isConnected()) return;
            Logger.Write("sending get runtime test information message");
            _client.Send(new RequestRuntimeTestInformationMessage(methodName));
        }

        public void RunRelatedTestsFor(string member)
        {
            if (isConnected())
                _client.Send(new RunRelatedTestsMessage(member));
        }

        public void StartOnDemandRun(OnDemandRun run)
        {
            StartOnDemandRun(new OnDemandRun[] { run });
        }

        public void StartOnDemandRun(IEnumerable<OnDemandRun> runs)
        {
            var message = new OnDemandTestRunMessage();
            foreach (var run in runs)
                message.AddRun(run);
            if (isConnected())
                _client.Send(message);
        }

        public void RequestManualMinimize()
        {
            if (isConnected())
                _client.Send(new ManualMinimizationRequestMessage());
        }


        public void GetLastAffectedGraph()
        {
            if(isConnected()) 
                _client.Send(new GetLastAffectedGraphMessage());
        }

        public void SetCustomOutputpath(string path)
        {
            if (isConnected())
                _client.Send(new UpdateCustomOutputpathMessage(path));
        }

        public void GoTo(string file, int line, int column)
        {
            if (isConnected())
                _client.Send(new GoToFileAndLineMessage(file, line, column));
        }

        public void FocusEditor()
        {
            if (isConnected())
                _client.Send(new FocusEditorMessage());
        }

        public void QueueRealtimeChange(RealtimeChangeList list)
        {
            if (isConnected())
                _client.Send(list);
        }

        void _client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var message = e.Message;
            if (message.GetType().Equals(typeof(AbortMessage)))
                raiseMessage((AbortMessage)message);

            if (typeof(ReplyMessage).IsAssignableFrom(message.GetType()))
                _responses.Add(new QueuedReply(message));

            // Always publish raw messages too
            _messageHandler.Consume(message);
        }

        private void raiseMessage(AbortMessage abortMessage)
        {
            if (RecievedMessage != null)
                RecievedMessage(this, new MessageArgs(abortMessage));
        }

        private void raiseSystemMessageEvent(IMessage message)
        {
            if (RecievedSystemMessage != null)
                RecievedSystemMessage(this, new SystemMessageArgs(message));
        }

        public void OnError(string ex)
        {
            Logger.Write(ex);
        }

        public IEnumerable<string> GetProjectBuildList(IEnumerable<string> projects)
        {
            if (!isConnected())
                return new string[] {};
            var response = requestFromServer<OrderedBuildList, OrderedBuildList>(new OrderedBuildList(projects));
            if (response == null)
                return new string[] {};
            return response.Projects;
        }

        public void TestProfilerCorrupted()
        {
            if(isConnected())
            {
                _client.Send(new TestProfilerCorruptedMessage());
            }
        }

        public void AbortRun()
        {
            if (isConnected())
                _client.Send(new AbortMessage(""));
        }
    }

    class QueuedReply
    {
        public DateTime QueuedAt { get; private set; }
        public object Message { get; private set; }

        public QueuedReply(object message)
        {
            QueuedAt = DateTime.Now;
            Message = message;
        }
    }
}
