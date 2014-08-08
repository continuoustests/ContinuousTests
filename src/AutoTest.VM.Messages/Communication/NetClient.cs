using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using AutoTest.Messages.Serializers;

namespace AutoTest.VM.Messages.Communication
{
    public class NetClient
    {
        private readonly IClientFeedbackProvider _feedbackProvider;
        private NetworkStream _stream;
        readonly byte[] _buffer = new byte[100000];
        readonly CustomBinaryFormatter _formatter = new CustomBinaryFormatter();
        private string _currentIp;
        private int _currentPort;
        private readonly MemoryStream _readBuffer = new MemoryStream();
        private bool IsSending { get; set; }
        private bool _isConnected = false;
        private Queue queue = new Queue();

        public bool IsConnected { get { return _isConnected; } }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public NetClient(IClientFeedbackProvider feedbackProvider)
        {
            IsSending = false;
            MessageInitializer.RegisterMessagesWithSerializer(_formatter);
            _feedbackProvider = feedbackProvider;
        }

        public void Connect(string ip, int port)
        {
            Connect(ip, port, 0);
        }

        private void Connect(string ip, int port, int retryCount)
        {
            if (retryCount >= 5)
                return;
			try {
            var client = new TcpClient();
            client.Connect(ip, port);
            _currentIp = ip;
            _currentPort = port;
            _stream = client.GetStream();
            _stream.BeginRead(_buffer, 0, _buffer.Length, ReadCompleted, _stream);
            _isConnected = true;
			} 
			catch 
			{
                Reconnect(retryCount);
			}
        }

        public void Disconnect()
        {
			try {
            _stream.Close();
            _stream.Dispose();
			}
			catch
			{}
            _isConnected = false;
        }

        private void Reconnect(int retryCount)
        {
            retryCount++;
            _readBuffer.SetLength(0);
			Disconnect();
			Connect(_currentIp, _currentPort, retryCount);
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
                        var base64 = Encoding.UTF8.GetString(data, 0, data.Length);
                        var actual = Convert.FromBase64String(base64);
                        var o = _formatter.Deserialize(new MemoryStream(actual));
                        ThreadPool.QueueUserWorkItem(HandleMessage, o);
                        _readBuffer.SetLength(0);
                    }
                    else
                    {
                        _readBuffer.WriteByte(_buffer[i]);
                    }
                }
                stream.BeginRead(_buffer, 0, _buffer.Length, ReadCompleted, stream);
            }
            catch //(Exception ex)
            {
                //WriteError(ex);
                Reconnect(0);
            }
        }

        private void HandleMessage(object o)
        {
            try
            {
                if (MessageReceived != null)
                    MessageReceived(this, new MessageReceivedEventArgs(o));
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }
        }

        public void Send(object o)
        {
            if (IsSending)
                throw new Exception("Cannot call send while doing SendAndWait, make up your mind");
            lock (queue)
            {
                queue.Enqueue(o);
                if(!IsSending) {
			        SendFromQueue();                      
                }
            }
        }

        public void SendAndWait(object o)
        {
            Send(o);
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
                    {
                        SendFromQueue();
                    }
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
            object message = null;
            lock (queue)
            {
                if (!IsSending && queue.Count > 0)
                {
                    message = queue.Dequeue();
                }
            }
            if (message != null)
            {
                try
                {
                    var m = new MemoryStream();
                    _formatter.Serialize(m, message);
                    var data = m.ToArray();
                    var encodedData = Convert.ToBase64String(data);
                    var toSend = new byte[encodedData.Length + 1];
                    Encoding.UTF8.GetBytes(encodedData, 0, encodedData.Length, toSend, 0);
                    _stream.BeginWrite(toSend, 0, toSend.Length, WriteCompleted, _stream);
                }
                catch (Exception ex)
                {
                    //let read reconnect
                }
            }
        }

        private void WriteError(Exception ex)
        {
			if (_feedbackProvider == null) return;
            _feedbackProvider.OnError(ex.ToString());
        }
    }
}
