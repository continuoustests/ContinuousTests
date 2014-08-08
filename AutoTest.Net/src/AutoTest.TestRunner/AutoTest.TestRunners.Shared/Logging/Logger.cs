using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared.Logging
{
    public static class Logger
    {
        private static ILogger _instance = new NullLogger();

        public static ILogger Instance { get { return _instance; } }

        public static void SetLogger(ILogger logger)
        {
            _instance = logger;
        }

        public static void Write(string message)
        {
            _instance.Write(message);
        }

        public static void Write(string message, params object[] args)
        {
            _instance.Write(message, args);
        }
		
		public static void WriteChunk(string message)
		{
			_instance.WriteChunk(message);
		}
        
		public static void WriteChunk(string message, params object[] args)
		{
			_instance.WriteChunk(message, args);
		}

        public static void Write(Exception ex)
        {
            _instance.Write(ex);
        }
    }
}
