using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Caching.RunResultCache;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class RemovedTestsLocatorTest
    {
        [Test]
        public void Should_locate_removed_test_in_full_test_run()
        {
            var results = new TestRunResults("project1", "assembly", false, TestRunner.NUnit, new TestResult[]
                                                    {
                                                        new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test1"),
                                                        new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test2"),
                                                        new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test3")
                                                    });
            var cache = new RunResultCache();
            cache.Merge(results);

            results = new TestRunResults("project1", "assembly", false, TestRunner.NUnit, new TestResult[]
                                                    {
                                                        new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test1")
                                                    });

            var locator = new RemovedTestsLocator(cache);
            results =  locator.SetRemovedTestsAsPassed(results, null);
            
            results.Passed.Length.ShouldEqual(2);
            results.Passed[0].Name.ShouldEqual("Test3");
            results.Passed[1].Name.ShouldEqual("Test2");
        }

        [Test]
        public void Should_locate_removed_test_in_partial_test_run()
        {
            var results = new TestRunResults("project1", "assembly", false, TestRunner.NUnit, new TestResult[]
                                                    {
                                                        new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test1"),
                                                        new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test2"),
                                                        new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test3"),
                                                        new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test4")
                                                    });
            var cache = new RunResultCache();
            cache.Merge(results);

            var infos = new TestRunInfo[] { new TestRunInfo(new Project("project", new ProjectDocument(ProjectType.CSharp)), "assembly") };
            infos[0].AddTestsToRun(new TestToRun[] { new TestToRun(TestRunner.NUnit, "Test1"), new TestToRun(TestRunner.NUnit, "Test2"), new TestToRun(TestRunner.NUnit, "Test3") });
            results = new TestRunResults("project1", "assembly", true, TestRunner.NUnit, new TestResult[]
                                                    {
                                                        new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test1")
                                                    });

            var locator = new RemovedTestsLocator(cache);
            results = locator.SetRemovedTestsAsPassed(results, infos);

            results.Passed.Length.ShouldEqual(2);
            results.Passed[0].Name.ShouldEqual("Test3");
            results.Passed[1].Name.ShouldEqual("Test2");
        }

        [Test]
        public void Should_not_remove_tests_from_different_runners()
        {
            var cache = new RunResultCache();
            cache.EnabledDeltas();
            var results = new TestRunResults("project", "assembly", false, TestRunner.NUnit, new TestResult[]
                                                    {
                                                        new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test1"),
                                                    });
            cache.Merge(results);
            var mah = cache.PopDeltas();
            mah.AddedTests.Length.ShouldEqual(1);

            var infos = new TestRunInfo[] { new TestRunInfo(new Project("project", new ProjectDocument(ProjectType.CSharp)), "assembly") };
            results = new TestRunResults("project", "assembly", false, TestRunner.XUnit, new TestResult[]
                                                    {
                                                        new TestResult(TestRunner.XUnit, TestRunStatus.Failed, "Test1"),
                                                    });

            var locator = new RemovedTestsLocator(cache);
            results = locator.SetRemovedTestsAsPassed(results, infos);
            cache.Merge(results);

            results.Passed.Length.ShouldEqual(0);

            var meh = cache.PopDeltas();
            meh.RemovedTests.Length.ShouldEqual(0);
        }

        [Test]
        public void Should_remove_tests_for_run_infos_having_run_all_tests()
        {
            var results = new TestRunResults("project1", "assembly", false, TestRunner.NUnit, new TestResult[]
                                                    {
                                                        new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test1")
                                                    });
            var cache = new RunResultCache();
            cache.Merge(results);

            var runInfo = new TestRunInfo(new Project("project1", new ProjectDocument(ProjectType.CSharp)), "assembly");
            
            var locator = new RemovedTestsLocator(cache);
            var output = locator.RemoveUnmatchedRunInfoTests(new TestRunResults[] {}, new TestRunInfo[] { runInfo });

            output.Count.ShouldEqual(1);
            output[0].Passed.Length.ShouldEqual(1);
            output[0].Passed[0].Name.ShouldEqual("Test1");
        }
    }
}
