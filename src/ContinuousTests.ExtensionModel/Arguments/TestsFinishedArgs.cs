using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Client.Handlers;

namespace ContinuousTests.ExtensionModel.Arguments
{
    public class TestsFinishedArgs : EventArgs
    {
        public string Assembly { get; internal set; }
        public TestResult[] All { get; internal set; }
        public TestResult[] Failed { get { return All.Where(x => x.Status == TestState.Failed).ToArray(); } }
        public TestResult[] Ignored { get { return All.Where(x => x.Status == TestState.Ignored).ToArray(); } }
        public TestResult[] Passed { get { return All.Where(x => x.Status == TestState.Passed).ToArray(); } }

        internal static TestsFinishedArgs FromInternal(AutoTest.Messages.TestRunMessage msg)
        {
            return new TestsFinishedArgs()
                        {
                            Assembly = msg.Results.Assembly,
                            All = msg.Results.All.Select(x => getResult(msg.Results.Assembly, x)).ToArray()
                        };
        }

        private static TestResult getResult(string assembly, AutoTest.Messages.TestResult x)
        {
            return GetResult(assembly, x.Message, x.Name, x.Runner, x.Status, x.StackTrace);
        }

        internal static TestResult GetResult(string assembly, string message, string name, AutoTest.Messages.TestRunner runner, AutoTest.Messages.TestRunStatus status, IEnumerable<AutoTest.Messages.IStackLine> stackLines)
        {
            return new TestResult()
                        {
                            Assembly = assembly,
                            Message = message,
                            Name = name,
                            Runner = getRunner(runner),
                            Status = getStatus(status),
                            StackTrace = stackLines.Select(y => getStackTrace(y)).ToArray()
                        };
        }

        private static TestRunner getRunner(AutoTest.Messages.TestRunner testRunner)
        {
            if (testRunner == AutoTest.Messages.TestRunner.Any)
                return TestRunner.Any;
            if (testRunner == AutoTest.Messages.TestRunner.MSpec)
                return TestRunner.MSpec;
            if (testRunner == AutoTest.Messages.TestRunner.MSTest)
                return TestRunner.MSTest;
            if (testRunner == AutoTest.Messages.TestRunner.NUnit)
                return TestRunner.NUnit;
            return TestRunner.XUnit;
        }

        private static TestState getStatus(AutoTest.Messages.TestRunStatus testRunStatus)
        {
            if (testRunStatus == AutoTest.Messages.TestRunStatus.Failed)
                return TestState.Failed;
            if (testRunStatus == AutoTest.Messages.TestRunStatus.Ignored)
                return TestState.Ignored;
            return TestState.Passed;
        }

        private static StackLine getStackTrace(AutoTest.Messages.IStackLine x)
        {
            return new StackLine()
                            {
                                File = x.File,
                                LineNumber = x.LineNumber,
                                Method = x.Method
                            };
        }
    }

    public class TestResult
    {
        public string Assembly { get; internal set; }
        public string Message { get; internal set; }
        public string Name { get; internal set; }
        public TestRunner Runner { get; internal set; }
        public StackLine[] StackTrace { get; internal set; }
        public TestState Status { get; internal set; }
    }

    public class StackLine
    {
        public string File { get; internal set; }
        public int LineNumber { get; internal set; }
        public string Method { get; internal set; }
    }

    public enum TestState
    {
        Passed = 0,
        Ignored = 1,
        Failed = 2,
    }

    public enum TestRunner
    {
        Any = 0,
        NUnit = 1,
        MSTest = 2,
        XUnit = 3,
        MSpec = 4,
    }
}
