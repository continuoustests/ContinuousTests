using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Logging;

namespace AutoTest.TestRunners.Shared.Communication
{
    public interface ITestFeedbackProvider
    {
        void TestStarted(string testName);
        void TestFinished(TestResult result);
    }

    public class NullTestFeedbackProvider : ITestFeedbackProvider
    {
        public void TestFinished(TestResult result)
        {
        }

        public void TestStarted(string testName)
        {
        }
    }

    public class TestFeedbackProvider : ITestFeedbackProvider
    {
        private PipeServer _channel;

        public TestFeedbackProvider(PipeServer channel)
        {
            _channel = channel;
        }

        public void TestStarted(string testName)
        {
            if (testName == null)
                return;
            Logger.WriteChunk("\t{0}...", testName);
            _channel.Send("Test started:" + testName);
        }

        public void TestFinished(TestResult result)
        {
            if (result == null)
            {
                Logger.Write(" - Testresult was null");
                return;
            }
            var xml = result.ToXml();
            if (xml == null)
            {
                Logger.Write(" - Could not generate xml from " + result.TestName);
                return;
            }
            Logger.Write(" - {0}", result.State.ToString());
            _channel.Send(xml);
        }
    }
}
