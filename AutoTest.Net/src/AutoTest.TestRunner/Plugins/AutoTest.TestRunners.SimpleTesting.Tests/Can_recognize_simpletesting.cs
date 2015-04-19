using System.IO;
using System.Linq;
using System.Reflection;
using AutoTest.TestRunners.Shared.Options;
using NUnit.Framework;
using TestState = AutoTest.TestRunners.Shared.Results.TestState;

namespace AutoTest.TestRunners.SimpleTesting.Tests
{

    [Category("slow")]
    [TestFixture]
    public class Can_recognize_simpletesting
    {
        [Test]
        public void I_can_haz_simpletesting_test()
        {
            var runner = new Runner();
            Assert.That(runner.IsTest(getAssembly(), "AutoTest.TestRunners.SimpleTesting.Tests.Resources.SimpleTestingTests.a_passing_test"), Is.True);
        }

        [Test]
        public void When_handed_a_non_simpletesting_test_it_will_not_recognize_it()
        {
            var runner = new Runner();
            Assert.That(runner.IsTest(getAssembly(), "AutoTest.TestRunners.SimpleTesting.Tests.Resources.SomeClass.Foo"), Is.False);
        }

        [Test]
        public void When_handed_an_assembly_that_contains_simpletesting_tests_it_will_report_that_it_can_handle_the_assembly()
        {
            var runner = new Runner();
            Assert.That(runner.ContainsTestsFor(getAssembly()), Is.True);
        }

        [Test]
        public void When_passed_a_class_containing_simpletesting_test_fields_it_will_report_that_it_can_handle_it()
        {
            var runner = new Runner();
            Assert.That(runner.ContainsTestsFor(getAssembly(), "AutoTest.TestRunners.SimpleTesting.Tests.Resources.SimpleTestingTests"), Is.True);
        }

        [Test]
        public void Will_respond_to_simpletesting_identifier()
        {
            var runner = new Runner();
            Assert.That(runner.Handles("SimpleTesting"), Is.True);
        }

        [Test]
        public void Will_respond_to_simpletesting_identifier_different_casing()
        {
            var runner = new Runner();
            Assert.That(runner.Handles("SimpLetestinG"), Is.True);
        }


        [Test]
        public void When_told_to_run_all_tests_it_reports_all_tests()
        {
            var runner = new Runner();
            var options = new AssemblyOptions(getAssembly());
            var settings = new RunSettings(options, new string[] { }, null);
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(5));
        }

        [Test]
        public void when_told_to_run_all_on_type_it_runs_all_on_type()
        {
            var runner = new Runner();
            var options = new AssemblyOptions(getAssembly());
            options.AddMember("AutoTest.TestRunners.SimpleTesting.Tests.Resources.SimpleTestingTests");
            var settings = new RunSettings(options, new string[] { }, null);
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public void when_running_named_test_that_passes()
        {
            var runner = new Runner();
            var options = new AssemblyOptions(getAssembly());
            options.AddTest("AutoTest.TestRunners.SimpleTesting.Tests.Resources.SimpleTestingTests.a_passing_test");
            var settings = new RunSettings(options, new string[] { }, null);
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().State == TestState.Passed);
            Assert.That(!result.First().StackLines.Any());
            Assert.That(result.First().TestDisplayName == "a_passing_test");
            Assert.That(result.First().Runner == "SimpleTesting");
            Assert.AreEqual("AutoTest.TestRunners.SimpleTesting.Tests.Resources.SimpleTestingTests.a_passing_test", result.First().TestName);
            Assert.AreEqual("SimpleTestingTests", result.First().TestFixture);
        }

