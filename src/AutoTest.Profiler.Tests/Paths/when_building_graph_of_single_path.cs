using System.Collections.Generic;
using System.Linq;
using AutoTest.Graphs;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Paths
{
    [TestFixture]
    public class when_building_graph_of_multiple_paths
    {
        private AffectedGraph _graph;

        [SetUp]
        public void SetUp()
        {
            var paths = new[]
                            {
                                new List<string> {"GrandChild", "Child3", "Root"},
                                new List<string> {"GrandChild", "Child1", "Root"}
                            };
            _graph = GraphBuilder.BuildGraphFor(paths);
        }

        [Test]
        public void the_graph_has_four_nodes()
        {
            Assert.AreEqual(4, _graph.AllNodes().Count());
        }

        [Test]
        public void the_graph_has_four_connections()
        {
            Assert.AreEqual(4, _graph.AllConnections().Count());
        }

        [Test]
        public void the_first_connection_is_from_grandchild_to_child3()
        {
            Assert.AreEqual("GrandChild", _graph.AllConnections().ToList()[0].From);
            Assert.AreEqual("Child3", _graph.AllConnections().ToList()[0].To);
        }
        [Test]
        public void the_second_connection_is_from_child3_to_root()
        {
            Assert.AreEqual("Child3", _graph.AllConnections().ToList()[1].From);
            Assert.AreEqual("Root", _graph.AllConnections().ToList()[1].To);
        }
        [Test]
        public void the_third_connection_is_from_grandchild_to_child1()
        {
            Assert.AreEqual("GrandChild", _graph.AllConnections().ToList()[2].From);
            Assert.AreEqual("Child1", _graph.AllConnections().ToList()[2].To);
        }

        [Test]
        public void the_fourth_connection_is_from_child1_to_root()
        {
            Assert.AreEqual("Child1", _graph.AllConnections().ToList()[3].From);
            Assert.AreEqual("Root", _graph.AllConnections().ToList()[3].To);
        }

        [Test]
        public void the_root_node_is_the_searched_node()
        {
            Assert.AreEqual("GrandChild", _graph.GetRootNode().Name);
        }
    }


    [TestFixture]
    public class when_building_graph_of_single_path
    {
        private AffectedGraph _graph;

        [SetUp]
        public void SetUp()
        {
            var paths = new[]
                            {
                                new List<string> {"GrandChild", "Child3", "Root"},
                            };
            _graph = GraphBuilder.BuildGraphFor(paths);
        }

        [Test]
        public void the_graph_has_3_nodes()
        {
            Assert.AreEqual(3, _graph.AllNodes().Count());
        }

        [Test]
        public void the_graph_has_two_connections()
        {
            Assert.AreEqual(2, _graph.AllConnections().Count());
        }

        [Test]
        public void the_first_connection_is_from_grandchild_to_child3()
        {
            Assert.AreEqual("GrandChild", _graph.AllConnections().ToList()[0].From);
            Assert.AreEqual("Child3",_graph.AllConnections().ToList()[0].To);
        }
        [Test]
        public void the_second_connection_is_from_child3_to_root()
        {
            Assert.AreEqual("Child3", _graph.AllConnections().ToList()[1].From);
            Assert.AreEqual("Root", _graph.AllConnections().ToList()[1].To);
        }

        [Test]
        public void the_root_node_is_the_searched_node()
        {
            Assert.AreEqual("GrandChild", _graph.GetRootNode().Name);
        }
    }
}