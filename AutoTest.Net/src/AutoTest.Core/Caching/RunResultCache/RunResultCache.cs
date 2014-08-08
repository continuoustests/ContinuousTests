using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.TestRunners;
using AutoTest.Messages;
using AutoTest.Core.DebugLog;

namespace AutoTest.Core.Caching.RunResultCache
{
    public class RunResultCache : IRunResultCache, IMergeRunResults
    {
        private object _padLock = new object();
        private bool _deltasSupported = false;

        private Dictionary<int, BuildItem> _dict = new Dictionary<int, BuildItem>();

        private Dictionary<int, BuildItem> _errors = new Dictionary<int, BuildItem>();
        private Dictionary<int, BuildItem> _warnings = new Dictionary<int, BuildItem>();
        private Dictionary<int, TestItem> _failed = new Dictionary<int, TestItem>();
        private Dictionary<int, TestItem> _ignored = new Dictionary<int, TestItem>();

        private Dictionary<int, BuildItem> _lastErrors = new Dictionary<int, BuildItem>();
        private Dictionary<int, BuildItem> _lastWarnings = new Dictionary<int, BuildItem>();
        private Dictionary<int, TestItem> _lastFailed = new Dictionary<int, TestItem>();
        private Dictionary<int, TestItem> _lastIgnored = new Dictionary<int, TestItem>();

        public BuildItem[] Errors { get { return _errors.Select(x => x.Value).ToArray(); } }
        public BuildItem[] Warnings { get { return _warnings.Select(x => x.Value).ToArray(); } }
        public TestItem[] Failed { get { return _failed.Select(x => x.Value).ToArray(); } }
        public TestItem[] Ignored { get { return _ignored.Select(x => x.Value).ToArray(); } }

        public void Merge(BuildRunResults results)
        {
            lock (_padLock)
            {
                Debug.WriteDebug("Merging build run results");
                mergeBuildList(_errors, results.Project, results.Errors);
                mergeBuildList(_warnings, results.Project, results.Warnings);
            }
        }

        public void Merge(TestRunResults results)
        {
            lock (_padLock)
            {
                Debug.WriteDebug("Merging test run results");
                removeChanged(results);
                mergeTestList(_failed, results.Assembly, results.Project, results.Failed, results.Passed);
                mergeTestList(_ignored, results.Assembly, results.Project, results.Ignored, results.Passed);
            }
        }

        public void EnabledDeltas()
        {
            _deltasSupported = true;
        }

        public RunResultCacheDeltas PopDeltas()
        {
            if (!_deltasSupported)
                throw new Exception("Deltas are not supported in the run results cache. Run EnabledDeltas() to setup support");
            lock (_padLock)
            {
                var deltas = new RunResultCacheDeltas();
                deltas.Load(_lastErrors, _lastWarnings, _lastFailed, _lastIgnored, _errors, _warnings, _failed, _ignored);

                _lastErrors.Clear();
                _lastWarnings.Clear();
                _lastFailed.Clear();
                _lastIgnored.Clear();

                foreach (var item in _errors)
                    _lastErrors.Add(item.Key, item.Value);
                foreach (var item in _warnings)
                    _lastWarnings.Add(item.Key, item.Value);
                foreach (var item in _failed)
                    _lastFailed.Add(item.Key, item.Value);
                foreach (var item in _ignored)
                    _lastIgnored.Add(item.Key, item.Value);
                return deltas;
            }
        }

        public void Clear()
        {
            _errors.Clear();
            _warnings.Clear();
            _failed.Clear();
            _ignored.Clear();
            if (_deltasSupported)
                PopDeltas();
        }