        [Test]
        public void when_running_named_test_that_fails_without_exception()
        {
            var runner = new Runner();
            var options = new AssemblyOptions(getAssembly());
            options.AddTest("AutoTest.TestRunners.SimpleTesting.Tests.Resources.SimpleTestingTests.a_failing_test");
            var settings = new RunSettings(options, new string[] { }, null);
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().State == TestState.Failed);
            Assert.That(!result.First().StackLines.Any());
            Assert.That(result.First().TestDisplayName == "a_failing_test");
            Assert.That(result.First().Runner == "SimpleTesting");
            Assert.AreEqual("AutoTest.TestRunners.SimpleTesting.Tests.Resources.SimpleTestingTests.a_failing_test", result.First().TestName);
            Assert.AreEqual("SimpleTestingTests", result.First().TestFixture);
        }

        [Test]
        public void when_running_named_test_that_fails_with_exception()
        {
            var runner = new Runner();
            var options = new AssemblyOptions(getAssembly());
            options.AddTest("AutoTest.TestRunners.SimpleTesting.Tests.Resources.SimpleTestingTests.a_failing_test_with_exception");
            var settings = new RunSettings(options, new string[] { }, null);
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().State == TestState.Failed);
            Assert.That(result.First().StackLines.Any());
            Assert.That(result.First().TestDisplayName == "a_failing_test_with_exception");
        }

        [Test]
        public void can_all_tests_in_a_class()
        {
            var runner = new Runner();
            var options = new AssemblyOptions(getAssembly());
            options.AddMember("AutoTest.TestRunners.SimpleTesting.Tests.Resources.SimpleTestingTests");
            var settings = new RunSettings(options, new string[] { }, null);
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]                                                                                       
        public void can_all_tests_in_a_nested_class()
        {
            var runner = new Runner();
            var options = new AssemblyOptions(getAssembly());
            options.AddMember("AutoTest.TestRunners.SimpleTesting.Tests.Resources.more.MoreSimpleTestingTests+Nested");
            var settings = new RunSettings(options, new string[] { }, null);
            var result = runner.Run(settings);
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        //[Test]
        //public void When_told_to_run_a_test_that_passes_it_reports_a_passing_result()
        //{
        //    var runner = new Runner();
        //    var options = new AssemblyOptions(getAssembly());
        //    options.AddTest("AutoTest.TestRunners.MSpec.Tests.TestResource.Can_add_numbers");
        //    var settings = new RunSettings(options, new string[] { }, null);
        //    var result = runner.Run(settings);
        //    Assert.That(result.Count(), Is.EqualTo(1));
        //    var test = result.ElementAt(0);
        //    Assert.That(test.State, Is.EqualTo(Shared.Results.TestState.Passed));
        //    Assert.That(test.Assembly, Is.EqualTo(getAssembly()));
        //    Assert.That(test.Runner, Is.EqualTo("MSpec"));
        //    Assert.That(test.TestFixture, Is.EqualTo("AutoTest.TestRunners.MSpec.Tests.TestResource.Can_add_numbers"));
        //    Assert.That(test.TestName, Is.EqualTo("AutoTest.TestRunners.MSpec.Tests.TestResource.Can_add_numbers"));
        //    Assert.That(test.TestDisplayName, Is.EqualTo("AutoTest.TestRunners.MSpec.Tests.TestResource.Can_add_numbers"));
        //}

        //[Test]
        //public void When_told_to_run_a_test_that_is_ignored_it_reports_a_ignored_result()
        //{
        //    var runner = new Runner();
        //    var options = new AssemblyOptions(getAssembly());
        //    options.AddTest("AutoTest.TestRunners.MSpec.Tests.TestResource.Ignored_test");
        //    var settings = new RunSettings(options, new string[] { }, null);
        //    var result = runner.Run(settings);
        //    Assert.That(result.Count(), Is.EqualTo(1));
        //    var test = result.ElementAt(0);
        //    Assert.That(test.State, Is.EqualTo(Shared.Results.TestState.Ignored));
        //    Assert.That(test.Assembly, Is.EqualTo(getAssembly()));
        //    Assert.That(test.Runner, Is.EqualTo("MSpec"));
        //    Assert.That(test.TestFixture, Is.EqualTo("AutoTest.TestRunners.MSpec.Tests.TestResource.Ignored_test"));
        //    Assert.That(test.TestName, Is.EqualTo("AutoTest.TestRunners.MSpec.Tests.TestResource.Ignored_test"));
        //}

        //[Test]
        //public void When_told_to_run_a_test_that_is_not_implemented_it_reports_a_ignored_result()
        //{
        //    var runner = new Runner();
        //    var options = new AssemblyOptions(getAssembly());
        //    options.AddTest("AutoTest.TestRunners.MSpec.Tests.TestResource.Not_implemented_test");
        //    var settings = new RunSettings(options, new string[] { }, null);
        //    var result = runner.Run(settings);
        //    Assert.That(result.Count(), Is.EqualTo(1));
        //    var test = result.ElementAt(0);
        //    Assert.That(test.State, Is.EqualTo(Shared.Results.TestState.Ignored));
        //    Assert.That(test.Assembly, Is.EqualTo(getAssembly()));
        //    Assert.That(test.Runner, Is.EqualTo("MSpec"));
        //    Assert.That(test.TestFixture, Is.EqualTo("AutoTest.TestRunners.MSpec.Tests.TestResource.Not_implemented_test"));
        //    Assert.That(test.TestName, Is.EqualTo("AutoTest.TestRunners.MSpec.Tests.TestResource.Not_implemented_test"));
        //}

        //[Test]
        //public void When_told_to_run_a_test_that_fails_it_reports_a_failed_result()
        //{
        //    var runner = new Runner();
        //    var options = new AssemblyOptions(getAssembly());
        //    options.AddTest("AutoTest.TestRunners.MSpec.Tests.TestResource.Can_not_add_numbers");
        //    var settings = new RunSettings(options, new string[] { }, null);
        //    var result = runner.Run(settings);
        //    Assert.That(result.Count(), Is.EqualTo(1));
        //    var test = result.ElementAt(0);
        //    Assert.That(test.State, Is.EqualTo(Shared.Results.TestState.Failed));
        //    Assert.That(test.Assembly, Is.EqualTo(getAssembly()));
        //    Assert.That(test.Runner, Is.EqualTo("MSpec"));
        //    Assert.That(test.TestFixture, Is.EqualTo("AutoTest.TestRunners.MSpec.Tests.TestResource.Can_not_add_numbers"));
        //    Assert.That(test.TestName, Is.EqualTo("AutoTest.TestRunners.MSpec.Tests.TestResource.Can_not_add_numbers"));
        //    Assert.That(test.Message, Is.EqualTo("I'm failing here"));
        //    Assert.That(test.StackLines.Count(), Is.EqualTo(2));
        //}

        //[Test]
        //public void When_told_to_run_a_member_it_reports_all_tests_in_member_which_for_mspec_is_a_single_test()
        //{
        //    var runner = new Runner();
        //    var options = new AssemblyOptions(getAssembly());
        //    options.AddMember("AutoTest.TestRunners.MSpec.Tests.TestResource.Can_add_numbers");
        //    var settings = new RunSettings(options, new string[] { }, null);
        //    var result = runner.Run(settings);
        //    Assert.That(result.Count(), Is.EqualTo(1));
        //    var test = result.ElementAt(0);
        //    Assert.That(test.State, Is.EqualTo(Shared.Results.TestState.Passed));
        //    Assert.That(test.Assembly, Is.EqualTo(getAssembly()));
        //    Assert.That(test.Runner, Is.EqualTo("MSpec"));
        //    Assert.That(test.TestFixture, Is.EqualTo("AutoTest.TestRunners.MSpec.Tests.TestResource.Can_add_numbers"));
        //    Assert.That(test.TestName, Is.EqualTo("AutoTest.TestRunners.MSpec.Tests.TestResource.Can_add_numbers"));
        //}

        //[Test]
        //public void When_told_to_run_a_namespace_it_reports_all_tests_in_namespace()
        //{
        //    var runner = new Runner();
        //    var options = new AssemblyOptions(getAssembly());
        //    options.AddNamespace("AutoTest.TestRunners.MSpec.Tests.TestResource");
        //    var settings = new RunSettings(options, new string[] { }, null);
        //    var result = runner.Run(settings);
        //    Assert.That(result.Count(), Is.EqualTo(4));
        //}


        private static string getAssembly()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "AutoTest.TestRunners.SimpleTesting.Tests.Resources.dll");
        }
    }

}
