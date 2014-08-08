using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.TestRunners;
using NUnit.Framework;
using AutoTest.Messages;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class TestResultTests
    {
        [Test]
        public void Should_map_passed()
        {
            new TestResult(TestRunner.NUnit, TestRunStatus.Passed, String.Empty).Status.ShouldEqual(TestRunStatus.Passed);
        }

        [Test]
        public void Should_map_failed()
        {
            new TestResult(TestRunner.NUnit, TestRunStatus.Failed, String.Empty).Status.ShouldEqual(TestRunStatus.Failed);
        }

        [Test]
        public void Should_map_ignored()
        {
            new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, String.Empty).Status.ShouldEqual(TestRunStatus.Ignored);
        }

        [Test]
        public void Should_return_passed_message()
        {
            var passedResult = TestResult.Pass();
            passedResult.Message.ShouldEqual(string.Empty);
            passedResult.Status.ShouldEqual(TestRunStatus.Passed);
        }

        [Test]
        public void Should_return_fail_message()
        {
            var failedMessage = TestResult.Fail("omg!");
            failedMessage.Status.ShouldEqual(TestRunStatus.Failed);
            failedMessage.Name.ShouldEqual("omg!");
        }

        [Test]
        public void Should_map_message()
        {
            new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "asdf").Name.ShouldEqual("asdf");
        }

        [Test]
        public void Should_be_equal()
        {
            var result1 = new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "test name", "test message", new IStackLine[] { });
            var result2 = new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "test name", "test message", new IStackLine[] { });
            result1.Equals(result2).ShouldBeTrue();
        }

        [Test]
        public void Should_not_be_equal()
        {
            var result1 = new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "test name", "test message", new IStackLine[] { });
            var result2 = new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "another test name", "test message", new IStackLine[] { });
            result1.Equals(result2).ShouldBeFalse();
        }
    }
}
