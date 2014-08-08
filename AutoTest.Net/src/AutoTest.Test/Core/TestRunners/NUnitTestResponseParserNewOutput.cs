using System;
using NUnit.Framework;
using Rhino.Mocks;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Caching.Projects;
using System.IO;
using AutoTest.Core.Messaging;
using AutoTest.Messages;
namespace AutoTest.Test
{
	[TestFixture]
	public class NUnitTestResponseParserNewOutput
	{
		private NUnitTestResponseParser _parser;

        [SetUp]
        public void SetUp()
        {
            var bus = MockRepository.GenerateMock<IMessageBus>();
            _parser = new NUnitTestResponseParser(bus, TestRunner.NUnit);
			var sources = new TestRunInfo[]
				{ 
					new TestRunInfo(new Project("project1", null), "/home/ack/src/AutoTest.Net/src/AutoTest.TestCore/bin/Debug/AutoTest.TestCore.dll"),
					new TestRunInfo(new Project("project2", null), "/home/ack/src/AutoTest.Net/src/AutoTest.Test/bin/Debug/AutoTest.Test.dll"),
					new TestRunInfo(new Project("project3", null), "/home/ack/src/AutoTest.Net/src/AutoTest.WinForms.Test/bin/Debug/AutoTest.WinForms.Test.dll")
				};
			_parser.Parse(File.ReadAllText("TestResources/NUnit/NewOutput.txt"), sources, false);
        }

		[Test]
		public void Should_containt_tests_for_two_assemblies()
		{
			_parser.Result.Length.ShouldEqual(3);
		}
	}
}

