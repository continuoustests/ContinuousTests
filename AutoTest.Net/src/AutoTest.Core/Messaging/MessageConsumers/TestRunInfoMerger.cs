using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    public class TestRunInfoMerger
    {
        private List<RunInfo> _list;

        public TestRunInfoMerger(IEnumerable<RunInfo> list)
        {
            _list = list.ToList();
        }

        public List<RunInfo> MergeByAssembly(IEnumerable<RunInfo> list)
        {
            addUnexisting(list.ToList(), (x, y) => x.Assembly.Equals(y.Assembly));
            mergeExisting(list.ToList(), (x, y) => x.Assembly.Equals(y.Assembly));
            return _list;
        }

        public List<RunInfo> MergeByProject(IEnumerable<RunInfo> list)
        {
            addUnexisting(list.ToList(), (x, y) => x.Project.Key.Equals(y.Project.Key));
            mergeExisting(list.ToList(), (x, y) => x.Project.Key.Equals(y.Project.Key));
            return _list;
        }

        private void mergeExisting(List<RunInfo> list, Func<RunInfo, RunInfo, bool> match)
        {
            addNewEntries(list, (info) => { return info.GetTests().ToList(); }, (info, test) => { info.AddTestsToRun(new TestToRun[] { test }); }, match);
            addNewEntries(list, (info) => { return info.GetMembers().ToList(); }, (info, member) => { info.AddMembersToRun(new TestToRun[] { member }); }, match);
            addNewEntries(list, (info) => { return info.GetNamespaces().ToList(); }, (info, ns) => { info.AddNamespacesToRun(new TestToRun[] { ns }); }, match);
        }

        public void addNewEntries(List<RunInfo> list, Func<RunInfo, List<TestToRun>> getItems, Action<RunInfo, TestToRun> addItem, Func<RunInfo, RunInfo, bool> match)
        {
            list.Where(x => exists(x, match)).ToList().ForEach(x => getItems.Invoke(x).ForEach(t => addIfNew(x, t, getItems, addItem, match)));
        }

        private void addIfNew(RunInfo x, TestToRun t, Func<RunInfo, List<TestToRun>> getItems, Action<RunInfo, TestToRun> addItem, Func<RunInfo, RunInfo, bool> match)
        {
            var current = _list.Where(y => match(y, x)).First();
            if (!testExists(t, getItems, current))
                addItem(current, t);
        }

        private static bool testExists(TestToRun t, Func<RunInfo, List<TestToRun>> getItems, RunInfo current)
        {
            return getItems(current).Exists(test => compareTests(t, test));
        }

        private static bool compareTests(TestToRun a, TestToRun b)
        {
            return a.Runner.Equals(b.Runner) && a.Test.Equals(b.Test);
        }

        private void addUnexisting(List<RunInfo> list, Func<RunInfo, RunInfo, bool> match)
        {
            list.Where(x => !exists(x, match)).ToList()
                .ForEach(x => _list.Add(x));
        }

        private bool exists(RunInfo x, Func<RunInfo, RunInfo, bool> match)
        {
            return _list.Exists(y => match(y, x));
        }
    }
}
