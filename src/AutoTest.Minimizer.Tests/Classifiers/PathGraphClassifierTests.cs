using System;
using System.Collections.Generic;
using AutoTest.Graphs;
using AutoTest.Minimizer.RiskClassifiers;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests.Classifiers
{
    [TestFixture]
    public class PathGraphClassifierTests
    {
        [Test]
        public void null_graph_returns_zero()
        {
            var classifier = new TestPathsGraphRiskClassifier();
            Assert.AreEqual(0, classifier.CalculateRiskFor(null));
        }

        [Test]
        public void adding_null_node_throws_argument_null_exception()
        {
            var graph = new AffectedGraph();
            //Assert.Throws<ArgumentNullException>(() => graph.AddNode(null));
        }

        [Test]
        public void Foo()
        {
        }

        [Test]
        public void graph_with_no_root_node_returns_zero()
        {
            var graph = new AffectedGraph();
            graph.AddNode(new AffectedGraphNode(displayName: "display",
                                                isInterface: false,
                                                isTest: true,
                                                isRootNode: false,
                                                name: "A name",
                                                fullName: "name",
                                                assembly: "foo.dll",
                                                type: "Type",
                                                testDescriptors: new List<TestDescriptor>(),
                                                isChange: false,
                                                inTestAssembly: true,
                                                complexity: 0));
            var classifier = new TestPathsGraphRiskClassifier();
            Assert.AreEqual(0, classifier.CalculateRiskFor(graph));
        }


        [Test]
        public void single_node_in_graph_that_is_a_test()
        {
            var graph = new AffectedGraph();
            graph.AddNode(new AffectedGraphNode(displayName:"display",
                                                isInterface: false,
                                                isTest: true,
                                                isRootNode: true,
                                                name: "A name",
                                                fullName: "name",
                                                assembly: "foo.dll",
                                                type: "Type",
                                                testDescriptors:new List<TestDescriptor>(),
                                                isChange: false, inTestAssembly:true, complexity:0));
            var classifier = new TestPathsGraphRiskClassifier();
            Assert.AreEqual(100, classifier.CalculateRiskFor(graph));
        }

        [Test]
        public void single_node_in_graph_that_is_not_a_test()
        {
            var graph = new AffectedGraph();
            graph.AddNode(new AffectedGraphNode(displayName: "display",
                                                isInterface: false,
                                                isTest: false,
                                                isRootNode: true,
                                                name: "A name",
                                                fullName: "name",
                                                assembly: "foo.dll",
                                                type: "Type",
                                                testDescriptors: new List<TestDescriptor>(),
                                                isChange: false, inTestAssembly: false, complexity:0));
            var classifier = new TestPathsGraphRiskClassifier();
            Assert.AreEqual(0, classifier.CalculateRiskFor(graph));
        }


        [Test]
        public void node_and_direct_test_in_graph()
        {
            var graph = new AffectedGraph();
            graph.AddNode(new AffectedGraphNode(displayName: "display",
                                                isInterface: false,
                                                isTest: false,
                                                isRootNode: true,
                                                name: "A name",
                                                fullName: "fullnamecode",
                                                assembly: "foo.dll",
                                                type: "Type",
                                                testDescriptors: new List<TestDescriptor>(),
                                                isChange: false, inTestAssembly: false, complexity:0));
            graph.AddNode(new AffectedGraphNode(displayName: "display",
                                                isInterface: false,
                                                isTest: true,
                                                isRootNode: true,
                                                name: "A name",
                                                fullName: "fullnametest",
                                                assembly: "foo.dll",
                                                type: "Type",
                                                testDescriptors: new List<TestDescriptor>(),
                                                isChange: false, inTestAssembly: true, complexity:0));
            graph.AddConnection("fullnamecode", "fullnametest", false);
            var classifier = new TestPathsGraphRiskClassifier();
            Assert.AreEqual(100, classifier.CalculateRiskFor(graph));
        }

        [Test]
        public void node_and_direct_call_from_test_assembly_in_graph()
        {
            var graph = new AffectedGraph();
            graph.AddNode(new AffectedGraphNode(displayName: "display",
                                                isInterface: false,
                                                isTest: false,
                                                isRootNode: true,
                                                name: "A name",
                                                fullName: "fullnamecode",
                                                assembly: "foo.dll",
                                                type: "Type",
                                                testDescriptors: new List<TestDescriptor>(),
                                                isChange: false, inTestAssembly: false, complexity: 0));
            graph.AddNode(new AffectedGraphNode(displayName: "display",
                                                isInterface: false,
                                                isTest: false,
                                                isRootNode: true,
                                                name: "A name",
                                                fullName: "fullnametestasm",
                                                assembly: "foo.dll",
                                                type: "Type",
                                                testDescriptors: new List<TestDescriptor>(),
                                                isChange: false, inTestAssembly: true, complexity:0));
            graph.AddNode(new AffectedGraphNode(displayName: "display",
                                                isInterface: false,
                                                isTest: true,
                                                isRootNode: true,
                                                name: "A name",
                                                fullName: "fullnametest",
                                                assembly: "foo.dll",
                                                type: "Type",
                                                testDescriptors: new List<TestDescriptor>(),
                                                isChange: false, inTestAssembly: true, complexity:0));
            graph.AddConnection("fullnamecode", "fullnametestasm", false);
            graph.AddConnection("fullnamecode", "fullnametest", false);
            var classifier = new TestPathsGraphRiskClassifier();
            Assert.AreEqual(100, classifier.CalculateRiskFor(graph));
        }

        [Test]
        public void node_and_direct_call_from_interface_in_graph()
        {
            var graph = new AffectedGraph();
            graph.AddNode(new AffectedGraphNode(displayName: "display",
                                                isInterface: false,
                                                isTest: false,
                                                isRootNode: true,
                                                name: "A name",
                                                fullName: "fullnamecode",
                                                assembly: "foo.dll",
                                                type: "Type",
                                                testDescriptors: new List<TestDescriptor>(),
                                                isChange: false, inTestAssembly: false, complexity: 0));
            graph.AddNode(new AffectedGraphNode(displayName: "display",
                                                isInterface: true,
                                                isTest: false,
                                                isRootNode: true,
                                                name: "A name",
                                                fullName: "fullnameinterface",
                                                assembly: "foo.dll",
                                                type: "Type",
                                                testDescriptors: new List<TestDescriptor>(),
                                                isChange: false, inTestAssembly: true, complexity: 0));
            graph.AddNode(new AffectedGraphNode(displayName: "display",
                                                isInterface: false,
                                                isTest: true,
                                                isRootNode: true,
                                                name: "A name",
                                                fullName: "fullnametest",
                                                assembly: "foo.dll",
                                                type: "Type",
                                                testDescriptors: new List<TestDescriptor>(),
                                                isChange: false, inTestAssembly: true, complexity: 0));
            graph.AddConnection("fullnamecode", "fullnametest", false);
            graph.AddConnection("fullnamecode", "fullnameinterface", false);
            var classifier = new TestPathsGraphRiskClassifier();
            Assert.AreEqual(100, classifier.CalculateRiskFor(graph));
        }
    }
}
