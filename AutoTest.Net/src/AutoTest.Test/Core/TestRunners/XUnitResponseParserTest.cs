using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.TestRunners.TestRunners;
using Castle.Core.Logging;
using Rhino.Mocks;
using AutoTest.Core.Messaging;
using System.IO;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Caching.Projects;
using AutoTest.Messages;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class XUnitResponseParserTest
    {
        private NUnitTestResponseParser _parser;

        [SetUp]
        public void SetUp()
        {
            var bus = MockRepository.GenerateMock<IMessageBus>();
            _parser = new NUnitTestResponseParser(bus, TestRunner.XUnit);
			var sources = new TestRunInfo[]
				{ 
					new TestRunInfo(new Project("project1", null), string.Format("/home/ack/backup/WorkWin7/src/DotNET/Temp/SomeProjectUsingXUnit/bin/Debug/SomeProjectUsingXUnit.dll", Path.DirectorySeparatorChar))
				};
            var text = File.ReadAllText("TestResources/NUnit/XUnitOutput.txt");
            text = text.Replace("\r\n", "").Replace("\n", "");
			_parser.Parse(text, sources, false);
        }

        [Test]
        public void Should_find_succeeded_test()
        {
            _parser.Result[0].All.Length.ShouldEqual(3);
            _parser.Result[0].Passed.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_find_test_name()
        {
            _parser.Result[0].All[0].Name.ShouldEqual("SomeProjectUsingXUnit.AbstractTestClass.Should_skip_this_test");
        }

        [Test]
        public void Should_find_failed_test()
        {
            _parser.Result[0].All.Length.ShouldEqual(3);
            _parser.Result[0].Failed.Length.ShouldEqual(1);
            _parser.Result[0].Failed[0].Message.ShouldEqual("Assert.Equal() FailureExpected: 4Actual:   3");
            _parser.Result[0].Failed[0].StackTrace.Length.ShouldEqual(3);
        }
		
		[Test]
		public void Should_extract_assemblies()
		{
			_parser.Result[0].Assembly.ShouldEqual(string.Format("/home/ack/backup/WorkWin7/src/DotNET/Temp/SomeProjectUsingXUnit/bin/Debug/SomeProjectUsingXUnit.dll", Path.DirectorySeparatorChar));
		}
		
		[Test]
		public void Should_extract_run_time()
		{
			_parser.Result[0].TimeSpent.ShouldEqual(new TimeSpan(0, 0, 0, 0, 004));
		}
    }
}
