using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.TestRunners;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.RunResultCache;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Caching
{
    [TestFixture]
    public class RunResultCacheTest
    {
        private RunResultCache _runResultCache;

        [SetUp]
        public void SetUp()
        {
            _runResultCache = new RunResultCache();
        }
        
        [Test]
        public void Should_add_build_errors()
        {
            var results = new BuildRunResults("project");
            results.AddError(new BuildMessage());
            _runResultCache.Merge(results);
            _runResultCache.Errors.Length.ShouldEqual(1);
            _runResultCache.Errors[0].Key.ShouldEqual("project");
        }

        [Test]
        public void Should_merge_build_errors()
        {
            var results = new BuildRunResults("project");
            results.AddError(new BuildMessage() {File = "some file", ErrorMessage = "some error message"});
            _runResultCache.Merge(results);
            
            results = new BuildRunResults("project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            results.AddError(new BuildMessage() { File = "some other file", ErrorMessage = "some other error message" });
            _runResultCache.Merge(results);
            _runResultCache.Errors.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_find_build_error_delta()
        {
            _runResultCache.EnabledDeltas();
            var results = new BuildRunResults("project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);
            _runResultCache.PopDeltas();
            
            results = new BuildRunResults("project");
            results.AddError(new BuildMessage() { File = "some other file", ErrorMessage = "some other error message" });
            _runResultCache.Merge(results);
            var deltas = _runResultCache.PopDeltas();

            deltas.AddedErrors.Length.ShouldEqual(1);
            deltas.RemovedErrors.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_find_build_warning_delta()
        {
            _runResultCache.EnabledDeltas();
            var results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some warning message" });
            _runResultCache.Merge(results);
            _runResultCache.PopDeltas();

            results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage() { File = "some other file", ErrorMessage = "some other warning message" });
            _runResultCache.Merge(results);
            var deltas = _runResultCache.PopDeltas();

            deltas.AddedWarnings.Length.ShouldEqual(1);
            deltas.RemovedWarnings.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_not_merge_same_build_errors_from_different_project()
        {
            var results = new BuildRunResults("project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);

            results = new BuildRunResults("another project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);
            _runResultCache.Errors.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_remove_cached_build_errors_that_now_works()
        {
            var results = new BuildRunResults("project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);

            results = new BuildRunResults("another project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);

            results = new BuildRunResults("project");
            _runResultCache.Merge(results);
            _runResultCache.Errors.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_add_build_warnings()
        {
            var results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage());
            _runResultCache.Merge(results);
            _runResultCache.Warnings.Length.ShouldEqual(1);
            _runResultCache.Warnings[0].Key.ShouldEqual("project");
        }

        [Test]
        public void Should_merge_build_warnings()
        {
            var results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);

            results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            results.AddWarning(new BuildMessage() { File = "some other file", ErrorMessage = "some other error message" });
            _runResultCache.Merge(results);
            _runResultCache.Warnings.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_not_merge_same_build_warnings_from_different_project()
        {
            var results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);

            results = new BuildRunResults("another project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);
            _runResultCache.Warnings.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_remove_cached_build_warnings_that_now_works()
        {
            var results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);

            results = new BuildRunResults("another project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);

            results = new BuildRunResults("project");
            _runResultCache.Merge(results);
            _runResultCache.Warnings.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_add_failed_tests()
        {
            var results = new TestResult[]
                              {
                                  new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] {})
                              };
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, results);
            _runResultCache.Merge(runResults);
            _runResultCache.Failed.Length.ShouldEqual(1);
            _runResultCache.Failed[0].Key.ShouldEqual("assembly");
            _runResultCache.Failed[0].Project.ShouldEqual("project");
        }

        [Test]
        public void Should_merge_failed_tests()
        {
            var results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, results);
            _runResultCache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, results);
            _runResultCache.Merge(runResults);

            _runResultCache.Failed.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_not_merge_same_failed_tests_from_different_assemblies()
        {
            var results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, results);
            _runResultCache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "another assembly", false, TestRunner.NUnit, results);
            _runResultCache.Merge(runResults);

            _runResultCache.Failed.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_not_merge_same_failed_tests_with_different_runners()
        {
            var results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, results);
            _runResultCache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestRunner.MSTest, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "assembly", false, TestRunner.MSTest, results);
            _runResultCache.Merge(runResults);

            _runResultCache.Failed.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_remove_cached_failed_tests_that_now_passes()
        {
            var results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, results);
            _runResultCache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Passed, "Test name", "", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, results);
            _runResultCache.Merge(runResults);

            Assert.AreEqual(0, _runResultCache.Failed.Length);
            _runResultCache.Failed.Length.ShouldEqual(0);
        }

        [Test]
        public void Should_add_ignored_tests()
        {
            var results = new TestResult[]
                              {
                                  new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] {})
                              };
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, results);
            _runResultCache.Merge(runResults);
            _runResultCache.Ignored.Length.ShouldEqual(1);
            _runResultCache.Ignored[0].Key.ShouldEqual("assembly");
            _runResultCache.Ignored[0].Project.ShouldEqual("project");
        }

        [Test]
        public void Should_merge_ignored_tests()
        {
            var results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, results);
            _runResultCache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, results);
            _runResultCache.Merge(runResults);

            _runResultCache.Ignored.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_not_merge_same_ignored_tests_from_different_assemblies()
        {
            var results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, results);
            _runResultCache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "another assembly", false, TestRunner.NUnit, results);
            _runResultCache.Merge(runResults);

            _runResultCache.Ignored.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_remove_cached_ignored_tests_that_now_passes()
        {
            var results = new TestResult[]
                              {
                                  new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] {}),
                                  new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Another test", "Message", new IStackLine[] {})
                              };
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, results);
            _runResultCache.Merge(runResults);

            runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Passed, "Test name", "", new IStackLine[] { }) });
            _runResultCache.Merge(runResults);

            _runResultCache.Ignored.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_merge_tests_going_from_failed_to_ignored()
        {
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) });
            _runResultCache.Merge(runResults);

            runResults = new TestRunResults("project", "assembly", true, TestRunner.NUnit, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Another message", new IStackLine[] { }) });
            _runResultCache.Merge(runResults);

            _runResultCache.Ignored.Length.ShouldEqual(1);
            _runResultCache.Failed.Length.ShouldEqual(0);
        }

        [Test]
        public void Should_merge_tests_going_from_ignored_to_failed()
        {
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] { }) });
            _runResultCache.Merge(runResults);

            runResults = new TestRunResults("project", "assembly", true, TestRunner.NUnit, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Another message", new IStackLine[] { }) });
            _runResultCache.Merge(runResults);

            _runResultCache.Ignored.Length.ShouldEqual(0);
            _runResultCache.Failed.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_merge_changed_tests_from_the_same_category()
        {
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) });
            _runResultCache.Merge(runResults);

            runResults = new TestRunResults("project", "assembly", true, TestRunner.NUnit, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { new StackLineMessage("method", "file", 10) }) });
            _runResultCache.Merge(runResults);

            _runResultCache.Failed.Length.ShouldEqual(1);
            _runResultCache.Failed[0].Value.StackTrace.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_clear_cache_when_issuing_clear()
        {
            var results = new BuildRunResults("another project");
            results.AddError(new BuildMessage() { File = "some files", ErrorMessage = "some error messages" });
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);
            _runResultCache.Merge(new TestRunResults("project", "assembly", false, TestRunner.NUnit, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] { }) }));
            _runResultCache.Merge(new TestRunResults("project", "assembly", true, TestRunner.NUnit, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { new StackLineMessage("method", "file", 10) }) }));

            _runResultCache.Clear();
            _runResultCache.Errors.Length.ShouldEqual(0);
            _runResultCache.Warnings.Length.ShouldEqual(0);
            _runResultCache.Failed.Length.ShouldEqual(0);
            _runResultCache.Ignored.Length.ShouldEqual(0);
        }

        [Test]
        public void Should_clear_cache_when_issuing_clear_with_deltas()
        {
            var results = new BuildRunResults("another project");
            results.AddError(new BuildMessage() { File = "some files", ErrorMessage = "some error messages" });
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);
            _runResultCache.Merge(new TestRunResults("project", "assembly", false, TestRunner.NUnit, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] { }) }));
            _runResultCache.Merge(new TestRunResults("project", "assembly", true, TestRunner.NUnit, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { new StackLineMessage("method", "file", 10) }) }));

            _runResultCache.EnabledDeltas();
            _runResultCache.Clear();
            _runResultCache.Errors.Length.ShouldEqual(0);
            _runResultCache.Warnings.Length.ShouldEqual(0);
            _runResultCache.Failed.Length.ShouldEqual(0);
            _runResultCache.Ignored.Length.ShouldEqual(0);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void Should_fail_is_cache_is_not_setup_to_support_deltas()
        {
            _runResultCache.PopDeltas();
        }

        [Test]
        public void Should_find_test_delta()
        {
            _runResultCache.EnabledDeltas();
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, new TestResult[]
                                {
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Passed, "Passing test name", "Message", new IStackLine[] { }),
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Failing test that will pass", "Message", new IStackLine[] { }),
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] { })
                                });
            _runResultCache.Merge(runResults);
            _runResultCache.PopDeltas();

            runResults = new TestRunResults("project", "assembly", true, TestRunner.NUnit, new TestResult[]
                                {
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Passed, "Failing test that will pass", "Message", new IStackLine[] { }),
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { })
                                });
            _runResultCache.Merge(runResults);
            var deltas = _runResultCache.PopDeltas();

            deltas.AddedTests.Length.ShouldEqual(1);
            deltas.AddedTests[0].Value.Name.ShouldEqual("Test name");
            deltas.AddedTests[0].Value.Status.ShouldEqual(TestRunStatus.Failed);
            deltas.RemovedTests.Length.ShouldEqual(2);
            deltas.RemovedTests[0].Value.Name.ShouldEqual("Failing test that will pass");
            deltas.RemovedTests[1].Value.Name.ShouldEqual("Test name");
        }

        [Test]
        public void Should_find_test_deltas_in_same_status()
        {
            _runResultCache.EnabledDeltas();
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, new TestResult[]
                                {
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] { })
                                });
            _runResultCache.Merge(runResults);
            _runResultCache.PopDeltas();

            runResults = new TestRunResults("project", "assembly", true, TestRunner.NUnit, new TestResult[]
                                {
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message 2", new IStackLine[] { })
                                });
            _runResultCache.Merge(runResults);
            var deltas = _runResultCache.PopDeltas();

            deltas.AddedTests.Length.ShouldEqual(1);
            deltas.AddedTests[0].Value.Name.ShouldEqual("Test name");
            deltas.AddedTests[0].Value.Status.ShouldEqual(TestRunStatus.Ignored);
            deltas.AddedTests[0].Value.Message.ShouldEqual("Message 2");
            deltas.RemovedTests.Length.ShouldEqual(1);
            deltas.RemovedTests[0].Value.Name.ShouldEqual("Test name");
        }

        [Test]
        public void Should_find_test_delta_since_last_pop()
        {
            _runResultCache.EnabledDeltas();
            var runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, new TestResult[]
                                {
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Passed, "Passing test name", "Message", new IStackLine[] { }),
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Some failing test", "Message", new IStackLine[] { }),
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message 1", new IStackLine[] { })
                                });
            _runResultCache.Merge(runResults);
            var deltas = _runResultCache.PopDeltas();
            deltas.AddedTests.Length.ShouldEqual(2);

            runResults = new TestRunResults("project", "assembly", false, TestRunner.NUnit, new TestResult[]
                                {
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Passed, "Passing test name", "Message", new IStackLine[] { }),
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Failing test that will pass", "Message", new IStackLine[] { }),
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message 2", new IStackLine[] { })
                                });
            _runResultCache.Merge(runResults);

            runResults = new TestRunResults("project", "assembly", true, TestRunner.NUnit, new TestResult[]
                                {
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Passed, "Failing test that will pass", "Message", new IStackLine[] { }),
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message 3", new IStackLine[] { })
                                });
            _runResultCache.Merge(runResults);
            deltas = _runResultCache.PopDeltas();

            deltas.AddedTests.Length.ShouldEqual(1);
            deltas.AddedTests[0].Value.Name.ShouldEqual("Test name");
            deltas.AddedTests[0].Value.Message.ShouldEqual("Message 3");
            deltas.AddedTests[0].Value.Status.ShouldEqual(TestRunStatus.Failed);
            deltas.RemovedTests.Length.ShouldEqual(1);
            deltas.RemovedTests[0].Value.Name.ShouldEqual("Test name");
            deltas.RemovedTests[0].Value.Status.ShouldEqual(TestRunStatus.Failed);
        }

        [Test]
        public void Should_find_test_delta_since_last_pop_with_different_runners()
        {

            var locator = new AutoTest.Core.Messaging.MessageConsumers.RemovedTestsLocator(_runResultCache);
            _runResultCache.EnabledDeltas();
            var infos = new AutoTest.Core.Messaging.MessageConsumers.TestRunInfo[] { new AutoTest.Core.Messaging.MessageConsumers.TestRunInfo(new AutoTest.Core.Caching.Projects.Project("project", new AutoTest.Core.Caching.Projects.ProjectDocument(AutoTest.Core.Caching.Projects.ProjectType.CSharp)), "assembly") };
            infos[0].AddTestsToRun(new AutoTest.Core.Messaging.MessageConsumers.TestToRun[] { new AutoTest.Core.Messaging.MessageConsumers.TestToRun(TestRunner.NUnit, "Test name") });
            infos[0].ShouldOnlyRunSpcifiedTestsFor(TestRunner.NUnit);
            infos[0].ShouldOnlyRunSpcifiedTestsFor(TestRunner.MSTest);
            var runResults = new TestRunResults("project", "assembly", true, TestRunner.NUnit, new TestResult[]
                                {
                                    new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message 1", new IStackLine[] { })
                                });
            runResults = locator.SetRemovedTestsAsPassed(runResults, infos);
            runResults.Passed.Length.ShouldEqual(0);
            _runResultCache.Merge(runResults);
            var deltas = _runResultCache.PopDeltas();
            deltas.AddedTests.Length.ShouldEqual(1);
            deltas.RemovedTests.Length.ShouldEqual(0);

            runResults = new TestRunResults("project", "assembly", false, TestRunner.XUnit, new TestResult[]
                                {
                                });
            runResults = locator.SetRemovedTestsAsPassed(runResults, infos);
            _runResultCache.Merge(runResults);
            
            deltas = _runResultCache.PopDeltas();

            deltas.AddedTests.Length.ShouldEqual(0);
            deltas.RemovedTests.Length.ShouldEqual(0);
        }
    }
}
