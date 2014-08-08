using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Results;

namespace AutoTest.TestRunners.Shared
{
    public interface ITestRunProcessFeedback
    {
        void ProcessStart(string commandline);
        void TestStarted(string signature);
        void TestFinished(TestResult result);
    }
}
