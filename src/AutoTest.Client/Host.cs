using System;
using AutoTest.Client.GraphGenerators;
using AutoTest.VM.Messages.Communication;
using AutoTest.Client.Logging;
using AutoTest.VM.Messages;
using AutoTest.Client.Handlers;
using AutoTest.Messages;
using System.Threading;

namespace AutoTest.Client
{
    class VMSpawnedArgs : EventArgs
    {
        public VMSpawnedMessage Message;

        public VMSpawnedArgs(VMSpawnedMessage message)
        {
            Message = message;
        }
    }

    class Host : IClientFeedbackProvider
    {
        private const int _port = 9070;
        private NetClient _host;
        private ISpawnHandler _spawnHandler;
        private Guid _spawnCorrelationID;

        public event EventHandler<VMSpawnedArgs> VMSpawned;

        public bool SpawnVM(StartupParams startup, ISpawnHandler spawnHandler)
        {
            try
            {
                _spawnHandler = spawnHandler;
                _spawnCorrelationID = Guid.NewGuid();
                Logger.Write(string.Format("Sending VMSpawnMessage with {0} and {1}", _spawnCorrelationID, startup.WatchToken));
                var thread = new Thread(send);
                thread.Start(new VMSpawnMeessage(_spawnCorrelationID, 0, startup.WatchToken));
                var timeout = DateTime.Now.AddSeconds(1);
                while (DateTime.Now < timeout)
                {
                    Thread.Sleep(10);
                    if (_host == null)
                        continue;
                    if (thread.ThreadState == ThreadState.Stopped)
                        break;
                }
                thread.Abort();
                if (_host == null)
                    return false;
                return _host.IsConnected;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        private void send(object message)
        {
            var msg = (IMessage)message;
            Thread.Sleep(400);
            send(msg);
        }

        public void UpdateConfiguration(ConfigurationUpdateMessage message)
        {
            trySend(message);
        }

        public void OnError(string ex)
        {
            Logger.Write(ex);
        }

        public void RegisterLiccense(string license)
        {
            trySend(new RegisterLicenseMessage(license));
        }

        private void trySend(IMessage message)
        {
            try
            {
                send(message);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private void send(IMessage message)
        {
            if (_host == null)
            {
                _host = new NetClient(this);
                Logger.Write(string.Format("Connectiong to 127.0.0.1:{0}", _port));
                _host.Connect("127.0.0.1", _port);
                _host.MessageReceived += _spawnClient_MessageReceived;
            }
            _host.Send(message);
        }

        private void _spawnClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Logger.Write(string.Format("Recieving message of type {0} from host", e.Message.GetType()));
            if (e.Message.GetType().Equals(typeof(VMSpawnedMessage)))
            {
                Logger.Write("Recieved VMSpawnedMessage");
                var message = (VMSpawnedMessage)e.Message;
                if (!message.CorrelationId.Equals(_spawnCorrelationID))
                    return;
                Logger.Write("Message has correct correlationID");
                _host.Disconnect();
                _host.MessageReceived -= _spawnClient_MessageReceived;
                _host = null;
                Logger.Write(string.Format("Disconnected from 127.0.0.1:{0}", _port));
                if (VMSpawned != null)
                    VMSpawned(this, new VMSpawnedArgs(message));
                _spawnHandler.VMStarted(message);
            }
            if (e.Message.GetType().Equals(typeof(ValidLicenseMessage)))
            {
                //throw new NotImplementedException(); WTF was this throwing unimpl exception?!
            }
        }
    }
}
