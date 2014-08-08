using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.DebugLog;
using AutoTest.Messages;

namespace AutoTest.Core.Caching.RunResultCache
{
    public class RunResultCacheDeltas
    {
        private Dictionary<int, BuildItem> _addedErrors = new Dictionary<int, BuildItem>();
        private Dictionary<int, BuildItem> _removedErrors = new Dictionary<int, BuildItem>();
        private Dictionary<int, BuildItem> _addedWarnings = new Dictionary<int, BuildItem>();
        private Dictionary<int, BuildItem> _removedWarnings = new Dictionary<int, BuildItem>();

        private Dictionary<int, TestItem> _addedTests = new Dictionary<int, TestItem>();
        private Dictionary<int, TestItem> _removedTests = new Dictionary<int, TestItem>();

        public BuildItem[] AddedErrors { get { return _addedErrors.Select(x => x.Value).ToArray(); } }
        public BuildItem[] RemovedErrors { get { return _removedErrors.Select(x => x.Value).ToArray(); } }
        public BuildItem[] AddedWarnings { get { return _addedWarnings.Select(x => x.Value).ToArray(); } }
        public BuildItem[] RemovedWarnings { get { return _removedWarnings.Select(x => x.Value).ToArray(); } }

        public TestItem[] AddedTests { get { return _addedTests.Select(x => x.Value).ToArray(); } }
        public TestItem[] RemovedTests { get { return _removedTests.Select(x => x.Value).ToArray(); } }

        public void AddError(BuildItem error) { _addedErrors.Add(error.GetHashCode(), error); }
        public void RemoveError(BuildItem error) { _removedErrors.Add(error.GetHashCode(), error); }
        public void AddWarning(BuildItem warning) { _addedWarnings.Add(warning.GetHashCode(), warning); }
        public void RemoveWarning(BuildItem warning) { _removedWarnings.Add(warning.GetHashCode(), warning); }
        public void AddTest(TestItem test) { _addedTests.Add(test.GetHashCode(), test); }
        public void RemoveTest(TestItem ignored) { _removedTests.Add(ignored.GetHashCode(), ignored); }

        public void Load(Dictionary<int, BuildItem> lastErrors, Dictionary<int, BuildItem> lastWarnings, Dictionary<int, TestItem> lastFailed, Dictionary<int, TestItem> lastIgnored, Dictionary<int, BuildItem> errors, Dictionary<int, BuildItem> warnings, Dictionary<int, TestItem> failed, Dictionary<int, TestItem> ignored)
        {
            getBuildDeltas(lastErrors, errors, _addedErrors, _removedErrors);
            getBuildDeltas(lastWarnings, warnings, _addedWarnings, _removedWarnings);
            getTestDeltas(lastFailed, failed, _addedTests, _removedTests);
            getTestDeltas(lastIgnored, ignored, _addedTests, _removedTests);
            //logDeltas();
        }

        private void getBuildDeltas(Dictionary<int, BuildItem> lastBuildItems, Dictionary<int, BuildItem> buildItems, Dictionary<int, BuildItem> added, Dictionary<int, BuildItem> removed)
        {
            foreach (var error in buildItems)
            {
                if (!lastBuildItems.ContainsKey(error.Key))
                    added.Add(error.Key, error.Value);
            }
            foreach (var error in lastBuildItems)
            {
                if (!buildItems.ContainsKey(error.Key))
                    removed.Add(error.Key, error.Value);
            }
        }

        private void getTestDeltas(Dictionary<int, TestItem> lastTests, Dictionary<int, TestItem> tests, Dictionary<int, TestItem> added, Dictionary<int, TestItem> removed)
        {
            foreach (var test in tests)
            {
                if (!lastTests.ContainsKey(test.Key))
                    added.Add(test.Key, test.Value);
            }
            foreach (var test in lastTests)
            {
                if (!tests.ContainsKey(test.Key))
                    removed.Add(test.Key, test.Value);
            }
        }

        /*private void logDeltas()
        {
            if (Debug.IsDisabled)
                return;

            Debug.WriteDebug("Result deltas");
            foreach (var error in _addedErrors)
                logBuildItem("Added error", error.Value);
            foreach (var error in _removedErrors)
                logBuildItem("Removed error", error.Value);

            foreach (var warning in _addedWarnings)
                logBuildItem("Added warning", warning.Value);
            foreach (var warning in _removedWarnings)
                logBuildItem("Removed warning", warning.Value);

            foreach (var test in _addedTests)
                logTest("Added test", test.Value);
            foreach (var test in _removedTests)
                logTest("Removed test", test.Value);
        }

        private void logBuildItem(string prefix, BuildItem item)
        {
            Debug.WriteDebug("{5} {0} in {1}, {2} {3}:{4}", item.Key, item.Value.File, item.Value.ErrorMessage, item.Value.LineNumber, item.Value.LinePosition, prefix);
        }

        private void logTest(string prefix, TestItem item)
        {
            Debug.WriteDebug("{6} ({2}.{1}) from {0} named {4} saying {3} in {5}", item.Key, item.Value.Status, item.Value.Runner, item.Value.Name, item.Value.Message, getStackTrace(item.Value.StackTrace), prefix);
        }

        private string getStackTrace(IStackLine[] iStackLine)
        {
            var builder = new StringBuilder();
            foreach (var line in iStackLine)
                builder.Append(string.Format(" {0}, {1}:{2}", line.File, line.Method, line.LineNumber));
            return builder.ToString();
        }*/
    }
}
