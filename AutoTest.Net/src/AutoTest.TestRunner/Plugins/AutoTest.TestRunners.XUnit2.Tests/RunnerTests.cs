using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Plugins;
using AutoTest.TestRunners.Shared.Results;
using NUnit.Framework;

namespace AutoTest.TestRunners.XUnit2.Tests
{
    interface ITestRunner
    {
        IEnumerable<TestResult> Run(Plugin plugin, RunSettings settings);
    }

    class TestRunner : MarshalByRefObject, ITestRunner
    {
        public IEnumerable<TestResult> Run(Plugin plugin, RunSettings settings)
        {
            return plugin.New().Run(settings);
        }
    }

    [TestFixture]
    public class RunnerTests
    {
        private Plugin _plugin = null;
        private ITestRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _plugin = new Plugin(typeof(Runner).Assembly.Location, typeof(Runner).FullName);
            _runner = new TestRunner();
        }

        [Test]
        public void Should_identify_test()
        {
            var runner = new Runner();
            var assembly = getPath("AutoTest.TestRunners.XUnit2.Tests.TestResource.dll");
            var method = "AutoTest.TestRunners.XUnit2.Tests.TestResource.Class1.Should_pass";
            Assert.That(runner.IsTest(assembly, method), Is.True);
        }

        [Test]
        public void Should_identify_test_container()
        {
            var runner = new Runner();
            var assembly = getPath("AutoTest.TestRunners.XUnit2.Tests.TestResource.dll");
            var cls = "AutoTest.TestRunners.XUnit2.Tests.TestResource.Class1";
            Assert.That(runner.ContainsTestsFor(assembly, cls), Is.True);
        }

        [Test]
        public void Should_contain_tests_for()
        {
            var runner = new Runner();
            var assembly = getPath("AutoTest.TestRunners.XUnit2.Tests.TestResource.dll");
            Assert.That(runner.ContainsTestsFor(assembly), Is.True);
        }

        private string getPath(string relativePath)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(path, relativePath);
        }

        [Test]
        public void Should_run_tests()
        {
            var options = new RunSettings(new AssemblyOptions(Path.GetFullPath(@"AutoTest.TestRunners.XUnit.Tests.TestResource.dll")), new string[0], null);

            var result = _runner.Run(_plugin, options);

            Assert.That(result.Count(), Is.EqualTo(7));
            var test1 = result.Where(x => x.TestName.Equals("AutoTest.TestRunners.XUnit.Tests.TestResource.Class1.Should_pass")).First();
            Assert.That(test1.Runner, Is.EqualTo("XUnit"));
            Assert.That(test1.Assembly, Is.EqualTo(options.Assembly));
            Assert.That(test1.TestFixture, Is.EqualTo(""));
            Assert.That(test1.DurationInMilliseconds, Is.GreaterThan(0));
            Assert.That(test1.TestName, Is.EqualTo("AutoTest.TestRunners.XUnit.Tests.TestResource.Class1.Should_pass"));
            Assert.That(test1.State, Is.EqualTo(Shared.Results.TestState.Passed));

            var test2 = result.Where(x => x.TestName.Equals("AutoTest.TestRunners.XUnit.Tests.TestResource.Class1.Should_fail")).First();
            Assert.That(test2.Runner, Is.EqualTo("XUnit2"));
            Assert.That(test2.Assembly, Is.EqualTo(options.Assembly));
            Assert.That(test2.TestFixture, Is.EqualTo(""));
            Assert.That(test2.TestName, Is.EqualTo("AutoTest.TestRunners.XUnit.Tests.TestResource.Class1.Should_fail"));
            Assert.That(test2.State, Is.EqualTo(Shared.Results.TestState.Failed));
            Assert.That(test2.StackLines.Count(), Is.EqualTo(1));
            Assert.That(test2.StackLines.ElementAt(0).Method, Is.EqualTo("AutoTest.TestRunners.XUnit.Tests.TestResource.Class1.Should_fail()"));
            // Only verify if build with debug
            if (test2.StackLines.ElementAt(0).File.Length > 0)
            {
                Assert.IsTrue(File.Exists(test2.StackLines.ElementAt(0).File));
                Assert.That(test2.StackLines.ElementAt(0).Line, Is.EqualTo(23));
            }
        }

        [Test]
        public void Should_run_single_test()
        {
            var assemblyOptions = new AssemblyOptions(Path.GetFullPath(@"AutoTest.TestRunners.XUnit.Tests.TestResource.dll"));
            assemblyOptions.AddTest("AutoTest.TestRunners.XUnit.Tests.TestResource.Class1.Should_pass");

            var options = new RunSettings(assemblyOptions, new string[0], null);

            var result = _runner.Run(_plugin, options);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Should_run_members()
        {
            var assemblyOptions = new AssemblyOptions(Path.GetFullPath(@"AutoTest.TestRunners.XUnit.Tests.TestResource.dll"));
            assemblyOptions.AddMember("AutoTest.TestRunners.XUnit.Tests.TestResource.Class1");

            var options = new RunSettings(assemblyOptions, new string[0], null);

            var result = _runner.Run(_plugin, options);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Should_run_namespace()
        {
            var assemblyOptions = new AssemblyOptions(Path.GetFullPath(@"AutoTest.TestRunners.XUnit.Tests.TestResource.dll"));
            assemblyOptions.AddNamespace("AutoTest.TestRunners.XUnit.Tests.TestResource.Anothernamespace");

            var options = new RunSettings(assemblyOptions, new string[0], null);

            var result = _runner.Run(_plugin, options);

            Assert.That(result.Count(), Is.EqualTo(4));
        }
    }
}
