using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using AutoTest.Messages.Serializers;
using AutoTest.Messages;

namespace AutoTest.VM.Messages.Communication
{
    public class TcpServer : ITcpServer, IMessageForwarder
    {
        private string _currentIp;
        private int _currentPort;
        private Socket _listener;
        private readonly List<ConnectedClient> _clients = new List<ConnectedClient>();
        private readonly CustomBinaryFormatter _formatter;
        
        private readonly IServerFeedbackProvider _feedbackProvider;

        
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public int Port { get { return _currentPort; } }
        public int ClientCount
        { 
            get
            {
                int count = 0;
                foreach (var client in _clients)
                {
                    if (client.Stream == null)
                        continue;
                    count++;
                }
                return count;
            }
        }

        public TcpServer(IServerFeedbackProvider feedbackProvider)
        {
            _formatter = new CustomBinaryFormatter();
            MessageInitializer.RegisterMessagesWithSerializer(_formatter);
            _feedbackProvider = feedbackProvider;
        }

        public void StartServer(string ip, int port)
        {
            try
            {
                _currentIp = ip;
                var localAddress = IPAddress.Parse(_currentIp);
                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var ipEndpoint = new IPEndPoint(localAddress, port);
                _listener.Bind(ipEndpoint);
                _currentPort = ((IPEndPoint)_listener.LocalEndPoint).Port;
                _listener.Listen(1);
                _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);
				if (_feedbackProvider != null)
					_feedbackProvider.ServerStarted(_currentIp, _currentPort);
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }
        }

        public void Stop()
        {
            if (_listener == null)
                return;
            RemoveClients();
            _listener.Close();
        }

        private void RemoveClients()
        {
            lock (_clients)
            {
                var clientCount = _clients.Count;
                for (var i = 0; i < clientCount; i++)
                {
                    RemoveClient(_clients[0].Stream);
                    notifyProviderAboutDisconnect("server is shutting down");
                }
            }
        }

        public void Send(object o)
        {
            var m = new MemoryStream();

            lock (_clients)
            {
                _formatter.Serialize(m, o); 
                byte[] data = m.ToArray();
                SendToClients(data, false);
            }
        }

        public void Forward(object message)
        {
            Send(message);
        }

        private void SendToClients(byte[] data, bool onlyDiagnostic)
        {
            var encodedData = Convert.ToBase64String(data);
            var toSend = new byte[encodedData.Length + 1];
            Encoding.UTF8.GetBytes(encodedData, 0, encodedData.Length, toSend, 0);
            foreach (var client in _clients)
            {
                try
                {
                    var stream = client.Stream;
                    if (!onlyDiagnostic || client.IsDiagnostic)
                        client.Stream.BeginWrite(toSend, 0, toSend.Length, WriteCompleted, stream);
                }
                catch (Exception ex)
                {
                    RemoveClient(client.Stream);
                    notifyProviderAboutDisconnect(string.Format("SendToClients failed ({0})", ex.ToString()));
                }
            }
        }

        private void ForwardToDiagnosticClients(object o)
        {
            var m = new MemoryStream();
            lock (_clients)
            {
                _formatter.Serialize(m, o);
                byte[] data = m.ToArray();
                SendToClients(data, true);
            }
        }

        private void RemoveClient(NetworkStream client)
        {
            try
            {
                lock (_clients)
                {
                    _clients.RemoveAll(c => c.Stream.Equals(client));
                }
                client.Close();

            }
            catch (Exception ex)
            {
                WriteError(ex);
            }
        }

        private void notifyProviderAboutDisconnect(string reason)
        {
            if (_feedbackProvider == null)
                return;
            _feedbackProvider.RemovingClient(_currentIp, _currentPort, reason, ClientCount);
        }

        private void WriteCompleted(IAsyncResult result)
        {
            var client = (NetworkStream) result.AsyncState;
            try
            {
                client.EndWrite(result);
            }
            catch (Exception ex)
            {
                RemoveClient(client);
                notifyProviderAboutDisconnect(string.Format("WriteCompleted failed ({0})", ex.ToString()));
            }
        }

        private void ReadCompleted(IAsyncResult result)
        {
            var client = (ConnectedClient) result.AsyncState;
            try
            {
                var x = client.Stream.EndRead(result);
                if (x == 0) RemoveClient(client.Stream);
                for (int i = 0; i < x;i++)
                {
                    if (client.Buffer[i] == 0)
                    {
                        byte[] data = client.ReadBuffer.ToArray();
                        var base64 = Encoding.UTF8.GetString(data, 0, data.Length);
                        var actual = Convert.FromBase64String(base64);
                        object o;
                        lock (_clients)
                        {
                            o = _formatter.Deserialize(new MemoryStream(actual));
                        }
                        if (o.GetType().Equals(typeof(DiagnosticInstanceMessage)))
                            MoveToDiagnostic(client.Stream);
                        ThreadPool.QueueUserWorkItem(HandleMessage, o);
                        client.ReadBuffer.SetLength(0);
                    }
                    else
                    {
                        client.ReadBuffer.WriteByte(client.Buffer[i]);
                    }
                }
                client.Stream.BeginRead(client.Buffer, 0, client.Buffer.Length, ReadCompleted, client);
            }
            catch (Exception ex)
            {
                RemoveClient(client.Stream);
                notifyProviderAboutDisconnect(string.Format("ReadCompleted failed ({0})", ex));
            }
        }

        private void MoveToDiagnostic(NetworkStream stream)
        {
            lock (_clients)
            {
                var client = _clients.Find(c => c.Stream.Equals(stream));
                if (client == null)
                    return;
                client.SetAsDiagnostic();
            }
        }

        private void HandleMessage(object o)
        {
            try
            {
                if (MessageReceived != null)
                    MessageReceived(this, new MessageReceivedEventArgs(o));
                ForwardToDiagnosticClients(o);
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }
        }

        private void AcceptCallback(IAsyncResult result)
        {
            var listener = (Socket)result.AsyncState;
            try
            {
                var client = listener.EndAccept(result);
                var clientStream = new NetworkStream(client);
                ConnectedClient c;
                lock (_clients)
                {
                    c = new ConnectedClient(clientStream, new byte[5000], new MemoryStream());
                    _clients.Add(c);
                }
                clientStream.BeginRead(c.Buffer, 0, c.Buffer.Length, ReadCompleted, c);
                if (_feedbackProvider != null)
                    _feedbackProvider.ClientConnected(_currentIp, _currentPort, ClientCount);
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }
            finally
            {
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
            }
        }

        private void WriteError(Exception ex)
        {
			if (_feedbackProvider == null) return;
            _feedbackProvider.OnError(ex.ToString());
        }
    }
}
