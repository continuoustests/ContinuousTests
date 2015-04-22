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
        public string LocalConfig { get; private set; }

        public StartupParams(string watchToken)
        {
            IP = null;
            Port = 0;
            WatchToken = watchToken;
            LocalConfig = "";
        }

        public StartupParams(string watchToken, string localConfig)
        {
            IP = null;
            Port = 0;
            WatchToken = watchToken;
            LocalConfig = localConfig;
        }

        public StartupParams(string ip, int port, string watchToken)
        {
            IP = ip;
            Port = port;
            WatchToken = watchToken;
            LocalConfig = "";
        } 
    }
}
