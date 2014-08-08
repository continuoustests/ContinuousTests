using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Paths
{
    [TestFixture]
    public class when_finding_single_path_in_teardown
    {
        private List<IEnumerable<string>> _paths;

        [SetUp]
        public void SetUp()
        {
            var chain = new CallChain("Root", "RootR", 1);
            var c = new CallChain("Child1", "Child1R", 2);
            chain.AddChild(c);
            chain.AddChild(new CallChain("Child2", "Child2R", 3));
            var child = new CallChain("Child3", "Child3R", 4);
            child.AddChild(new CallChain("GrandChild", "GrandChildR", 5));
            chain.AddChild(child);
            var information = new TestRunInformation { Name = "test", };
            information.AddTearDown(chain);
            information.TestChain = new CallChain("A Test", "runtime", 332);
            _paths = PathFinder.FindPathsTo(information, "GrandChild").ToList();
        }

        [Test]
        public void one_path_is_present()
        {
            Assert.AreEqual(1, _paths.Count);
        }

        [Test]
        public void there_are_four_nodes_in_the_path()
        {
            Assert.AreEqual(4, _paths[0].Count());
        }

        [Test]
        public void the_first_node_is_the_grandchild()
        {
            Assert.AreEqual("GrandChild", _paths[0].ToList()[0]);
        }

        [Test]
        public void the_last_node_is_the_test()
        {
            Assert.AreEqual("A Test", _paths[0].ToList()[3]);
        }
    }
}