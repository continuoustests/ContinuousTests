using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;
using System.Collections;
using NUnit.Util;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners.NUnit
{
    class TestHarvester : MarshalByRefObject, EventListener
    {
        private string currentAssembly = "Run started was never called";
        private List<Shared.Results.TestResult> _results = new List<Shared.Results.TestResult>();
        private ITestFeedbackProvider _channel;

        public IEnumerable<AutoTest.TestRunners.Shared.Results.TestResult> Results { get { return _results; } }

        public TestHarvester(ITestFeedbackProvider channel)
        {
            _channel = channel;
        }

        public void RunStarted(string name, int testCount)
        {
            if (name == null)
                return;
            if (name.Trim().Length == 0)
                return;
            currentAssembly = name;
        }

        public void RunFinished(TestResult result)
        {
        }

        public void RunFinished(Exception exception)
        {
        }

        public void TestFinished(TestResult testResult)
        {
            switch (testResult.ResultState)
            {
                case ResultState.Error:
                case ResultState.Failure:
                case ResultState.Cancelled:
                    var failure = new AutoTest.TestRunners.Shared.Results.TestResult("NUnit", currentAssembly, getFixture(testResult.Test.TestName.FullName), testResult.Time * 1000, testResult.Test.TestName.FullName, TestRunners.Shared.Results.TestState.Failed, testResult.Message);
                    failure.AddStackLines(getStackLines(testResult).ToArray());
                    _results.Add(failure);
                    _channel.TestFinished(failure);
                    break;

                case ResultState.Success:
                    var success = new AutoTest.TestRunners.Shared.Results.TestResult("NUnit", currentAssembly, getFixture(testResult.Test.TestName.FullName), testResult.Time * 1000, testResult.Test.TestName.FullName, TestRunners.Shared.Results.TestState.Passed, testResult.Message);
                    _results.Add(success);
                    _channel.TestFinished(success);
                    break;

                case ResultState.Inconclusive:
                case ResultState.Ignored:
                case ResultState.Skipped:
                case ResultState.NotRunnable:
                    var ignoreResult = new AutoTest.TestRunners.Shared.Results.TestResult("NUnit", currentAssembly, getFixture(testResult.Test.TestName.FullName), testResult.Time * 1000, testResult.Test.TestName.FullName, TestRunners.Shared.Results.TestState.Ignored, testResult.Message);
                    ignoreResult.AddStackLines(getStackLines(testResult).ToArray());
                    _results.Add(ignoreResult);
                    _channel.TestFinished(ignoreResult);
                    break;
            }
        }

        private static IEnumerable<TestRunners.Shared.Results.StackLine> getStackLines(TestResult testResult)
        {
            var stackLines = new List<TestRunners.Shared.Results.StackLine>();
            string stackTrace = StackTraceFilter.Filter(testResult.StackTrace);
            if (stackTrace != null && stackTrace != string.Empty)
            {
                string[] trace = stackTrace.Split(System.Environment.NewLine.ToCharArray());
                foreach (string s in trace)
                {
                    if (s != string.Empty)
                        stackLines.Add(new TestRunners.Shared.Results.StackLine(s));
                }
            }
            return stackLines;
        }

        private string getFixture(string fullname)
        {
            var end = fullname.LastIndexOf(".");
            if (end == -1)
                return "";
            return fullname.Substring(0, end);
        }

        public void TestStarted(TestName testName)
        {
            if (_channel != null)
                _channel.TestStarted(testName.FullName);
        }

        public void SuiteStarted(TestName testName)
        {
        }

        public void SuiteFinished(TestResult suiteResult)
        {
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject.GetType() != typeof(System.Threading.ThreadAbortException))
                this.UnhandledException((Exception)e.ExceptionObject);
        }


        public void UnhandledException(Exception exception)
        {
            _results.Add(new AutoTest.TestRunners.Shared.Results.TestResult("NUnit", currentAssembly, "", 0, "Unhandled exception", TestRunners.Shared.Results.TestState.Panic, exception.ToString()));
        }

        public void TestOutput(TestOutput output)
        {
        }


        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
