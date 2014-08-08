using System;
using NUnit.Framework;
using Rhino.Mocks;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Core.Messaging;
using System.IO;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Caching.Projects;
using AutoTest.Messages;
namespace AutoTest.Test.Core.TestRunners
{
	[TestFixture]
	public class NUnitTestResponseParserMultipleAssembliesTest
	{
		private NUnitTestResponseParser _parser;

        [SetUp]
        public void SetUp()
        {
            var bus = MockRepository.GenerateMock<IMessageBus>();
            _parser = new NUnitTestResponseParser(bus, TestRunner.NUnit);
			var sources = new TestRunInfo[]
				{ 
					new TestRunInfo(new Project("project1", null), "/SomePath/AutoTest.WinForms.Test/bin/Debug/AutoTest.WinForms.Test.dll"),
					new TestRunInfo(new Project("project2", null), "/SomePath/AutoTest.Console.Test/bin/Debug/AutoTest.Console.Test.dll")
				};
			_parser.Parse(File.ReadAllText("TestResources/NUnit/multipleAssemblies.txt"), sources, false);
        }

		[Test]
		public void Should_containt_tests_for_two_assemblies()
		{
			_parser.Result.Length.ShouldEqual(2);
		}
		
		[Test]
		public void Should_extract_assemblies()
		{
			_parser.Result[0].Assembly.ShouldEqual("/SomePath/AutoTest.WinForms.Test/bin/Debug/AutoTest.WinForms.Test.dll");
			_parser.Result[1].Assembly.ShouldEqual("/SomePath/AutoTest.Console.Test/bin/Debug/AutoTest.Console.Test.dll");
		}
		
		[Test]
		public void Should_extract_run_time()
		{
			_parser.Result[0].TimeSpent.ShouldEqual(new TimeSpan(0, 0, 0, 2, 415));
			_parser.Result[1].TimeSpent.ShouldEqual(new TimeSpan(0, 0, 0, 0, 884));
		}
		
        [Test]
        public void Should_find_succeeded_test()
        {
            _parser.Result[0].All.Length.ShouldEqual(7);
            _parser.Result[1].All.Length.ShouldEqual(1);
        }
	}
}

