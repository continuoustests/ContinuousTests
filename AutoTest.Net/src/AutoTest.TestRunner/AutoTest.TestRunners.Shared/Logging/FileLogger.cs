using System;
using System.IO;
using System.Text;

namespace AutoTest.TestRunners.Shared.Logging
{
	public class FileLogger : ILogger
	{
		private object _lock = new object();
		private bool _outputEnabled;
		private bool _debugEnabled;
		private string _file;

		public FileLogger(bool outputEnabled, string logFile) {
			_outputEnabled = outputEnabled;
			_file = logFile;
			_debugEnabled = _outputEnabled && _file != null;
		}

		public void Write(string message) {
			if (!_outputEnabled) return;
            write(message);
        }

        public void Write(string message, params object[] args) {
			if (!_outputEnabled) return;
            write(message, args);
        }

		public void WriteChunk(string message) {
			if (!_outputEnabled) return;
			write(message);
		}
        
		public void WriteChunk(string message, params object[] args) {
			if (!_outputEnabled) return;
			write(message, args);
		}

        public void Debug(string message) {
			if (!_debugEnabled) return;
            write(message);
        }

        public void Debug(string message, params object[] args) {
			if (!_debugEnabled) return;
            write(message, args);
        }

		public void DebugChunk(string message) {
			if (!_debugEnabled) return;
			write(message);
		}
        
		public void DebugChunk(string message, params object[] args) {
			if (!_debugEnabled) return;
			write(message, args);
		}

        public void Debug(Exception ex) {
			if (!_debugEnabled) return;
            Debug(getExceptionInfo(ex));
        }

        public void Write(Exception ex) {
            if (!_debugEnabled) return;
            Debug(getExceptionInfo(ex));
        }

        private string getExceptionInfo(Exception ex) {
            var sb = new StringBuilder();
            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.StackTrace);
            if (ex.InnerException != null)
                sb.AppendLine(getExceptionInfo(ex.InnerException));
            return sb.ToString();
        }

		private void write(string message, params object[] args)
		{
			write(string.Format(message, args));
		}

		private void write(string message)
		{
			lock (_lock)
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        using (var writer = new StreamWriter(_file, true))
                        {
                            writer.WriteLine(message);
                            break;
                        }
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                }
            }
		}
    }
}