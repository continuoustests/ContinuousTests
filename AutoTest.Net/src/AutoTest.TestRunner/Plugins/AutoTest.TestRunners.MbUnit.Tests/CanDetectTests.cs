using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;

namespace AutoTest.TestRunners.MbUnit.Tests
{
    [TestFixture]
    public class When_looking_for_a_specific_test_in_an_assembly : TestRunnerScenario
    {
        [Test]
        public void and_the_test_does_not_exist_it_returns_false()
        {
            Assert.That(_runner.IsTest(getAssembly(), "AutoTest.TestRunners.MbUnitTests.Tests.TestResource.ClassContainingTests.A_passing_test_that_does_not_exist"), Is.False);
        }

        [Test]
        public void and_the_test_exists_it_returns_true()
        {
            Assert.That(_runner.IsTest(getAssembly(), "AutoTest.TestRunners.MbUnitTests.Tests.TestResource.ClassContainingTests.A_passing_test"), Is.True);
        }
    }

    [TestFixture]
    public class When_checking_wether_an_assembly_contains_tests : TestRunnerScenario
    {
        [Test]
        public void and_it_contains_MbUnit_tests_it_should_return_true()
        {
            Assert.That(_runner.ContainsTestsFor(getAssembly()), Is.True);
        }

        [Test]
        public void and_it_does_not_contain_MbUnit_tests_it_should_return_false()
        {
            Assert.That(_runner.ContainsTestsFor(Assembly.GetExecutingAssembly().Location), Is.False);
        }
    }
}
