using System;
using AutoTest.VM.Messages.Communication;
using AutoTest.Client.Logging;
using System.Diagnostics;
using AutoTest.Client.Handlers;
using AutoTest.VM.Messages;
using System.IO;
using System.Reflection;

namespace AutoTest.Client
{
    class StandaloneVMCreator : IDisposable, IServerFeedbackProvider
    {
        private readonly TcpServer _server;
        private readonly StartupParams _params;

        public event EventHandler<VMSpawnedArgs> VMSpawned;

        public StandaloneVMCreator(StartupParams startup, IStartupHandler handler)
        {
            _params = startup;
            _server = new TcpServer(this);
        }

        public void Create()
        {
            _server.StartServer("127.0.0.1", 0);
            _server.MessageReceived += _server_MessageReceived;
            var process = new Process();
            var installPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath);
            var file = Path.Combine(installPath, "AutoTest.VM.exe");
            if (!File.Exists(file))
            {
                installPath = Environment.CurrentDirectory;
                file = Path.Combine(installPath, "AutoTest.VM.exe");
            }
            var debug = "normal";
            if (File.Exists(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "ENABLE_DEBUG")))
                debug = "debug";
            var pid = Process.GetCurrentProcess().Id;
            var arguments = string.Format("{0} \"{1}\" \"{2}\" \"{3}\" \"{4}\" \"{5}\" \"{6}\"", 0, _params.WatchToken, Guid.NewGuid(), debug, _server.Port, "local", pid);
            if (_params.LocalConfig != null) {
                arguments += string.Format(" \"{0}\"", _params.LocalConfig);
            }
            Logger.Write("Starting vm: " + file + " " + arguments);
            process.StartInfo = new ProcessStartInfo(file, arguments);
            if (debug == "debug")
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            else
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = true;
            if (debug == "debug")
                process.StartInfo.CreateNoWindow = false;
            else
                process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = installPath;
            process.Start();
        }

        public void Dispose()
        {
            _server.MessageReceived -= _server_MessageReceived;
        }

        public void ServerStarted(string ip, int port)
        {
            Logger.Write("Standalone vm creator started");
        }

        public void ClientConnected(string ip, int port, int clientCount)
        {
            Logger.Write("Client conneected to standalone vm creator");
        }

        public void RemovingClient(string ip, int port, string reason, int clientCount)
        {
            Logger.Write("Client disconnected to standalone vm creator");
        }

        public void OnError(string ex)
        {
            Logger.Write(ex);
        }

        void _server_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (VMSpawned == null)
                return;

            if (e.Message.GetType().Equals(typeof(VMInitializedMessage)))
            {
                var message = (VMInitializedMessage)e.Message;
                VMSpawned(this, new VMSpawnedArgs(new VMSpawnedMessage(message.CorrelationId, message.Port, message.NUnitTestRunner, message.MsTestRunner, message.LoggingEnabled, message.StartedPaused)));
            }
        }
    }
}
