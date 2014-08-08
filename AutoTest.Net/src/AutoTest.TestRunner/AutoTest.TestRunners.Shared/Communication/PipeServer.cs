using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Pipes;
using System.IO;

namespace AutoTest.TestRunners.Shared.Communication
{
    public class PipeServer : IDisposable
    {
        private NamedPipeServerStream _server = null;
        private List<Thread> _activeThreads = new List<Thread>();
        private bool _exit = false;
        private Stack<string> _unsentMessages = new Stack<string>();
		private bool _isSupported = OS.IsWindows;

        public PipeServer(string name)
        {
			if (!_isSupported)
				return;
            // The blocking synchronous way of using it is used here because there is a bug in mono
            // the PipeOptions enum does not have the correct int values compared to ms .net version
            _server = new NamedPipeServerStream(name, PipeDirection.Out);
            var connect = new Thread(() => _server.WaitForConnection());
            _activeThreads.Add(connect);
            var server = new Thread(run);
            _activeThreads.Add(server);
            connect.Start();
            server.Start();
        }
		
        public void Send(string message)
        {
			if (_isSupported)
            	_unsentMessages.Push(message);
        }

        private void run()
        {
            try
            {
                while (!_exit)
                {
                    while (_server.IsConnected && _unsentMessages.Count > 0)
                        send(_unsentMessages.Pop());
                    Thread.Sleep(15);
                }
                if (_server.IsConnected)
                    _server.Disconnect();
                _server.Dispose();
                _server = null;
            }
            catch
            {
            }
        }

        private void send(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var base64Str = Convert.ToBase64String(bytes);
            var base64 = Encoding.UTF8.GetBytes(base64Str);
            var buffer = new byte[base64.Length + 1];
            Array.Copy(base64, buffer, base64.Length);
            buffer[buffer.Length - 1] = 0;
            _server.Write(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            _exit = true;
            while (_server != null)
                Thread.Sleep(10);
            // Make sure we kill the waiting threads so the app can quit
            _activeThreads.ForEach(x => x.Abort());
        }
    }
}