        private void mergeBuildList(Dictionary<int, BuildItem> list, string key, BuildMessage[] results)
        {
            var itemsToRemove = new List<int>();
            var buildResults = new Dictionary<int, BuildItem>();
            results.ToList().ForEach(x =>
                                         {
                                             var resultItem = new BuildItem(key, x);
                                             buildResults.Add(resultItem.GetHashCode(), resultItem);
                                         });
            foreach (var item in list)
            {
                if (item.Value.Key.Equals(key) && !buildResults.ContainsKey(item.Key))
                {
                    //Debug.WriteDebug("Removing old build item from {0} in {1}, {2} {3}:{4}", item.Value.Key, item.Value.Value.File, item.Value.Value.ErrorMessage, item.Value.Value.LineNumber, item.Value.Value.LinePosition);
                    itemsToRemove.Add(item.Key);
                }
            }
            foreach (var item in itemsToRemove)
                list.Remove(item);
            foreach (var item in buildResults)
            {
                if (!list.ContainsKey(item.Key))
                {
                    //Debug.WriteDebug("Adding new build item from {0} in {1}, {2} {3}:{4}", item.Value.Key, item.Value.Value.File, item.Value.Value.ErrorMessage, item.Value.Value.LineNumber, item.Value.Value.LinePosition);
                    //list.Insert(0, item);
                    list.Add(item.Key, item.Value);
                }
            }
        }

        private void removeChanged(TestRunResults results)
        {
            foreach (var test in results.Passed)
            {
                var item = new TestItem(results.Assembly, results.Project, test);
                removeIfExists(item, _ignored);
                removeIfExists(item, _failed);
            }
            moveTestsBetweenStates(results, results.Failed, _ignored);
            moveTestsBetweenStates(results, results.Ignored, _failed);
        }

        private void removeIfExists(TestItem item, Dictionary<int, TestItem> list)
        {
            var passing = list
                .Where(x => x.Value.IsTheSameTestAs(item))
                .Select(x => x.Key)
                .ToArray();
            if (passing.Length > 0)
            {
                //logTest("Removing passing test ", item);
                foreach (var passed in passing)
                    list.Remove(passed);
            }
        }

        private void moveTestsBetweenStates(TestRunResults results, TestResult[] newSstate, Dictionary<int, TestItem> oldState)
        {
            foreach (var test in newSstate)
            {
                var item = new TestItem(results.Assembly, results.Project, test);
                var changed = oldState
                    .Where(x => x.Value.IsTheSameTestAs(item))
                    .Select(x => x.Key)
                    .ToArray();
                if (changed.Length > 0)
                {
                    //logTest("Removing test that changed state ", item);
                    foreach (var changedItem in changed)
                        oldState.Remove(changedItem);
                }
            }
        }

        private void mergeTestList(Dictionary<int, TestItem> list, string key, string project, TestResult[] results, TestResult[] passingTests)
        {
            foreach (var test in results)
            {
                var item = new TestItem(key, project, test);
                if (!list.ContainsKey(item.GetHashCode()))
                {
                    var newTests = list
                       .Where(x => x.Value.IsTheSameTestAs(item))
                       .Select(x => x.Key)
                       .ToArray();
                    if (newTests.Length > 0)
                    {
                        //logTest("Removing existing test in case it changed ", item);
                        foreach (var newTest in newTests)
                            list.Remove(newTest);
                    }
                    //logTest("Adding test ", item);
                    //list.Insert(0, item);
                    list.Add(item.GetHashCode(), item);
                }
            }
        }

        //private void logTest(string prefix, TestItem item)
        //{
            //Debug.WriteDebug("{6} ({2}.{1}) from {0} named {4} saying {3} in {5}", item.Key, item.Value.Status, item.Value.Runner, item.Value.Name, item.Value.Message, getStackTrace(item.Value.StackTrace), prefix);
        //}

        private string getStackTrace(IStackLine[] iStackLine)
        {
            var builder = new StringBuilder();
            foreach (var line in iStackLine)
                builder.Append(string.Format(" {0}, {1}:{2}", line.File, line.Method, line.LineNumber));
            return builder.ToString();
        }
    }
}
