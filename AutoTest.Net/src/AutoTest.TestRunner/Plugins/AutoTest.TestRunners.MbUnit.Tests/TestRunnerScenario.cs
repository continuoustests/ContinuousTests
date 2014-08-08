using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace AutoTest.TestRunners.MbUnit.Tests
{
    public class TestRunnerScenario
    {
        protected Runner _runner;

        [SetUp]
        public void Setup()
        {
            _runner = new Runner();
        }

        protected string getAssembly()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "AutoTest.TestRunners.MbUnit.Tests.TestResource.dll");
        }
    }
}
