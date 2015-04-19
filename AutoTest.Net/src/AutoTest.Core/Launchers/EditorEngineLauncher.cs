using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using AutoTest.Core.Configuration;
using Debug = AutoTest.Core.DebugLog.Debug;
using AutoTest.Core.Messaging;
using AutoTest.CoreExtensions;
using AutoTest.Messages;

namespace AutoTest.Core.Launchers
{
	public class EditorEngineLauncher :
		IConsumerOf<BuildRunMessage>,
		IConsumerOf<TestRunMessage>,
		IConsumerOf<RunStartedMessage>,
		IConsumerOf<RunFinishedMessage>
	{
		private IMessageBus _bus;		
		private string _path = null;
		private SocketClient _client = null;
		private SocketClient _eventendpoint = null;
        private Thread _shutdownWhenDisconnected = null;
        private bool _eventPointShutdownTriggered = false;
		
		public EditorEngineLauncher(IMessageBus bus)
		{
			_bus = bus;
		}
		
		public void Connect(string path)
		{
			_path = path;
			if (_client != null &&_client.IsConnected)
				_client.Disconnect();
			_client = null;
			isConnected();
			connectToEventEndpoint();
		}
		
		public void GoTo(string file, int line, int column)
		{
			if (!isConnected())
				return;
			send(string.Format("goto {0}|{1}|{2}", file, line, column));
		}

		public void Focus()
		{
			if (!isConnected())
				return;
			send("setfocus");
		}

		public void Consume(BuildRunMessage message) {
			if (!isConnected()) return;
			var state = "succeeded";
			if (message.Results.ErrorCount > 0)
				state = "failed";
			send(
				string.Format("autotest.net build \"{0}\" {1}",
					message.Results.Project,
					state));
		}
		
		public void Consume(TestRunMessage message) {
			if (!isConnected()) return;
			var state = "succeeded";
			if (message.Results.Failed.Length > 0)
				state = "failed";
			send(
				string.Format("autotest.net testrun \"{0}\" {1} {2}",
					message.Results.Assembly,
					message.Results.Runner.ToString(),
					state));
		}
		
		public void Consume(RunStartedMessage message) {
			if (!isConnected()) return;
			send("autotest.net runstarted");
		}
		
		public void Consume(RunFinishedMessage message) {
			if (!isConnected()) return;
			send(
				string.Format("autotest.net runfinished {0} {1}",
					message.Report.NumberOfBuildsFailed,
					message.Report.NumberOfTestsFailed));
		}
		
        private object _clientLock = new object();
		private bool isConnected()
		{
			try
			{
				if (_client != null && _client.IsConnected)
					return true;
                var instance = new InstanceLocator("editor").GetInstance(_path);
				if (instance == null)
					return false;
				_client = new SocketClient();
				_client.IncomingMessage += Handle_clientIncomingMessage;
				_client.Connect(instance.Port);
				if (_client.IsConnected) {
					startBackgroundShutdownHandler();
					return true;
				}
				_client = null;
				return false;
			}
			catch (Exception ex)
			{
				Debug.WriteError(ex.ToString());
				return false;
            }
		}

        
		private void connectToEventEndpoint()
		{
			try
			{
				if (_eventendpoint != null && _eventendpoint.IsConnected)
					return;
                var instance = new InstanceLocator("event").GetInstance(_path);
				if (instance == null) {
					return;
				}
				_eventendpoint = new SocketClient();
				_eventendpoint.IncomingMessage += Handle_eventIncomingMessage;
				_eventendpoint.Connect(instance.Port);
				if (_eventendpoint.IsConnected) {
					startBackgroundShutdownHandler();
					return;
				}
				_bus.Publish(new ExternalCommandMessage("EditorEngine", "shutdown"));
			}
			catch (Exception ex)
			{
				Debug.WriteError(ex.ToString());
			}
		}

		private void startBackgroundShutdownHandler()
		{
			if (_shutdownWhenDisconnected != null)
				return;
			_shutdownWhenDisconnected = new Thread(exitWhenDisconnected);
			_shutdownWhenDisconnected.Start();
		}

		private void exitWhenDisconnected()
		{
			while (isConnected() && !_eventPointShutdownTriggered) {
				Thread.Sleep(200);
			}
			_bus.Publish(new ExternalCommandMessage("EditorEngine", "shutdown"));
		}

		void Handle_clientIncomingMessage(object sender, IncomingMessageArgs e)
		{
			Debug.WriteDebug("Dispatching editor message: " + e.Message);
			_bus.Publish(new ExternalCommandMessage("EditorEngine", e.Message));
		}

		void Handle_eventIncomingMessage(object sender, IncomingMessageArgs e)
		{
			if (e.Message == "shutdown")
				_eventPointShutdownTriggered = true;
		}

		private void send(string message)
		{
			Debug.WriteDebug("Sending to editor engine: " + message);
			_client.Send(message);
		}
	}
	
	class InstanceLocator
	{
        private string _instanceType;

        public InstanceLocator(string instanceType)
		{
			_instanceType = instanceType;
		}

		public Instance GetInstance(string path)
		{
	        var process = new Process();
            string[] errors;
            var output = process.QueryAll("oi", "codemodel get-token-"+_instanceType+"-endpoint \""+path+"\"", false, path, out errors).ToArray();
            if (output.Length == 2 && output[0].Contains(":")) {
                int port;
                if (int.TryParse(output[0].Split(new char[] {':'})[1], out port))
                    return new Instance("", 0, path, port);
            }
            return null;
		}
	}

	class FS
	{
		public static string GetTempFilePath()
		{
			var tmpfile = Path.GetTempFileName();
			if (OS.IsOSX)
				tmpfile = Path.Combine("/tmp", Path.GetFileName(tmpfile));
			return tmpfile;
		}

		public static string GetTempDir()
		{
			if (OS.IsOSX)
				return "/tmp";
			return Path.GetTempPath();
		}
	}

	static class OS
    {
        private static bool? _isWindows;
        private static bool? _isUnix;
        private static bool? _isOSX;

        public static bool IsWindows {
            get {
                if (_isWindows == null) {
                    _isWindows = 
                        Environment.OSVersion.Platform == PlatformID.Win32S ||
                        Environment.OSVersion.Platform == PlatformID.Win32NT ||
                        Environment.OSVersion.Platform == PlatformID.Win32Windows ||
                        Environment.OSVersion.Platform == PlatformID.WinCE ||
                        Environment.OSVersion.Platform == PlatformID.Xbox;
                }
                return (bool) _isWindows;
            }
        }

        public static bool IsPosix {
            get {
                return IsUnix || IsOSX;
            }
        }

        public static bool IsUnix {
            get {
                if (_isUnix == null)
                    setUnixAndLinux();
                return (bool) _isUnix;
            }
        }

        public static bool IsOSX {
            get {
                if (_isOSX == null)
                    setUnixAndLinux();
                return (bool) _isOSX;
            }
        }

        private static void setUnixAndLinux()
        {
            try
            {
                if (IsWindows) {
                    _isOSX = false;
                    _isUnix = false;
                } else  {
                    var process = new System.Diagnostics.Process
                                      {
                                          StartInfo =
                                              new System.Diagnostics.ProcessStartInfo("uname", "-a")
                                                  {
                                                      RedirectStandardOutput = true,
                                                      WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                                                      UseShellExecute = false,
                                                      CreateNoWindow = true
                                                  }
                                      };

                    process.Start();
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    _isOSX = output.Contains("Darwin Kernel");
                    _isUnix = !_isOSX;
                }
            }
            catch
            {
                _isOSX = false;
                _isUnix = false;
            }
        }
    }
}

