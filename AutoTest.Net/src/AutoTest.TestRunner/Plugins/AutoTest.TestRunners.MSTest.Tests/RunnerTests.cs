using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared.Options;
using System.IO;

namespace AutoTest.TestRunners.MSTest.Tests
{
    [TestFixture]
    public class RunnerTests
    {
        [Test]
        public void Should_run_all_tests()
        {
            var settings = new RunSettings(new AssemblyOptions(Path.GetFullPath(@"AutoTest.TestRunners.MSTest.Tests.TestResource.dll")), new string[] {}, null);
            var runner = new Runner();
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(11));
        }

        [Test]
        public void Should_run_single_passing_test()
        {
            var assemblyPath = Path.GetFullPath(@"AutoTest.TestRunners.MSTest.Tests.TestResource.dll");
            var assembly = new AssemblyOptions(assemblyPath);
            assembly.AddTest("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture1.Passing_test");
            var settings = new RunSettings(assembly, new string[] { }, null);
            var runner = new Runner();
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(1));
            var test = result.ElementAt(0);
            Assert.That(test.Runner, Is.EqualTo("MSTest"));
            Assert.That(test.Assembly, Is.EqualTo(assemblyPath));
            Assert.That(test.State, Is.EqualTo(Shared.Results.TestState.Passed));
            Assert.That(test.TestFixture, Is.EqualTo("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture1"));
            Assert.That(test.TestName, Is.EqualTo("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture1.Passing_test"));
        }

        [Test]
        public void Should_run_single_failing_test()
        {
            var assemblyPath = Path.GetFullPath(@"AutoTest.TestRunners.MSTest.Tests.TestResource.dll");
            var assembly = new AssemblyOptions(assemblyPath);
            assembly.AddTest("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture1.Failing_test");
            var settings = new RunSettings(assembly, new string[] { }, null);
            var runner = new Runner();
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(1));
            var test = result.ElementAt(0);
            Assert.That(test.Runner, Is.EqualTo("MSTest"));
            Assert.That(test.Assembly, Is.EqualTo(assemblyPath));
            Assert.That(test.State, Is.EqualTo(Shared.Results.TestState.Failed));
            Assert.That(test.TestFixture, Is.EqualTo("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture1"));
            Assert.That(test.TestName, Is.EqualTo("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture1.Failing_test"));
        }

        [Test]
        public void Should_run_single_inconclusive_test()
        {
            var assemblyPath = Path.GetFullPath(@"AutoTest.TestRunners.MSTest.Tests.TestResource.dll");
            var assembly = new AssemblyOptions(assemblyPath);
            assembly.AddTest("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture1.Inconclusive_test");
            var settings = new RunSettings(assembly, new string[] { }, null);
            var runner = new Runner();
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(1));
            var test = result.ElementAt(0);
            Assert.That(test.Runner, Is.EqualTo("MSTest"));
            Assert.That(test.Assembly, Is.EqualTo(assemblyPath));
            Assert.That(test.State, Is.EqualTo(Shared.Results.TestState.Ignored));
            Assert.That(test.TestFixture, Is.EqualTo("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture1"));
            Assert.That(test.TestName, Is.EqualTo("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture1.Inconclusive_test"));
            Assert.That(test.DurationInMilliseconds, Is.GreaterThan(0));
        }

        [Test]
        public void Should_not_run_test_with_ignore_attribute()
        {
            var assemblyPath = Path.GetFullPath(@"AutoTest.TestRunners.MSTest.Tests.TestResource.dll");
            var assembly = new AssemblyOptions(assemblyPath);
            assembly.AddTest("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture1.Ignore_Attrib_test");
            var settings = new RunSettings(assembly, new string[] { }, null);
            var runner = new Runner();
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Should_not_run_class_with_ignore_attribute()
        {
            var assemblyPath = Path.GetFullPath(@"AutoTest.TestRunners.MSTest.Tests.TestResource.dll");
            var assembly = new AssemblyOptions(assemblyPath);
            assembly.AddTest("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture3.SomeTest");
            var settings = new RunSettings(assembly, new string[] { }, null);
            var runner = new Runner();
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Should_run_full_fixture()
        {
            var assemblyPath = Path.GetFullPath(@"AutoTest.TestRunners.MSTest.Tests.TestResource.dll");
            var assembly = new AssemblyOptions(assemblyPath);
            assembly.AddMember("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture2");
            var settings = new RunSettings(assembly, new string[] { }, null);
            var runner = new Runner();
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(2));
            var test = result.ElementAt(0);
            Assert.That(test.TestFixture, Is.EqualTo("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture2"));
            Assert.That(test.TestName, Is.EqualTo("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture2.Another_passing_test"));

            Assert.That(result.ElementAt(1).TestFixture, Is.EqualTo("AutoTest.TestRunners.MSTest.Tests.TestResource.TestFixture2"));
        }

        [Test]
        public void Should_run_test_for_namespace()
        {
            var assemblyPath = Path.GetFullPath(@"AutoTest.TestRunners.MSTest.Tests.TestResource.dll");
            var assembly = new AssemblyOptions(assemblyPath);
            assembly.AddNamespace("AutoTest.TestRunners.MSTest.Tests.TestResource.SomeNamespace");
            var settings = new RunSettings(assembly, new string[] { }, null);
            var runner = new Runner();
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(4));
        }

        [Test]
        public void Should_not_find_tests_on_abstract_classes()
        {
            var assemblyPath = Path.GetFullPath(@"AutoTest.TestRunners.MSTest.Tests.TestResource.dll");
            var runner = new Runner();
            Assert.That(runner.ContainsTestsFor(assemblyPath, "AutoTest.TestRunners.MSTest.Tests.TestResource.AbstractClass"), Is.False);
        }

        [Test]
        public void Should_not_detect_test_on_abstract_classes()
        {
            var assemblyPath = Path.GetFullPath(@"AutoTest.TestRunners.MSTest.Tests.TestResource.dll");
            var runner = new Runner();
            Assert.That(runner.IsTest(assemblyPath, "AutoTest.TestRunners.MSTest.Tests.TestResource.AbstractClass.Test_on_abstract_class"), Is.False);
        }

        [Test]
        public void Should_not_run_tests_on_abstract_classes()
        {
            var assembly = new AssemblyOptions(Path.GetFullPath(@"AutoTest.TestRunners.MSTest.Tests.TestResource.dll"));
            assembly.AddMember("AutoTest.TestRunners.MSTest.Tests.TestResource.AbstractClass");
            var settings = new RunSettings(assembly, new string[] {}, null);
            
            var runner = new Runner();
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Should_find_tests_inherited_from_abstract_classes()
        {
            var assemblyPath = Path.GetFullPath(@"AutoTest.TestRunners.MSTest.Tests.TestResource.dll");
            var runner = new Runner();
            Assert.That(runner.ContainsTestsFor(assemblyPath, "AutoTest.TestRunners.MSTest.Tests.TestResource.InheritingFromAbstractClass"), Is.True);
        }

        [Test]
        public void Should_append_tests_from_abstract_class()
        {
            var assembly = new AssemblyOptions(Path.GetFullPath(@"AutoTest.TestRunners.MSTest.Tests.TestResource.dll"));
            assembly.AddMember("AutoTest.TestRunners.MSTest.Tests.TestResource.InheritingFromAbstractClass");
            var settings = new RunSettings(assembly, new string[] {}, null);
            
            var runner = new Runner();
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(1));
        }
    }
}
