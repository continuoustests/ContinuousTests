using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }

        public void Write(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

		public void WriteChunk(string message)
		{
			Console.Write(message);
		}
        
		public void WriteChunk(string message, params object[] args)
		{
			Console.Write(message, args);
		}

        public void Write(Exception ex)
        {
            Write(getExceptionInfo(ex));
        }

        private string getExceptionInfo(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.StackTrace);
            if (ex.InnerException != null)
                sb.AppendLine(getExceptionInfo(ex.InnerException));
            return sb.ToString();
        }
    }
}
