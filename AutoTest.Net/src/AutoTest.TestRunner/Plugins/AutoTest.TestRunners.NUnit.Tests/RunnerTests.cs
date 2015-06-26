using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.NUnit;
using System.IO;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Plugins;
using AutoTest.TestRunners.Shared.Results;
using System.Reflection;

namespace AutoTest.TestRunners.NUnit.Tests
{
    [TestFixture]
    public class RunnerTests
    {
        private Plugin _plugin = null;
        private IAutoTestNetTestRunner _runner;

        [TestFixtureSetUp]
        public void SetUp()
        {
            int a = 3;
            _plugin = new Plugin(typeof(Runner).Assembly.Location, typeof(Runner).FullName);
            _runner = _plugin.New();
        }

        [Test]
        public void Should_recognize_test()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var runner = new Runner();
            var assembly = Path.Combine(path, "AutoTest.TestRunners.NUnit.Tests.TestResource.dll");
            var method = "AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1.Should_pass";
            Assert.That(runner.IsTest(assembly, method), Is.True);
        }

        [Test]
        public void Should_recognize_testcase()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var runner = new Runner();
            var assembly = Path.Combine(path, "AutoTest.TestRunners.NUnit.Tests.TestResource.dll");
            var method = "AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1.Should_fail";
            Assert.That(runner.IsTest(assembly, method), Is.True);
        }

        [Test]
        public void Should_recognize_fixture()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var runner = new Runner();
            var assembly = Path.Combine(path, "AutoTest.TestRunners.NUnit.Tests.TestResource.dll");
            var cls = "AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1";
            Assert.That(runner.ContainsTestsFor(assembly, cls), Is.True);
        }

        [Test]
        public void Should_recognize_inherited_fixture()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var runner = new Runner();
            var assembly = Path.Combine(path, "AutoTest.TestRunners.NUnit.Tests.TestResource.dll");
            var cls = "AutoTest.TestRunners.NUnit.Tests.TestResource.InheritedFixture";
            Assert.That(runner.ContainsTestsFor(assembly, cls), Is.True);
        }

        [Test]
        public void Should_contain_tests_for()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var runner = new Runner();
            var assembly = Path.Combine(path, "AutoTest.TestRunners.NUnit.Tests.TestResource.dll");
            Assert.That(runner.ContainsTestsFor(assembly), Is.True);
        }

        [Test]
        public void Should_recognize_nested_test_fixtures()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var runner = new Runner();
            var assembly = Path.Combine(path, "AutoTest.TestRunners.NUnit.Tests.TestResource.dll");
            var method = "AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1+NestedFixture.Nested_test";
            Assert.That(runner.IsTest(assembly, method), Is.True);
        }

        [Test]
        public void Should_run_test()
        {
            var assembly = new AssemblyOptions(
                Path.GetFullPath(@"AutoTest.TestRunners.NUnit.Tests.TestResource.dll")
            );
            var options = new RunSettings(assembly, new string[] {}, "feedback_pipe");
            var results = _runner.Run(options);
            Assert.That(results.Count(), Is.EqualTo(9));

            Assert.That(results.ElementAt(0).Assembly, Is.EqualTo(Path.GetFullPath(@"AutoTest.TestRunners.NUnit.Tests.TestResource.dll")));
            Assert.That(results.ElementAt(0).TestFixture, Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1"));
            Assert.That(results.ElementAt(0).State, Is.EqualTo(Shared.Results.TestState.Failed));
            Assert.That(results.ElementAt(0).TestName, Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1.Should_fail()"));
            Assert.That(results.ElementAt(0).StackLines.Count(), Is.EqualTo(1));
            Assert.That(results.ElementAt(0).StackLines.ElementAt(0).Method.Replace(" ()", "()"), Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1.Should_fail()"));

            Assert.That(results.ElementAt(1).Assembly, Is.EqualTo(Path.GetFullPath(@"AutoTest.TestRunners.NUnit.Tests.TestResource.dll")));
            Assert.That(results.ElementAt(1).TestFixture, Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1"));
            Assert.That(results.ElementAt(1).State, Is.EqualTo(Shared.Results.TestState.Ignored));
            Assert.That(results.ElementAt(1).TestName, Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1.Should_ignore"));
            Assert.That(results.ElementAt(1).StackLines.Count(), Is.EqualTo(1));
            Assert.That(results.ElementAt(1).StackLines.ElementAt(0).Method.Replace(" ()", "()"), Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1.Should_ignore()"));

            Assert.That(results.ElementAt(2).Assembly, Is.EqualTo(Path.GetFullPath(@"AutoTest.TestRunners.NUnit.Tests.TestResource.dll")));
            Assert.That(results.ElementAt(2).TestFixture, Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1"));
            Assert.That(results.ElementAt(2).State, Is.EqualTo(Shared.Results.TestState.Passed));
            Assert.That(results.ElementAt(2).TestName, Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1.Should_pass"));
            Assert.That(results.ElementAt(2).StackLines.Count(), Is.EqualTo(0));

            // For some reason NUnit can't seems to locate filename and line number on windows when stacktrace only shows method name
            if (OS.IsPosix)
            {
                Assert.IsTrue(File.Exists(results.ElementAt(0).StackLines.ElementAt(0).File), "Did not find " + results.ElementAt(0).StackLines.ElementAt(0).File);
                Assert.That(results.ElementAt(0).StackLines.ElementAt(0).Line, Is.EqualTo(22));

                Assert.IsTrue(File.Exists(results.ElementAt(0).StackLines.ElementAt(0).File), "Did not find " + results.ElementAt(0).StackLines.ElementAt(0).File);
                Assert.That(results.ElementAt(1).StackLines.ElementAt(0).Line, Is.EqualTo(29));
            }
        }

        [Test]
        public void Should_run_tests_from_type()
        {
            var assembly = new AssemblyOptions(
                Path.GetFullPath(@"AutoTest.TestRunners.NUnit.Tests.TestResource.dll")
            );
            assembly.AddMember("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture2");
            var options = new RunSettings(assembly, new string[] {}, "feedback_pipe");
            var results = _runner.Run(options);
            Assert.That(results.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Should_run_tests_from_namespace()
        {
            var assembly = new AssemblyOptions(
                Path.GetFullPath(@"AutoTest.TestRunners.NUnit.Tests.TestResource.dll")
            );
            assembly.AddNamespace("AutoTest.TestRunners.NUnit.Tests.TestResource");
            var options = new RunSettings(assembly, new string[] {}, "feedback_pipe");
            var results = _runner.Run(options);
            Assert.That(results.Count(), Is.EqualTo(9));
        }

        [Test]
        public void Should_run_single_test_and_type()
        {
            var assembly = new AssemblyOptions(
                Path.GetFullPath(@"AutoTest.TestRunners.NUnit.Tests.TestResource.dll")
            );
            assembly.AddTest("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1.Should_ignore");
            assembly.AddMember("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture2");
            var options = new RunSettings(assembly, new string[] {}, "feedback_pipe");
            var results = _runner.Run(options);
            Assert.That(results.Count(), Is.EqualTo(3));
        }
    }
}
