using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System;
using System.Threading;

namespace AutoTest.VM.Messages.Communication
{
    public class VMHandle
    {
        public string File { get; private set; }
        public int ProcessID { get; private set; }
        public string IP { get; private set; }
        public int Port { get; private set; }
        public string Token { get; private set; }

        public VMHandle(string file, int processID, string ip, int port, string token)
        {
            File = file;
            ProcessID = processID;
            IP = ip;
            Port = port;
            Token = token;
        }
    }

    public class VMConnectHandle
    {
        private string _handle = null;

        public VMHandle GetCurrent()
        {
            var handle = getFilename();
            if (!File.Exists(handle))
                return null;
            try
            {
                return tokenFromFile(handle);
            }
            catch
            {
                return null;
            }
        }

        public VMHandle[] GetAll()
        {
            return getAllHandles().Select(x => tokenFromFile(x)).Where(x => x != null).ToArray();
        }

        public VMHandle[] GetFromToken(string token)
        {
            return GetFromToken(token, false);
        }

        public VMHandle[] GetFromToken(string token, bool caseInsensitive)
        {
            var tokens = new List<VMHandle>();
            var files = getAllHandles();
            foreach (var file in files)
            {
                try
                {
                    var handle = tokenFromFile(file);
                    if (handle == null)
                        continue;
                    if (caseInsensitive)
                    {
                        if (handle.Token.ToLower().Equals(token.ToLower()))
                            tokens.Add(handle);
                    }
                    else
                    {
                        if (handle.Token.Equals(token))
                            tokens.Add(handle);
                    }
                }
                catch
                {
                    try
                    {
                        if (File.Exists(file))
                            File.Delete(file);
                    }
                    catch
                    {
                        Thread.Sleep(200);
                        throw;
                    }
                }
            }
            return tokens.ToArray();
        }

        public void Write(string ip, int port, string watchToken)
        {
            _handle = getFilename();
            File.WriteAllText(_handle, string.Format("{0}:{1}{2}{3}", ip, port, Environment.NewLine, watchToken));
        }

        public void Remove()
        {
            if (_handle == null)
                throw new Exception("Cannot remove handle, handle was not set");
            if (File.Exists(_handle))
                File.Delete(_handle);
            _handle = null;
        }

        private VMHandle tokenFromFile(string handle)
        {
            try
            {
                var lines = File.ReadAllLines(handle);
                var chunks = lines[0].Split(new char[] { ':' });
                var filename = System.IO.Path.GetFileName(handle);
                return new VMHandle(handle, int.Parse(filename.Substring("mm_vm_".Length, filename.Length - "mm_vm_".Length)), chunks[0], int.Parse(chunks[1]), lines[1]);
            }
            catch
            {
                return null;
            }
        }

        private string[] getAllHandles()
        {
            return Directory.GetFiles(Path.GetTempPath(), "mm_vm_*");
        }

        private string getFilename()
        {
            var pid = Process.GetCurrentProcess().Id;
            return Path.Combine(Path.GetTempPath(), string.Format("mm_vm_{0}", pid));
        }
    }
}