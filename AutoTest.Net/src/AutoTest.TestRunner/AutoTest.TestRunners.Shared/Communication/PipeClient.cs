using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using AutoTest.TestRunners.Shared.Logging;

namespace AutoTest.TestRunners.Shared.Communication
{
    public class PipeClient
    {
        private int _offset = 0;
        private byte[] _bytes = new Byte[500];
        private List<byte> _current = new List<byte>();
        private NamedPipeClientStream _client = null;

        public void Listen(string pipe, Action<string> onRecieve)
        {
            try
            {
                _client = new NamedPipeClientStream(".", pipe, PipeDirection.In);
                _client.Connect(4000);
                while (_client.IsConnected)
                {
                    var str = getString(readMessage(_client));
                    if (str != null)
                        onRecieve(str);
                }
            }
            catch
            {
            }
        }

        public void Listen(string pipe, Action<byte[]> onRecieve)
        {
            try
            {
                _client = new NamedPipeClientStream(".", pipe, PipeDirection.In);
                _client.Connect(4000);
                while (_client.IsConnected)
                {
                    var bytes = readMessage(_client);
                    if (bytes.Length > 0)
                        onRecieve(bytes);
                }
            }
            catch
            {
            }
        }

        public void Disconnect()
        {
            if (_client != null)
            {
                try
                {
                    _client.Close();
                    _client.Dispose();
                }
                catch
                {
                }
            }
        }

        private byte[] readMessage(NamedPipeClientStream client)
        {
            byte[] message = new byte[0];
            _offset = 0;
            _current = new List<byte>();
            while (true)
            {
                var i = client.ReadByte();
                if (i == -1)
                {
                    _offset = 0;
                    return new byte[0];
                }
                if (i == 0)
                {
                    var buffer = new byte[_offset];
                    Array.Copy(_bytes, buffer, _offset);
                    _current.AddRange(buffer);
                    message = _current.ToArray();
                    _current = new List<byte>();
                    _offset = 0;
                    break;
                }
                _bytes[_offset] = Convert.ToByte(i);
                _offset++;
                if (_offset == _bytes.Length)
                {
                    _current.AddRange(_bytes);
                    _offset = 0;
                }
            }
            return message;
        }

        private string getString(byte[] bytes)
        {
            var base64 = "";
            try
            {
                base64 = Encoding.UTF8.GetString(bytes);
                return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " " + base64);
            }
        }
    }
}
