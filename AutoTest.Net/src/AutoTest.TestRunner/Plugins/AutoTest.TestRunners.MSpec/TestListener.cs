using System;
using System.Collections.Generic;
using System.Linq;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners.MSpec
{
    class TestListenerProxy : LinFu.DynamicProxy.IInterceptor
    {
        private string _assembly;
        private ITestFeedbackProvider _feedback;
        private DateTime _start = DateTime.MinValue;
        private List<TestResult> _results = new List<TestResult>();

        public TestResult[] Results { get { return _results.ToArray(); } }

        public TestListenerProxy(ITestFeedbackProvider feedback, string assembly)
        {
            _feedback = feedback;
            _assembly = assembly;
        }

        public object Intercept(LinFu.DynamicProxy.InvocationInfo invocation)
        {
            var args = invocation.Arguments;
            if (invocation.TargetMethod.Name == "OnSpecificationEnd")
                onSpecificationEnd(args[0], args[1]);
            else if (invocation.TargetMethod.Name == "OnSpecificationStart")
                onSpecificationStart(args[0]);
            return null;
        }

        private void onSpecificationStart(object specification)
        {
            _start = DateTime.Now;
            if (_feedback != null)
                _feedback.TestStarted(specification.Get<string>("ContainingType"));
        }

        private void onSpecificationEnd(object specification, object result)
        {
            var test = new TestResult(
                    "MSpec",
                    _assembly,
                    specification.Get<string>("ContainingType"),
                    DateTime.Now.Subtract(_start).TotalMilliseconds,
                    specification.Get<string>("ContainingType"),
                    specification.Get<string>("ContainingType"),
                    getState(result.Get<object>("Status").ToString()),
                    getMessage(result.Get<object>("Exception")));
            test.AddStackLines(getStackLines(result.Get<object>("Exception")));
            _results.Add(test);
            if (_feedback != null)
                _feedback.TestFinished(test);
        }

        private StackLine[] getStackLines(object exceptionResult)
        {
            if (exceptionResult == null)
                return new StackLine[] { };
            return exceptionResult.Get<string>("StackTrace")
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new StackLine(x)).ToArray();
        }

        private string getMessage(object exceptionResult)
        {
            if (exceptionResult == null)
                return "";
            return exceptionResult.Get<string>("Message");
        }

        private TestState getState(string status)
        {
            if (status == "Failing")
                return TestState.Failed;
            else if (status == "Ignored")
                return TestState.Ignored;
            else if (status == "NotImplemented")
                return TestState.Ignored;
            else if (status == "Passing")
                return TestState.Passed;
            return TestState.Panic;
        }
    }
}
