using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Messaging;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
    [TestFixture]
    public class RunReportTest
    {
        private RunReport _report;

        [SetUp]
        public void SetUp()
        {
            _report = new RunReport();
        }

        [Test]
        public void Should_add_succeeded_build_action()
        {
            _report.AddBuild("Project", new TimeSpan(0, 0, 25), true);
            _report.RunActions[0].Type.ShouldEqual(InformationType.Build);
            _report.RunActions[0].Project.ShouldEqual("Project");
            _report.RunActions[0].TimeSpent.ShouldEqual(new TimeSpan(0, 0, 25));
            _report.RunActions[0].Succeeded.ShouldBeTrue();
            _report.NumberOfBuildsSucceeded.ShouldEqual(1);
        }

        [Test]
        public void Should_add_failed_build_action()
        {
            _report.AddBuild("Project", new TimeSpan(0, 0, 25), false);
            _report.RunActions[0].Type.ShouldEqual(InformationType.Build);
            _report.RunActions[0].Project.ShouldEqual("Project");
            _report.RunActions[0].TimeSpent.ShouldEqual(new TimeSpan(0, 0, 25));
            _report.RunActions[0].Succeeded.ShouldBeFalse();
            _report.NumberOfBuildsFailed.ShouldEqual(1);
        }

        [Test]
        public void Should_add_test_run()
        {
            _report.AddTestRun("Project", "Assembly", new TimeSpan(0, 0, 30), 10, 20, 30);
            _report.RunActions[0].Type.ShouldEqual(InformationType.TestRun);
            _report.RunActions[0].Project.ShouldEqual("Project");
            _report.RunActions[0].Assembly.ShouldEqual("Assembly");
            _report.RunActions[0].TimeSpent.ShouldEqual(new TimeSpan(0, 0, 30));
            _report.NumberOfTestsPassed.ShouldEqual(10);
            _report.NumberOfTestsIgnored.ShouldEqual(20);
            _report.NumberOfTestsFailed.ShouldEqual(30);
            _report.NumberOfTestsRan.ShouldEqual(60);
        }

        [Test]
        public void Builds_ran_should_be_a_combination_of_failed_an_succeeded()
        {
            _report.AddBuild("", new TimeSpan(), true);
            _report.AddBuild("", new TimeSpan(), false);
            _report.NumberOfProjectsBuilt.ShouldEqual(2);
        }
    }
}
