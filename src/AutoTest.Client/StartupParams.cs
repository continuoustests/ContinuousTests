using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Client
{
    public class StartupParams
    {
        public string IP { get; private set; }
        public int Port { get; private set; }
        public string WatchToken { get; private set; }

        public StartupParams(string watchToken)
        {
            IP = null;
            Port = 0;
            WatchToken = watchToken;
        }

        public StartupParams(string ip, int port, string watchToken)
        {
            IP = ip;
            Port = port;
            WatchToken = watchToken;
        }
    }
}
