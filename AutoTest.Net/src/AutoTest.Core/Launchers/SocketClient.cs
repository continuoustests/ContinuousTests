using System;
using System.Net.Sockets;
using System.Collections;
using System.Text;
using System.IO;
using System.Threading;
using System.Linq;
namespace AutoTest.Core
{
	class IncomingMessageArgs : EventArgs
	{
		public string Message { get; private set; }
		
		public IncomingMessageArgs(string message)
		{
			Message = message;
		}
	}
	
	class SocketClient
	{
		private NetworkStream _stream;
        readonly byte[] _buffer = new byte[1000000];
        private int _currentPort;
        private readonly MemoryStream _readBuffer = new MemoryStream();
        private Queue queue = new Queue();
		private bool IsSending = false;
		
		public string RecievedMessage { get; private set; }
		public bool IsConnected { get; private set; }
		
		public event EventHandler<IncomingMessageArgs> IncomingMessage;
		
		public SocketClient()
		{
			IsConnected = false;
		}

        public void Connect(int port)
        {
            Connect(port, 0);
        }

        private void Connect(int port, int retryCount)
        {
            if (retryCount >= 5)
			{
				IsConnected = false;
                return;
			}
			try {
	            var client = new TcpClient();
	            client.Connect("127.0.0.1", port);
	            _currentPort = port;
	            _stream = client.GetStream();
	            _stream.BeginRead(_buffer, 0, _buffer.Length, ReadCompleted, _stream);
				IsConnected = true;
			} 
			catch 
			{
                Reconnect(retryCount);
			}
        }

        public void Disconnect()
        {
			try {
				IsConnected = false;
	            _stream.Close();
	            _stream.Dispose();
			}
			catch
			{}
        }

        private void Reconnect(int retryCount)
        {
            retryCount++;
            _readBuffer.SetLength(0);
			Disconnect();
			Connect(_currentPort, retryCount);
		}

        private void ReadCompleted(IAsyncResult result)
        {
            var stream = (NetworkStream)result.AsyncState;
            try
            {
                var x = stream.EndRead(result);
                if(x == 0) Reconnect(0);
                for (var i = 0; i < x;i++)
                {
                    if (_buffer[i] == 0)
                    {
                        var data = _readBuffer.ToArray();
                        var actual = Encoding.UTF8.GetString(data, 0, data.Length);
                        if (IncomingMessage != null)
							IncomingMessage(this, new IncomingMessageArgs(actual));
                        _readBuffer.SetLength(0);
                    }
                    else
                    {
                        _readBuffer.WriteByte(_buffer[i]);
                    }
                }
                stream.BeginRead(_buffer, 0, _buffer.Length, ReadCompleted, stream);
            }
            catch (Exception ex)
            {
                WriteError(ex);
                Reconnect(0);
            }
        }


        public void Send(string message)
        {
            if (IsSending)
                throw new Exception("Cannot call send while doing SendAndWait, make up your mind");
            lock (queue)
            {
                queue.Enqueue(message);
                if(!IsSending) {
					SendFromQueue();                      
                }
            }
        }

        public void SendAndWait(string message)
        {
            Send(message);
            IsSending = true;
            var timeout = DateTime.Now;
            while (IsSending && DateTime.Now.Subtract(timeout).TotalMilliseconds < 8000)
                Thread.Sleep(10);
        }

        private void WriteCompleted(IAsyncResult result)
        {
            var client = (NetworkStream)result.AsyncState;
            try
            {
                client.EndWrite(result);
                lock(queue)
                {
		    		IsSending = false;
                    if (queue.Count > 0)
                        SendFromQueue();
                }
                
            }
            catch (Exception ex)
            {
                WriteError(ex);
				Reconnect(0);
            }
        }

        private void SendFromQueue()
        {
            string message = null;
            lock (queue)
            {
                if (!IsSending && queue.Count > 0)
                    message = queue.Dequeue().ToString();
            }
            if (message != null)
            {
                try
                {
					byte[] toSend = Encoding.UTF8.GetBytes(message).Concat(new byte[] { 0x0 }).ToArray();
                    _stream.BeginWrite(toSend, 0, toSend.Length, WriteCompleted, _stream);
                }
                catch
                {
                }
            }
        }

        private void WriteError(Exception ex)
        {
        }
	}
}

