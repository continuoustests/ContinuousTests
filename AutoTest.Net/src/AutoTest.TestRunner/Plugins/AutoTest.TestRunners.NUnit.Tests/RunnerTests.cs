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
    interface ITestRunner
    {
        IEnumerable<TestResult> Run(Plugin plugin, RunSettings options);
    }

    class TestRunner : MarshalByRefObject, ITestRunner
    {
        public IEnumerable<TestResult> Run(Plugin plugin, RunSettings options)
        {
            return plugin.New().Run(options);
        }
    }

    [TestFixture]
    public class RunnerTests
    {
        //private AppDomain _childDomain = null;
        //private Plugin _plugin = null;
        //private ITestRunner _runner;

        //[TestFixtureSetUp]
        //public void SetUp()
        //{
        //    AppDomainSetup domainSetup = new AppDomainSetup()
        //    {
        //        ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
        //        ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
        //        ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName,
        //        LoaderOptimization = LoaderOptimization.MultiDomainHost
        //    };
        //    _childDomain = AppDomain.CreateDomain("NUnit app domain", null, domainSetup);
        //    _plugin = new Plugin(typeof(Runner).Assembly.Location, typeof(Runner).FullName);
        //    _runner = (ITestRunner)_childDomain.CreateInstanceAndUnwrap(typeof(TestRunner).Assembly.FullName, typeof(TestRunner).FullName);
        //}

        //[TestFixtureTearDown]
        //public void Teardown()
        //{
        //    if (_childDomain != null)
        //        AppDomain.Unload(_childDomain);
        //}

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

        //[Test]
        //public void Should_run_test()
        //{
        //    var options = new RunnerOptions("nunit");
        //    options.AddAssembly(new AssemblyOptions(
        //        Path.GetFullPath(@"AutoTest.TestRunners.NUnit.Tests.TestResource.dll")));
        //    var results = _runner.Run(_plugin, options);i
        //    Assert.That(results.Count(), Is.EqualTo(8));

        //    Assert.That(results.ElementAt(0).Assembly, Is.EqualTo(Path.GetFullPath(@"AutoTest.TestRunners.NUnit.Tests.TestResource.dll")));
        //    Assert.That(results.ElementAt(0).TestFixture, Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1"));
        //    Assert.That(results.ElementAt(0).State, Is.EqualTo(Shared.Results.TestState.Failed));
        //    Assert.That(results.ElementAt(0).TestName, Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1.Should_fail()"));
        //    Assert.That(results.ElementAt(0).StackLines.Count(), Is.EqualTo(1));
        //    Assert.That(results.ElementAt(0).StackLines.ElementAt(0).Method.Replace(" ()", "()"), Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1.Should_fail()"));

        //    Assert.That(results.ElementAt(1).Assembly, Is.EqualTo(Path.GetFullPath(@"AutoTest.TestRunners.NUnit.Tests.TestResource.dll")));
        //    Assert.That(results.ElementAt(1).TestFixture, Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1"));
        //    Assert.That(results.ElementAt(1).State, Is.EqualTo(Shared.Results.TestState.Ignored));
        //    Assert.That(results.ElementAt(1).TestName, Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1.Should_ignore"));
        //    Assert.That(results.ElementAt(1).StackLines.Count(), Is.EqualTo(1));
        //    Assert.That(results.ElementAt(1).StackLines.ElementAt(0).Method.Replace(" ()", "()"), Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1.Should_ignore()"));

        //    Assert.That(results.ElementAt(2).Assembly, Is.EqualTo(Path.GetFullPath(@"AutoTest.TestRunners.NUnit.Tests.TestResource.dll")));
        //    Assert.That(results.ElementAt(2).TestFixture, Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1"));
        //    Assert.That(results.ElementAt(2).State, Is.EqualTo(Shared.Results.TestState.Passed));
        //    Assert.That(results.ElementAt(2).TestName, Is.EqualTo("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1.Should_pass"));
        //    Assert.That(results.ElementAt(2).StackLines.Count(), Is.EqualTo(0));

        //    // For some reason NUnit can't seems to locate filename and line number on windows when stacktrace only shows method name
        //    if (OS.IsPosix)
        //    {
        //        Assert.IsTrue(File.Exists(results.ElementAt(0).StackLines.ElementAt(0).File), "Did not find " + results.ElementAt(0).StackLines.ElementAt(0).File);
        //        Assert.That(results.ElementAt(0).StackLines.ElementAt(0).Line, Is.EqualTo(23));

        //        Assert.IsTrue(File.Exists(results.ElementAt(0).StackLines.ElementAt(0).File), "Did not find " + results.ElementAt(0).StackLines.ElementAt(0).File);
        //        Assert.That(results.ElementAt(1).StackLines.ElementAt(0).Line, Is.EqualTo(30));
        //    }
        //}

        //[Test]
        //public void Should_run_tests_from_type()
        //{
        //    var options = new RunnerOptions("nunit");
        //    options.AddAssembly(new AssemblyOptions(
        //        Path.GetFullPath(@"AutoTest.TestRunners.NUnit.Tests.TestResource.dll")));
        //    options.Assemblies.ElementAt(0).AddMember("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture2");
        //    var results = _runner.Run(_plugin, options);
        //    Assert.That(results.Count(), Is.EqualTo(2));
        //}

        //[Test]
        //public void Should_run_tests_from_namespace()
        //{
        //    var options = new RunnerOptions("nunit");
        //    options.AddAssembly(new AssemblyOptions(
        //        Path.GetFullPath(@"AutoTest.TestRunners.NUnit.Tests.TestResource.dll")));
        //    options.Assemblies.ElementAt(0).AddNamespace("AutoTest.TestRunners.NUnit.Tests.TestResource");
        //    var results = _runner.Run(_plugin, options);
        //    Assert.That(results.Count(), Is.EqualTo(5));
        //}

        //[Test]
        //public void Should_run_single_test_and_type()
        //{
        //    var options = new RunnerOptions("nunit");
        //    options.AddAssembly(new AssemblyOptions(
        //        Path.GetFullPath(@"AutoTest.TestRunners.NUnit.Tests.TestResource.dll")));
        //    options.Assemblies.ElementAt(0).AddTest("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture1.Should_ignore");
        //    options.Assemblies.ElementAt(0).AddMember("AutoTest.TestRunners.NUnit.Tests.TestResource.Fixture2");
        //    var results = _runner.Run(_plugin, options);
        //    Assert.That(results.Count(), Is.EqualTo(3));
        //}
    }
}
