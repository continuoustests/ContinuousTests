using System;

namespace AutoTest.VM
{
    class LaunchArguments
    {
        public Guid CorrelationId { get; private set; }
        public int Port { get; private set; }
        public string WatchPath { get; private set; }
        public bool Debug { get; private set; }
        public int OwnerPort { get; private set; }
        public bool IsGlobal { get; private set; }
        public int MasterProcessId { get; private set; }
        public string ConfigPath { get; private set; }

        public LaunchArguments(Guid correlationId, int port, string watchPath, string debug, int ownerPort, string runProfile, int masterProcessId, string configPath)
        {
            CorrelationId = correlationId;
            Port = port;
            WatchPath = watchPath;
            Debug = (debug == "debug");
            OwnerPort = ownerPort;
            IsGlobal = (runProfile == "global");
            MasterProcessId = masterProcessId;
            ConfigPath = configPath;
        }
    }
}
