using System.Collections.Generic;
using System.Linq;
using AutoTest.Graphs;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests.Graphs
{
    [TestFixture]
    public class when_merging_graphs
    {
        [Test]
        public void empty_graphs_merge_to_empty_graph()
        {
            var graph1 = new AffectedGraph();
            var graph2 = new AffectedGraph();
            var merged = graph1.Merge(graph2);
            Assert.AreEqual(0, merged.AllNodes().Count());
        }

        [Test]
        public void nodes_from_both_graphs_get_added_to_output()
        {
            var graph1 = new AffectedGraph();
            graph1.AddNode(new AffectedGraphNode("test1", false, false, false, "test", "test12", "assembly", "type", new List<TestDescriptor>(), false, false, 0));
            var graph2 = new AffectedGraph();
            graph2.AddNode(new AffectedGraphNode("test1", false, false, false, "test", "test123", "assembly", "type", new List<TestDescriptor>(), false, false, 0));
            var merged = graph1.Merge(graph2);
            Assert.AreEqual(2, merged.AllNodes().Count());
        }

        [Test]
        public void connections_from_both_graphs_get_added_to_output()
        {
            var graph1 = new AffectedGraph();
            graph1.AddNode(new AffectedGraphNode("test1", false, false, false, "test", "test123", "assembly", "type", new List<TestDescriptor>(), false, false, 0));
            graph1.AddConnection("test123", "test123", false);
            var graph2 = new AffectedGraph();
            graph2.AddNode(new AffectedGraphNode("test12", false, false, false, "test", "test1234", "assembly", "type", new List<TestDescriptor>(), false, false, 0));
            graph2.AddConnection("test1234", "test1234", false);
            var merged = graph1.Merge(graph2);
            Assert.AreEqual(2, merged.AllConnections().Count());
        } 
    }

    [TestFixture]
    public class AffectedGraphTests
    {
        [Test]
        public void adding_a_null_node_does_not_add_node()
        {
            var g = new AffectedGraph();
            g.AddNode(null);
            Assert.AreEqual(0, g.AllNodes().Count());
        }

        [Test]
        public void adding_a_null_from_connection_does_not_add_node()
        {
            var g = new AffectedGraph();
            g.AddConnection(null, "TO", false);
            Assert.AreEqual(0, g.AllConnections().Count());
        }

        [Test]
        public void adding_a_null_to_connection_does_not_add_node()
        {
            var g = new AffectedGraph();
            g.AddConnection("FROM", null, false);
            Assert.AreEqual(0, g.AllConnections().Count());
        }

        [Test]
        public void a_node_can_be_added()
        {
            var g = new AffectedGraph();
            g.AddNode(new AffectedGraphNode("foo", false, false, false, "name", "bar::foo", "assembly", "type", new List<TestDescriptor>(), false, false, 0));
            Assert.AreEqual(1, g.AllNodes().Count());
            Assert.AreEqual("foo", g.AllNodes().First().DisplayName);
            Assert.AreEqual("bar::foo", g.AllNodes().First().FullName);
            Assert.IsNotNull(g.GetNode("bar::foo"));
        }

        [Test]
        public void when_no_root_node_get_root_node_returns_null()
        {
            var g = new AffectedGraph();
            g.AddNode(new AffectedGraphNode("foo", false, false, false, "name", "bar::foo", "assembly", "type", new List<TestDescriptor>(), false, false, 0));
            Assert.IsNull(g.GetRootNode());
        }

        [Test]
        public void when_root_node_get_root_node_returns_root_node()
        {
            var g = new AffectedGraph();
            var root = new AffectedGraphNode("foo", false, false, true, "name", "bar::foo", "assembly", "type", new List<TestDescriptor>(), false, false, 0);
            g.AddNode(root);
            g.AddNode(new AffectedGraphNode("foo", false, false, false, "name", "bar::foo2", "assembly", "type", new List<TestDescriptor>(), false, false, 0));
            Assert.AreEqual(root, g.GetRootNode());
        }

        [Test]
        public void a_connection_to_a_non_existant_node_does_not_add_connection()
        {
            var g = new AffectedGraph();
            g.AddNode(new AffectedGraphNode("foo", false, false, false, "name", "bar::foo", "assembly", "type", new List<TestDescriptor>(), false, false, 0));
            g.AddConnection("bar::foo", "somethingnon-existant", false);
            Assert.AreEqual(0, g.AllConnections().Count());
        }

        [Test]
        public void a_connection_from_a_non_existant_node_does_not_add_connection()
        {
            var g = new AffectedGraph();
            g.AddNode(new AffectedGraphNode("foo", false, false, false, "name", "bar::foo", "assembly", "type", new List<TestDescriptor>(), false, false, 0));
            g.AddConnection("somethingnon-existant", "bar::foo", false);
            Assert.AreEqual(0, g.AllConnections().Count());
        }

        [Test]
        public void a_connection_can_be_added_between_two_valid_nodes()
        {
            var g = new AffectedGraph();
            g.AddNode(new AffectedGraphNode("foo", false, false, false, "name", "bar::foo", "assembly", "type", new List<TestDescriptor>(), false, false, 0));
            g.AddNode(new AffectedGraphNode("foo", false, false, false, "name", "bar::foo2", "assembly", "type", new List<TestDescriptor>(), false, false, 0));
            g.AddConnection("bar::foo2", "bar::foo", false);
            Assert.AreEqual(1, g.AllConnections().Count());
        }

        [Test]
        public void multiple_connections_can_be_added_between_two_valid_nodes_but_only_one_appears()
        {
            var g = new AffectedGraph();
            g.AddNode(new AffectedGraphNode("foo", false, false, false, "name", "bar::foo", "assembly", "type", new List<TestDescriptor>(), false, false, 0));
            g.AddNode(new AffectedGraphNode("foo", false, false, false, "name", "bar::foo2", "assembly", "type", new List<TestDescriptor>(), false, false, 0));
            g.AddConnection("bar::foo2", "bar::foo", false);
            g.AddConnection("bar::foo2", "bar::foo", false);
            Assert.AreEqual(1, g.AllConnections().Count());
        }

        [Test]
        public void connection_can_be_added_between_same_nodes()
        {
            var g = new AffectedGraph();
            g.AddNode(new AffectedGraphNode("foo", false, false, false, "name", "bar::foo", "assembly", "type", new List<TestDescriptor>(), false, false, 0));
            g.AddConnection("bar::foo", "bar::foo", false);
            Assert.AreEqual(1, g.AllConnections().Count());
        }
    }
}
