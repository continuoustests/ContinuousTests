using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared;
using System.IO;

namespace AutoTest.TestRunners.Tests
{
    [TestFixture]
    public class TestRunProcessTests
    {
        //[Test]
        //public void Should_run_tests()
        //{
        //    return;
        //    var options = new RunOptions();
        //    var runner = new RunnerOptions("NUnit");
        //    options.AddTestRun(runner);
        //    runner.AddAssembly(new AssemblyOptions(@"C:\Users\ack\src\AutoTest.Net\src\AutoTest.TestRunner\Plugins\AutoTest.TestRunners.NUnit.Tests.TestResource\bin\Debug\AutoTest.TestRunners.NUnit.Tests.TestResource.dll"));
        //    runner.AddAssembly(new AssemblyOptions(@"C:\Users\ack\src\ClassLibrary1-v40\ClassLibrary1-v40\bin\AutoTest.Net\ClassLibrary1-v40.dll"));
        //    runner = new RunnerOptions("XUnit");
        //    options.AddTestRun(runner);
        //    runner.AddAssembly(new AssemblyOptions(@"C:\Users\ack\src\AutoTest.Net\src\AutoTest.TestRunner\Plugins\AutoTest.TestRunners.XUnit.Tests.TestResource\bin\AutoTest.Net\AutoTest.TestRunners.XUnit.Tests.TestResource.dll"));
        //    var process = new TestRunProcess(new feedback());
        //    var results = process.ProcessTestRuns(options);

        //    var sb = new StringBuilder();
        //    sb.AppendLine("");
        //    foreach (var result in results)
        //        sb.AppendLine(result.Runner + ", " + result.TestName + ", " + result.Message);

        //    throw new Exception(sb.ToString());
        //}
    }

    class feedback : ITestRunProcessFeedback
    {
        private object _lock = new object();

        public void ProcessStart(string commandline)
        {
            lock (_lock)
            {
                using (var writer = new StreamWriter(@"C:\tmp\bleh.txt", true))
                {
                    writer.WriteLine(commandline);
                }
            }
        }

        public void TestFinished(AutoTest.TestRunners.Shared.Results.TestResult result)
        {
        }

        public void TestStarted(string signature)
        {
            throw new NotImplementedException();
        }
    }
}
