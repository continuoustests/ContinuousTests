using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using celer.Core.TestRunners;
using celer.Core;
using celer.Tests.MSTestData;
using System.IO;
using System.Reflection;

namespace celer.Tests
{
    [TestFixture]
    public class MSTestTestRunnerTests
    {
        private static ITestRunner GetRunnerFor<T>(string name)
        {
            return new MSTestTestRunner(getMethod<T>(name));
        }

        private static RunResult RunTest<T>(string name)
        {
            var runner = GetRunnerFor<T>(name);
            return runner.Run();
        }

        private static System.Reflection.MethodInfo getMethod<T>(string name)
        {
            var method = typeof(T).GetMethod(name);
            if (method == null)
                throw new MethodNotFoundException("method " + name + " not found on " + typeof(T).Name);
            return method;
        }

        [Test]
        public void Simple_test_with_passing_assert()
        {
            var result = RunTest<MSTestFixture>("Simple_passing_test");
            Assert.That(result.Exception, Is.Null);
            Assert.That(result.Passed, Is.True);
            Assert.That(result.WasRun, Is.True);
            Assert.That(result.Test.Name, Is.EqualTo("Simple_passing_test"));
        }

        [Test]
        public void Simple_test_with_unhandled_exception()
        {
            var result = RunTest<MSTestFixture>("Simple_test_throwing_exception");
            Assert.That(result.Exception, Is.Not.Null);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.WasRun, Is.True);
            Assert.That(result.Test.Name, Is.EqualTo("Simple_test_throwing_exception"));
        }

        [Test]
        public void Simple_test_with_failing_assert()
        {
            var result = RunTest<MSTestFixture>("Simple_failing_test");
            Assert.That(result.Exception, Is.Not.Null);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.WasRun, Is.True);
            Assert.That(result.Test.Name, Is.EqualTo("Simple_failing_test"));
        }

        [Test]
        public void Simple_test_with_inconclusive_assert()
        {
            var result = RunTest<MSTestFixture>("Simple_inconclusive_test");
            Assert.That(result.Exception, Is.Not.Null);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.WasRun, Is.False);
            Assert.That(result.Test.Name, Is.EqualTo("Simple_inconclusive_test"));
        }

        [Test]
        public void Simple_test_with_expected_exception()
        {
            var result = RunTest<MSTestFixture>("Test_excpecting_exception");
            Assert.That(result.Exception, Is.Not.Null);
            Assert.That(result.Passed, Is.True);
            Assert.That(result.WasRun, Is.True);
            Assert.That(result.Test.Name, Is.EqualTo("Test_excpecting_exception"));
        }

        [Test]
        public void Simple_test_with_expected_exception_but_no_exception_thrown()
        {
            var result = RunTest<MSTestFixture>("Test_excpecting_exception_but_not_getting_one");
            Assert.That(result.Exception, Is.Null);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.WasRun, Is.True);
            Assert.That(result.Test.Name, Is.EqualTo("Test_excpecting_exception_but_not_getting_one"));
        }

        [Test]
        public void Runs_class_setup_and_teardown()
        {
            var fixture = GetRunnerFor<MSTestFixtureWithClassSetupAndTearDown>("class_init_teardown_test1");
            MSTestTestRunner.RunClassSetup(typeof(MSTestFixtureWithClassSetupAndTearDown));
            var result = fixture.Run();
            MSTestTestRunner.RunClassTeardown(typeof(MSTestFixtureWithClassSetupAndTearDown));
            Assert.That(result.Exception, Is.Null);
            Assert.That(result.Passed, Is.True);
            Assert.That(result.WasRun, Is.True);
            Assert.That(result.Test.Name, Is.EqualTo("class_init_teardown_test1"));
        }

        [Test]
        public void Runs_test_setup_and_teardown()
        {
            var result = RunTest<MSTestFixtureWithClassSetupAndTearDown>("test_setup_teardown");
            Assert.That(result.Exception, Is.Null);
            Assert.That(result.Passed, Is.True);
            Assert.That(result.WasRun, Is.True);
            Assert.That(result.Test.Name, Is.EqualTo("test_setup_teardown"));
        }

        [Test]
        public void Runs_test_failing_setup()
        {
            var result = RunTest<MSTestFixtureWithFailingClassSetupAndTearDown>("class_init_teardown_test1");
            Assert.That(result.Exception, Is.Not.Null);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.WasRun, Is.True);
            Assert.That(result.Test.Name, Is.EqualTo("class_init_teardown_test1"));
        }

        [Test]
        public void Should_handle_deployment_item()
        {
            var result = RunTest<MSTestFixture>("Test_with_deployment_item");
            Assert.That(result.Exception, Is.Null);
            Assert.That(result.Passed, Is.True);
            Assert.That(result.WasRun, Is.True);
            Assert.That(result.Test.Name, Is.EqualTo("Test_with_deployment_item"));
            Assert.That(File.Exists("SomeFile.txt"), Is.True);
        }

        [Test]
        public void Should_handle_deployment_item_with_custom_output_path()
        {
            var result = RunTest<MSTestFixture>("Test_with_deployment_item_with_custom_output");
            Assert.That(result.Exception, Is.Null);
            Assert.That(result.Passed, Is.True);
            Assert.That(result.WasRun, Is.True);
            Assert.That(result.Test.Name, Is.EqualTo("Test_with_deployment_item_with_custom_output"));
            Assert.That(File.Exists(Path.Combine("CustomOutput", "SomeFile.txt")), Is.True);
        }
    }
}
