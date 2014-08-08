using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Paths
{
    [TestFixture]
    public class when_finding_multiple_paths_in_test
    {
        private List<IEnumerable<string>> _paths;

        [SetUp]
        public void SetUp()
        {
            var chain = new CallChain("Root", "RootR", 1);
            var c = new CallChain("Child1", "Child1R", 2);
            c.AddChild(new CallChain("GrandChild", "GrandChildR", 17));
            chain.AddChild(c);
            chain.AddChild(new CallChain("Child2", "Child2R", 3));
            var child = new CallChain("Child3",  "Child3R", 4);
            child.AddChild(new CallChain("GrandChild","GrandChildR", 5));
            chain.AddChild(child);
            var information = new TestRunInformation {Name = "test", TestChain = chain};
            _paths = PathFinder.FindPathsTo(information, "GrandChild").ToList();
        }

        [Test]
        public void two_paths_are_present()
        {
            Assert.AreEqual(2, _paths.Count);
        }

        [Test]
        public void the_first_path_starts_at_the_root()
        {
            Assert.AreEqual("Root", _paths[0].Last());
        }

        [Test]
        public void the_first_path_has_depth_of_three()
        {
            Assert.AreEqual(3, _paths[0].Count());
        }

        [Test]
        public void the_first_path_connects_throughchild1()
        {
            Assert.AreEqual("Child1", _paths[0].ToList()[1]);
        }

        [Test]
        public void the_searched_node_is_first_node_in_first_path()
        {
            Assert.AreEqual("GrandChild", _paths[0].First());
        }

        [Test]
        public void the_searched_node_is_first_node_in_second_path()
        {
            Assert.AreEqual("GrandChild", _paths[1].First());
        }

        [Test]
        public void the_second_path_starts_at_the_root()
        {
            Assert.AreEqual("Root", _paths[1].Last());
        }
        [Test]
        public void the_second_path_connects_throughchild3()
        {
            Assert.AreEqual("Child3", _paths[1].ToList()[1]);
        }
        [Test]
        public void the_second_path_has_depth_of_three()
        {
            Assert.AreEqual(3, _paths[1].Count());
        }
    }
}