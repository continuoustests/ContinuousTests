using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using AutoTest.Core.Caching.RunResultCache;
using AutoTest.Core.DebugLog;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    public class RemovedTestsLocator : ILocateRemovedTests
    {
        private IRunResultCache _cache;
        private TestRunInfo[] _infos;
        private TestRunResults _results;

        public RemovedTestsLocator(IRunResultCache cache)
        {
            _cache = cache;
        }

        public List<TestRunResults> RemoveUnmatchedRunInfoTests(TestRunResults[] results, TestRunInfo[] infos)
        {
            var tests = new List<TestItem>();
            tests.AddRange(_cache.Failed);
            tests.AddRange(_cache.Ignored);

            var removed = new List<TestRunResults>();
            infos
                .Where(i => results.Count(x => i.Assembly.Equals(x.Assembly)) == 0).ToList()
                .ForEach(i =>
                    {
                        tests.Where(x => x.Key.Equals(i.Assembly) &&
                                   (!i.OnlyRunSpcifiedTestsFor(x.Value.Runner) || i.GetTestsFor(x.Value.Runner).Count(t => t.Equals(x.Value.Name)) > 0))
                            .GroupBy(x => x.Value.Runner).ToList()
                            .ForEach(x => 
                                {
                                    removed.Add(new TestRunResults(
                                        i.Project.Key,
                                        i.Assembly,
                                        i.OnlyRunSpcifiedTestsFor(TestRunner.Any),
                                        x.Key,
                                        x.Select(t => new TestResult(
                                            t.Value.Runner,
                                            TestRunStatus.Passed,
                                            t.Value.Name,
                                            t.Value.Message,
                                            t.Value.StackTrace,
                                            t.Value.TimeSpent.TotalMilliseconds).SetDisplayName(t.Value.DisplayName)).ToArray()));
                                });
                    });
            return removed;
        }

        public TestRunResults SetRemovedTestsAsPassed(TestRunResults results, TestRunInfo[] infos)
        {
            _results = results;
            _infos = infos;
            var tests = new List<TestResult>();
            tests.AddRange(results.All);
            tests.AddRange(getTests(_cache.Failed));
            tests.AddRange(getTests(_cache.Ignored));
            var modified = new TestRunResults(_results.Project, _results.Assembly, _results.IsPartialTestRun, _results.Runner, tests.ToArray());
            modified.SetTimeSpent(_results.TimeSpent);
            return modified;
        }

        private TestResult[] getTests(TestItem[] cacheList)
        {
            var tests = new List<TestResult>();
            foreach (var test in cacheList)
            {
                if (!test.Value.Runner.Equals(_results.Runner))
                    continue;
                if ((from t in _results.All where new TestItem(_results.Assembly, _results.Project, t).IsTheSameTestAs(test) select t).Count() == 0)
                {
                    if (_results.IsPartialTestRun && !wasRun(test))
                        continue;
                    tests.Add(new TestResult(test.Value.Runner, TestRunStatus.Passed, test.Value.Name, test.Value.Message, test.Value.StackTrace, test.Value.TimeSpent.TotalMilliseconds).SetDisplayName(test.Value.DisplayName));
                    Debug.WriteDebug("Adding deleted test previously failing as passed: " + test.Value.Name);
                }
            }
            return tests.ToArray();
        }

        private bool wasRun(TestItem test)
        {
            foreach (var info in _infos)
            {
                if (info.Assembly.Equals(_results.Assembly))
                {
                    if (info.GetTestsFor(test.Value.Runner).Contains(test.Value.Name))
                        return true;
                }
            }
            Debug.WriteDebug(test.Value.Name + " was not run");
            return false;
        }
    }
}
