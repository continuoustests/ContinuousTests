using System;
using System.Collections.Generic;
using System.Linq;

using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Messages;

using NUnit.Framework;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class When_the_test_run_generates_report_XML
    {
        [SetUp]
        public void SetUp()
        {
            var run = new MSpecTestRunner.Run
                      {
                          RunInfos = new[]
                                     {
                                         new TestRunInfo(new Project("Machine.Specifications.Example.Random", null),
                                                         @"C:\Some\Path\Machine.Specifications.Example.Random.dll")
                                     }
                      };

            _parser = new MSpecReportParser("TestResources/MSpec/SingleAssembly.xml", run);
            _results = _parser.Parse();
        }

        MSpecReportParser _parser;
        IEnumerable<TestRunResults> _results;

        [Test]
        public void Should_find_the_spec_assembly()
        {
            _results.Count().ShouldEqual(1);
        }

        [Test]
        public void Should_find_the_time_spent()
        {
            Assert.Greater(_results.First().TimeSpent, TimeSpan.Zero);
        }
        
        [Test]
        public void Should_find_all_specs()
        {
            _results.First().All.Length.ShouldEqual(29);
        }

        [Test]
        public void Should_find_succeeded_specs()
        {
            _results.First().Passed.Length.ShouldEqual(17);
        }

        [Test]
        public void Should_find_failed_specs()
        {
            _results.First().Failed.Length.ShouldEqual(4);
        }

        [Test]
        public void Should_pull_the_message_and_stack_trace_for_failed_specs()
        {
            _results.First().Failed.Any(x => String.IsNullOrEmpty(x.Message)).ShouldBeFalse();
            _results.First().Failed.Any(x => x.StackTrace.Length == 0).ShouldBeFalse();
        }

        [Test]
        public void Should_find_ignored_specs()
        {
            _results.First().Ignored.Length.ShouldEqual(8);
        }
        
        [Test]
        public void Should_give_the_ignore_reason_for_unimplemented_specs()
        {
            _results.First().Ignored.Any(x => x.Message == "not implemented").ShouldBeTrue();
        }
    }

    [TestFixture]
    public class When_the_test_run_generates_report_XML_for_two_assemblies
    {
        [SetUp]
        public void SetUp()
        {
            var run = new MSpecTestRunner.Run
                      {
                          RunInfos = new[]
                                     {
                                         new TestRunInfo(new Project("Machine.Specifications.Example.Random", null),
                                                         @"C:\Some\Path\Machine.Specifications.Example.Random.dll"),
                                                         new TestRunInfo(new Project("Machine.Specifications.FailingExample", null),
                                                         @"C:\Some\Path\Machine.Specifications.FailingExample.dll")
                                     }
                      };

            _parser = new MSpecReportParser("TestResources/MSpec/TwoAssemblies.xml", run);
            _results = _parser.Parse();
        }

        MSpecReportParser _parser;
        IEnumerable<TestRunResults> _results;

        [Test]
        public void Should_find_two_spec_assemblies()
        {
            _results.Count().ShouldEqual(2);
        }
        
        [Test]
        public void Should_find_the_time_spent_for_each_assembly()
        {
            _results.ToList().ForEach(x => Assert.Greater(x.TimeSpent, TimeSpan.Zero));
        }

        [Test]
        public void Should_find_the_assembly_name()
        {
            _results.First().Assembly.ShouldEqual("C:\\Some\\Path\\Machine.Specifications.Example.Random.dll");
        }
    }

    [TestFixture]
    public class When_the_test_run_generates_report_XML_for_an_unknown_assembly
    {
        [SetUp]
        public void SetUp()
        {
            var run = new MSpecTestRunner.Run
                      {
                          RunInfos = new[]
                                     {
                                         new TestRunInfo(new Project("Machine.Specifications.Example.Random", null),
                                                         @"C:\Path\That\Is\Not\Reported")
                                     }
                      };

            _parser = new MSpecReportParser("TestResources/MSpec/SingleAssembly.xml", run);
            _results = _parser.Parse();
        }

        MSpecReportParser _parser;
        IEnumerable<TestRunResults> _results;

        [Test]
        public void Should_find_no_spec_assemblies()
        {
            _results.Count().ShouldEqual(0);
        }
    }
}