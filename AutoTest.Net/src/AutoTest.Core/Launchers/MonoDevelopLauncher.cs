using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Reflection;
using System.Globalization;
using AutoTest.Core.DebugLog;
namespace AutoTest.Core.Launchers
{
	class MonoDevelopLauncher
	{
		public bool Launch(string file, int lineNumber, int column)
		{
			EndPoint endPoint = null;
			Socket listener = null;
			try
			{
				if (OS.IsPosix)
				{
					endPoint = getUnixEndPoint();
					listener = getUnixListener();
				}
				else
				{
					endPoint = getEndPoint();
					listener = getListener();
				}
				listener.Connect(endPoint);
				listener.Send(Encoding.UTF8.GetBytes(string.Format("{0};{1};{2}\n", file, lineNumber, column)));
				listener.Disconnect(false);
				return true;
			}
			catch (Exception exception)
			{
				Debug.WriteException(exception);
				if (listener == null)
					return false;
				if (listener.Connected)
					listener.Disconnect(false);
				return false;
			}
		}
		
		private EndPoint getUnixEndPoint()
		{
			var socket_filename = "/tmp/md-" + Environment.GetEnvironmentVariable ("USER") + "-socket";
			var assembly = Assembly.Load("Mono.Posix, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
			return (EndPoint) assembly.CreateInstance("Mono.Unix.UnixEndPoint", false, BindingFlags.CreateInstance, null, new object[] { socket_filename }, CultureInfo.InvariantCulture, null);
		}
		
		private Socket getUnixListener()
		{
			return new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
		}
		
		private EndPoint getEndPoint()
		{
			return new IPEndPoint(IPAddress.Loopback, 40000 + hashSDBMBounded(Environment.UserName));
		}
		
		private Socket getListener()
		{
			return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
		}
		
		public int hashSDBMBounded(string input)
		{
			ulong hash = 0;

			try {
				foreach (char c in input) {
					unchecked {
						hash = (ulong)char.GetNumericValue(c) + (hash << 6) + (hash << 16) - hash;
					}
				}
			} catch {
				// If we overflow, return the intermediate result
			}
				
			return (int)(hash % 1000);
		}	
	}
}

