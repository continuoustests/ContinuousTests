using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
    [TestFixture]
    public class RunInfoTests
    {
        private RunInfo _info;

        [SetUp]
        public void SetUp()
        {
            _info = new RunInfo(new Project("", new ProjectDocument(ProjectType.CSharp)));
        }

        [Test]
        public void Should_add_multiple_tests()
        {
            _info.AddTestsToRun(TestRunner.MSTest, "MyAssembly.MyClass.MyTest");
            _info.AddTestsToRun(TestRunner.NUnit, "MyAssembly2.Class.AnotherTest");
            var tests = _info.GetTests();
            tests.Length.ShouldEqual(2);
            tests[0].Runner.ShouldEqual(TestRunner.MSTest);
            tests[0].Test.ShouldEqual("MyAssembly.MyClass.MyTest");
            tests[1].Runner.ShouldEqual(TestRunner.NUnit);
            tests[1].Test.ShouldEqual("MyAssembly2.Class.AnotherTest");
        }

        [Test]
        public void Should_get_test_pr_runner()
        {
            _info.AddTestsToRun(TestRunner.MSTest, "MyAssembly.MyClass.MyTest");
            _info.AddTestsToRun(TestRunner.NUnit, "MyAssembly2.Class.AnotherTest");
            var tests = _info.GetTestsFor(TestRunner.NUnit);
            tests.Length.ShouldEqual(1);
            tests[0].ShouldEqual("MyAssembly2.Class.AnotherTest");
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
        public void Should_clone_to_test_run_info()
        {
            _info.AddTestsToRun(TestRunner.MSTest, "MyAssembly.MyClass.MyTest");
            _info.AddTestsToRun(TestRunner.NUnit, "MyAssembly2.Class.AnotherTest");
            _info.ShouldOnlyRunSpcifiedTestsFor(TestRunner.Any);
            _info.ShouldRerunAllTestWhenFinishedFor(TestRunner.MSTest);
            _info.ShouldRerunAllTestWhenFinishedFor(TestRunner.XUnit);
            var testInfo = _info.CloneToTestRunInfo();

            testInfo.Project.Key.Equals(_info.Project.Key);
            testInfo.Assembly.ShouldEqual(_info.Assembly);
            testInfo.GetTests().Length.ShouldEqual(2);
            testInfo.GetTestsFor(TestRunner.MSTest).Length.ShouldEqual(1);
            testInfo.GetTestsFor(TestRunner.NUnit).Length.ShouldEqual(1);
            testInfo.OnlyRunSpcifiedTestsFor(TestRunner.NUnit).ShouldBeTrue();
            testInfo.RerunAllTestWhenFinishedFor(TestRunner.MSTest).ShouldBeTrue();
            testInfo.RerunAllTestWhenFinishedFor(TestRunner.XUnit).ShouldBeTrue();
            testInfo.RerunAllTestWhenFinishedFor(TestRunner.NUnit).ShouldBeFalse();
            testInfo.RerunAllTestWhenFinishedFor(TestRunner.Any).ShouldBeFalse();
            testInfo.RerunAllTestWhenFinishedForAny().ShouldBeTrue();
        }
    }
}
