using System;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Caching.Projects;
using AutoTest.Messages;
namespace AutoTest.Test
{
	[TestFixture]
	public class TestRunInfoTests
	{
		private TestRunInfo _info;
		
		[SetUp]
		public void SetUp()
		{
			_info = new TestRunInfo(new Project("", new ProjectDocument(ProjectType.CSharp)), "");
		}
		
		[Test]
		public void Should_add_multiple_tests()
		{
			_info.AddTestsToRun(new TestToRun[] { new TestToRun(TestRunner.MSTest, "MyAssembly.MyClass.MyTest"), new TestToRun(TestRunner.NUnit, "MyAssembly2.Class.AnotherTest") });
			_info.GetTests().Length.ShouldEqual(2);
            _info.GetTests()[0].Runner.ShouldEqual(TestRunner.MSTest);
            _info.GetTests()[0].Test.ShouldEqual("MyAssembly.MyClass.MyTest");
            _info.GetTests()[1].Runner.ShouldEqual(TestRunner.NUnit);
            _info.GetTests()[1].Test.ShouldEqual("MyAssembly2.Class.AnotherTest");
		}

        [Test]
        public void Should_run_only_specified_tests_for_spesific_type()
        {
            _info.ShouldOnlyRunSpcifiedTestsFor(TestRunner.NUnit);
            _info.OnlyRunSpcifiedTestsFor(TestRunner.NUnit).ShouldBeTrue();
        }

        [Test]
        public void Should_run_only_specified_tests_for_all_types()
        {
            _info.ShouldOnlyRunSpcifiedTestsFor(TestRunner.Any);
            _info.OnlyRunSpcifiedTestsFor(TestRunner.NUnit).ShouldBeTrue();
        }

        [Test]
        public void Should_rerun_all_tests_for_spesific_type()
        {
            _info.ShouldRerunAllTestWhenFinishedFor(TestRunner.NUnit);
            _info.RerunAllTestWhenFinishedFor(TestRunner.NUnit).ShouldBeTrue();
        }

        [Test]
        public void Should_rerun_all_tests_for_all_types()
        {
            _info.ShouldRerunAllTestWhenFinishedFor(TestRunner.Any);
            _info.RerunAllTestWhenFinishedFor(TestRunner.NUnit).ShouldBeTrue();
        }

        [Test]
        public void Should_rerun_all_tests_for_any_types()
        {
            _info.ShouldRerunAllTestWhenFinishedFor(TestRunner.NUnit);
            _info.RerunAllTestWhenFinishedForAny().ShouldBeTrue();
        }
	}
}

