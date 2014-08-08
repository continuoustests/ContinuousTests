using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;

namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
    [TestFixture]
    public class TestRunInfoMergerTests
    {
        private List<RunInfo> _list;
        private List<RunInfo> _newList;

        [SetUp]
        public void SetUp()
        {
            _list = new List<RunInfo>();
            _newList = new List<RunInfo>();
        }

        [Test]
        public void When_assembly_does_not_exist_it_should_add_to_list()
        {
            _list.Add(getItem("Assembly1"));
            _newList.Add(getItem("Assembly2"));
            Assert.That(new TestRunInfoMerger(_list).MergeByAssembly(_newList).Count, Is.EqualTo(2));
        }

        [Test]
        public void When_assembly_exists_it_should_not_add_to_list()
        {
            var item = getItem("Assembly1");
            _list.Add(item);
            _newList.Add(item);
            Assert.That(new TestRunInfoMerger(_list).MergeByAssembly(_newList).Count, Is.EqualTo(1));
        }

        [Test]
        public void When_assembly_exists_with_different_tests_it_should_merge_with_existing()
        {
            var item = getItem("Assembly1");
            item.AddTestsToRun(getTest(AutoTest.Messages.TestRunner.NUnit, "Existing test"));
            _list.Add(item);
            item = getItem("Assembly1");
            item.AddTestsToRun(getTest(AutoTest.Messages.TestRunner.NUnit, "Existing test"));
            item.AddTestsToRun(getTest(AutoTest.Messages.TestRunner.NUnit, "A new test"));
            _newList.Add(item);
            Assert.That(new TestRunInfoMerger(_list).MergeByAssembly(_newList).ElementAt(0).GetTests().Count(), Is.EqualTo(2));
        }
        
        [Test]
        public void When_assembly_exists_with_same_test_but_different_runner_it_should_merge_with_existing()
        {
            var item = getItem("Assembly1");
            item.AddTestsToRun(getTest(AutoTest.Messages.TestRunner.NUnit, "Existing test"));
            _list.Add(item);
            item = getItem("Assembly1");
            item.AddTestsToRun(getTest(AutoTest.Messages.TestRunner.MSTest, "Existing test"));
            _newList.Add(item);
            Assert.That(new TestRunInfoMerger(_list).MergeByAssembly(_newList).ElementAt(0).GetTests().Count(), Is.EqualTo(2));
        }

        [Test]
        public void When_assembly_exists_with_different_members_it_should_merge_with_existing()
        {
            var item = getItem("Assembly1");
            item.AddMembersToRun(getTest(AutoTest.Messages.TestRunner.NUnit, "Existing member"));
            _list.Add(item);
            item = getItem("Assembly1");
            item.AddMembersToRun(getTest(AutoTest.Messages.TestRunner.NUnit, "Existing member"));
            item.AddMembersToRun(getTest(AutoTest.Messages.TestRunner.NUnit, "A new member"));
            _newList.Add(item);
            Assert.That(new TestRunInfoMerger(_list).MergeByAssembly(_newList).ElementAt(0).GetMembers().Count(), Is.EqualTo(2));
        }

        //[Test]
        //public void When_assembly_exists_with_different_namespaces_it_should_merge_with_existing()
        //{
        //    var item = getItem("Assembly1");
        //    item.AddNamespacesToRun(getTest(AutoTest.Messages.TestRunner.NUnit, "Existing namespace"));
        //    _list.Add(item);
        //    item = getItem("Assembly1");
        //    item.AddNamespacesToRun(getTest(AutoTest.Messages.TestRunner.NUnit, "Existing namespace"));
        //    item.AddNamespacesToRun(getTest(AutoTest.Messages.TestRunner.NUnit, "A new namespace"));
        //    _newList.Add(item);
        //    Assert.That(new TestRunInfoMerger(_list).MergeByAssembly(_newList).ElementAt(0).GetNamespaces().Count(), Is.EqualTo(2));
        //}

        private TestToRun[] getTest(AutoTest.Messages.TestRunner runner, string test)
        {
            return new TestToRun[] { new TestToRun(runner, test) };
        }

        private RunInfo getItem(string assemblyName)
        {
            var info = new RunInfo(null);
            info.SetAssembly(assemblyName);
            return info;
        }
    }
}
