using System;
using System.Collections.Generic;
using Xunit;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners.XUnit
{
    class XUnitLogger : IRunnerLogger
    {
        private readonly List<Shared.Results.TestResult> _results = new List<Shared.Results.TestResult>();
        private string _currentAssembly = null;
        private readonly ITestFeedbackProvider _channel = null;

        public XUnitLogger(ITestFeedbackProvider channel)
        {
            _channel = channel;
        }

        public IEnumerable<Shared.Results.TestResult> Results { get { return _results; } }

        public void SetCurrentAssembly(string assembly)
        {
            _currentAssembly = assembly;
        }

        public void AssemblyFinished(string assemblyFilename, int total, int failed, int skipped, double time)
        {
        }

        public void AssemblyStart(string assemblyFilename, string configFilename, string xUnitVersion)
        {
        }

        public bool ClassFailed(string className, string exceptionType, string message, string stackTrace)
        {
            var result = getResult(0, Shared.Results.TestState.Panic, className, message, stackTrace);
            _results.Add(result);
            _channel.TestFinished(result);
            return true;
        }

        public void ExceptionThrown(string assemblyFilename, Exception exception)
        {
            var result = getResult(0, Shared.Results.TestState.Panic, "Internal XUnit error", exception.ToString());
            _results.Add(result);
            _channel.TestFinished(result);
        }

        public void TestFailed(string name, string type, string method, double duration, string output, string exceptionType, string message, string stackTrace)
        {
            var result = getResult(duration, Shared.Results.TestState.Failed, name, message, stackTrace);
            _results.Add(result);
            _channel.TestFinished(result);
        }

        public bool TestFinished(string name, string type, string method)
        {
            return true;
        }

        public void TestPassed(string name, string type, string method, double duration, string output)
        {
            var result = getResult(duration, Shared.Results.TestState.Passed, name, output);
            _results.Add(result);
            _channel.TestFinished(result);
        }

        public void TestSkipped(string name, string type, string method, string reason)
        {
            var result = getResult(0, Shared.Results.TestState.Ignored, name, reason);
            _results.Add(result);
            _channel.TestFinished(result);
        }

        public bool TestStart(string name, string type, string method)
        {
            if (_channel != null)
                _channel.TestStarted(name);
            return true;
        }

        private Shared.Results.TestResult getResult(double duration, Shared.Results.TestState state, string name, string message)
        {
            return getResult(duration, state, name, message, null);
        }

        private Shared.Results.TestResult getResult(double duration, Shared.Results.TestState state, string name, string message, string stackLines)
        {
            var testName = name.IndexOf("(") == -1 ? name : name.Substring(0, name.IndexOf("("));
            var result = new Shared.Results.TestResult("XUnit", _currentAssembly, "", duration * 1000, testName, name, state, message);
            if (stackLines != null)
            {
                var lines = stackLines.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                    result.AddStackLine(new Shared.Results.StackLine(line));
            }
            return result;
        }
    }
}
