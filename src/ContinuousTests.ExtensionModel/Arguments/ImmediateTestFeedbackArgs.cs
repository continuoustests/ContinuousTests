using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContinuousTests.ExtensionModel.Arguments
{
    public class ImmediateTestFeedbackArgs : EventArgs
    {
        public string CurrentAssembly { get; internal set; }
        public int TestsCompleted { get; internal set; }
        public int TotalNumberOfTests { get; internal set; }
        public TestResult[] FailedTests { get; internal set; }
        public TestResult[] FailedButNowPassingTests { get; internal set; }

        internal static ImmediateTestFeedbackArgs FromInternal(AutoTest.Messages.LiveTestStatusMessage msg)
        {
            return new ImmediateTestFeedbackArgs()
            {
                CurrentAssembly = msg.CurrentAssembly,
                TestsCompleted = msg.TestsCompleted,
                TotalNumberOfTests = msg.TotalNumberOfTests,
                FailedTests = msg.FailedTests.Select(x => getTest(x)).ToArray(),
                FailedButNowPassingTests = msg.FailedButNowPassingTests.Select(x => getTest(x)).ToArray()
            };
        }

        private static TestResult getTest(AutoTest.Messages.LiveTestStatus x)
        {
            return TestsFinishedArgs.GetResult(x.Assembly, x.Test.Message, x.Test.Name, x.Test.Runner, x.Test.Status, x.Test.StackTrace);
        }
    }
}
