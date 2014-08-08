using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AutoTest.VM.Messages.Communication;
using AutoTest.VM.Messages;

namespace AutoTest.Diagnostics
{
    class DiagnosticNode : IDisposable
    {
        private NetClient _client;
        private List<object> _messages = new List<object>();

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        private Action<object> _reciever = null;

        public string IP { get; private set; }
        public int Port { get; private set; }
        public int ProcessID { get; private set; }
        public string Handle { get; private set; }
        public IEnumerable<object> Messages { get { return _messages; } }

        public bool IsConnected { get { return _client.IsConnected; } }

        public DiagnosticNode(string ip, int port, int processID, string handle)
        {
            initialize(ip, port, processID, handle, null);
        }

        public DiagnosticNode(string ip, int port, int processID, string handle, Action<object> response)
        {
            initialize(ip, port, processID, handle, response);
        }

        private void initialize(string ip, int port, int processID, string handle, Action<object> response)
        {
            IP = ip;
            Port = port;
            ProcessID = processID;
            _client = new NetClient(null);
            _client.MessageReceived += new EventHandler<MessageReceivedEventArgs>(_client_MessageReceived);
            _client.Connect(ip, port);
            if (_client.IsConnected)
                _client.Send(new DiagnosticInstanceMessage());
            Handle = handle;
            _reciever = response;
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        void _client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (MessageReceived != null)
                MessageReceived(this, e);
            if (_reciever != null)
                _reciever(e.Message);
            lock (_messages)
            {
                _messages.Add(e.Message);
            }
        }

        public void Dispose()
        {
            Disconnect();
        }

        public override string ToString()
        {
            var fileName = Path.GetFileName(Handle);
            if (fileName != null)
                return fileName;
            return Handle;
        }
    }
}
