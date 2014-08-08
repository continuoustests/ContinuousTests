using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Results;
using System.IO;

namespace AutoTest.TestRunners.Shared.Errors
{
    public class ErrorHandler
    {
        public static TestResult GetError(Exception ex)
        {
            var result = GetError(getMessage(ex));
            result.AddStackLines(getStackLines(ex));
            return result;
        }

        public static TestResult GetError(string message)
        {
            return GetError("", message);
        }

        public static TestResult GetError(string runner, Exception ex)
        {
            var result = GetError(runner, getMessage(ex));
            result.AddStackLines(getStackLines(ex));
            return result;
        }

        public static TestResult GetError(string runner, string message)
        {
            return new TestResult(runner, runner, "", 0, "AutoTest.TestRunner.exe internal error", TestState.Panic, message);
        }

        private static string getMessage(Exception exception)
        {
            if (exception == null)
                return "";
            if (exception.GetType().Equals(typeof(FileLoadException)))
            {
                var ex = (FileLoadException)exception;
                return ex.Message + Environment.NewLine + ex.FileName + Environment.NewLine + ex.FusionLog + getMessage(ex.InnerException);
            }
            if (exception.GetType().Equals(typeof(FileNotFoundException)))
            {
                var ex = (FileNotFoundException)exception;
                return ex.Message + Environment.NewLine + ex.FileName + Environment.NewLine + ex.FusionLog + getMessage(ex.InnerException);
            }
            if (exception.GetType().Equals(typeof(BadImageFormatException)))
            {
                var ex = (BadImageFormatException)exception;
                return ex.Message + Environment.NewLine + ex.FileName + Environment.NewLine + ex.FusionLog + getMessage(ex.InnerException);
            }
            return exception.Message + Environment.NewLine + getMessage(exception.InnerException);
        }

        private static StackLine[] getStackLines(Exception ex)
        {
            if (ex == null)
                return new StackLine[] { };
            var stackLines = new List<StackLine>();
            if (ex.InnerException != null)
                stackLines.AddRange(getStackLines(ex.InnerException));
            stackLines.AddRange(ex.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(x => new StackLine(x)));
            return stackLines.ToArray();
        }
    }
}
