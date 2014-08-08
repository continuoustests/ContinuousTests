using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared.Options;

namespace AutoTest.TestRunners.MbUnit.Tests
{
    [TestFixture]
    public class When_running_a_test : TestRunnerScenario
    {
        [Test]
        public void and_nothing_is_specified_it_should_run_all_test()
        {
            var settings = new RunSettings(new AssemblyOptions(getAssembly()), new string[] { }, null);
            Assert.That(_runner.Run(settings).Count(), Is.EqualTo(5));
        }

        [Test]
        public void and_a_test_is_specified_it_should_only_run_specified_tests()
        {
            var settings = new RunSettings(new AssemblyOptions(getAssembly()), new string[] { }, null);
            settings.Assembly.AddTest("AutoTest.TestRunners.MbUnitTests.Tests.TestResource.ClassContainingTests.A_passing_test");
            settings.Assembly.AddTest("AutoTest.TestRunners.MbUnitTests.Tests.TestResource.ClassContainingTests.A_failing_test");
            Assert.That(_runner.Run(settings).Count(), Is.EqualTo(2));
        }

        [Test]
        public void and_a_member_is_specified_it_should_only_run_tests_contained_by_these_members()
        {
            var settings = new RunSettings(new AssemblyOptions(getAssembly()), new string[] { }, null);
            settings.Assembly.AddMember("AutoTest.TestRunners.MbUnit.Tests.TestResource.AnotherClassContainingTests");
            Assert.That(_runner.Run(settings).Count(), Is.EqualTo(1));
        }

        [Test]
        public void and_a_namespace_is_specified_it_should_only_run_tests_contained_by_these_namespaces()
        {
            var settings = new RunSettings(new AssemblyOptions(getAssembly()), new string[] { }, null);
            settings.Assembly.AddNamespace("AutoTest.TestRunners.MbUnit.Tests.TestResource.AnotherNamespace");
            Assert.That(_runner.Run(settings).Count(), Is.EqualTo(1));
            Assert.That(_runner.Run(settings).ElementAt(0).TestName,
                Is.EqualTo("AutoTest.TestRunners.MbUnit.Tests.TestResource.AnotherNamespace.TestsInAnotherNamespace.Even_another_test"));
        }

        [Test]
        public void and_a_namespace_is_specified_it_should_only_run_tests_contained_by_these_namespaces_and_all_sub_namespaces()
        {
            // Note that the namespaces varies between MbUnit.Tests and MbUnitTests.Tests
            var settings = new RunSettings(new AssemblyOptions(getAssembly()), new string[] { }, null);
            settings.Assembly.AddNamespace("AutoTest.TestRunners.MbUnit.Tests.TestResource");
            Assert.That(_runner.Run(settings).Count(), Is.EqualTo(1));
        }

        [Test]
        public void and_it_passes_it_should_return_passing_result()
        {
            var settings = new RunSettings(new AssemblyOptions(getAssembly()), new string[] { }, null);
            settings.Assembly.AddTest("AutoTest.TestRunners.MbUnitTests.Tests.TestResource.ClassContainingTests.A_passing_test");
            var result = _runner.Run(settings);

            var test = result.ElementAt(0);
            Assert.That(test.Assembly, Is.EqualTo(getAssembly()));
            Assert.That(test.DurationInMilliseconds, Is.GreaterThan(0));
            Assert.That(test.Runner, Is.EqualTo("MbUnit"));
            Assert.That(test.State, Is.EqualTo(Shared.Results.TestState.Passed));
            Assert.That(test.TestFixture, Is.EqualTo("AutoTest.TestRunners.MbUnitTests.Tests.TestResource.ClassContainingTests"));
            Assert.That(test.TestName, Is.EqualTo("AutoTest.TestRunners.MbUnitTests.Tests.TestResource.ClassContainingTests.A_passing_test"));
        }

        [Test]
        public void and_it_fails_it_should_return_failed_result()
        {
            var settings = new RunSettings(new AssemblyOptions(getAssembly()), new string[] { }, null);
            settings.Assembly.AddTest("AutoTest.TestRunners.MbUnitTests.Tests.TestResource.ClassContainingTests.A_failing_test");
            var result = _runner.Run(settings);

            Assert.That(result.Count(), Is.EqualTo(1));
            var test = result.ElementAt(0);
            Assert.That(test.Assembly, Is.EqualTo(getAssembly()));
            Assert.That(test.DurationInMilliseconds, Is.GreaterThan(0));
            Assert.That(test.Runner, Is.EqualTo("MbUnit"));
            Assert.That(test.State, Is.EqualTo(Shared.Results.TestState.Failed));
            Assert.That(test.TestFixture, Is.EqualTo("AutoTest.TestRunners.MbUnitTests.Tests.TestResource.ClassContainingTests"));
            Assert.That(test.TestName, Is.EqualTo("AutoTest.TestRunners.MbUnitTests.Tests.TestResource.ClassContainingTests.A_failing_test"));
        }

        [Test]
        public void and_its_inconclusive_it_should_return_ignored_result()
        {
            var settings = new RunSettings(new AssemblyOptions(getAssembly()), new string[] { }, null);
            settings.Assembly.AddTest("AutoTest.TestRunners.MbUnitTests.Tests.TestResource.ClassContainingTests.An_inconclusive_test");
            var result = _runner.Run(settings);

            Assert.That(result.Count(), Is.EqualTo(1));
            var test = result.ElementAt(0);
            Assert.That(test.Assembly, Is.EqualTo(getAssembly()));
            Assert.That(test.Runner, Is.EqualTo("MbUnit"));
            Assert.That(test.State, Is.EqualTo(Shared.Results.TestState.Ignored));
            Assert.That(test.TestFixture, Is.EqualTo("AutoTest.TestRunners.MbUnitTests.Tests.TestResource.ClassContainingTests"));
            Assert.That(test.TestName, Is.EqualTo("AutoTest.TestRunners.MbUnitTests.Tests.TestResource.ClassContainingTests.An_inconclusive_test"));
        }
    }
}
