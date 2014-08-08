using System.Collections.Generic;
using System.Linq;
using AutoTest.Graphs;
using AutoTest.Minimizer.RiskClassifiers;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests.Classifiers
{
    [TestFixture]
    public class CoverageDistanceAndCoverageGraphRiskClassifierTests
    {
        [Test]
        public void null_graph_returns_zero()
        {
            var classifier = new CoverageDistanceAndComplexityGraphRiskClassifier();
            Assert.AreEqual(0, classifier.CalculateRiskFor(null));
        }


        [Test]
        public void single_node_nodein_graph_that_is_non_covered_a_test()
        {
            var graph = new AffectedGraph();
            graph.AddNode(new AffectedGraphNode(displayName: "display",
                                                isInterface: false,
                                                isTest: true,
                                                isRootNode: true,
                                                name: "A name",
                                                fullName: "name",
                                                assembly: "foo.dll",
                                                type: "Type",
                                                testDescriptors: new List<TestDescriptor>(),
                                                isChange: false,
                                                inTestAssembly: false,
                                                complexity: 0));
            var classifier = new CoverageDistanceAndComplexityGraphRiskClassifier();
            Assert.AreEqual(0, classifier.CalculateRiskFor(graph));
        }

        [Test]
        public void single_node_nodein_graph_that_is_a_covered_test()
        {
            var graph = new AffectedGraph();
            graph.AddNode(new AffectedGraphNode(displayName: "display",
                                                isInterface: false,
                                                isTest: true,
                                                isRootNode: true,
                                                name: "A name",
                                                fullName: "name",
                                                assembly: "foo.dll",
                                                type: "Type",
                                                testDescriptors: new List<TestDescriptor>(),
                                                isChange: false,
                                                inTestAssembly: false,
                                                complexity: 0));
            graph.AllNodes().First().MarkAsProfiled();
            var classifier = new CoverageDistanceAndComplexityGraphRiskClassifier();
            Assert.AreEqual(100, classifier.CalculateRiskFor(graph));
        }        


        [Test]
        public void single_node_nodein_graph_that_is_not_a_test()
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
                                                isChange: false,
                                                inTestAssembly: false,
                                                complexity: 0));
            var classifier = new CoverageDistanceAndComplexityGraphRiskClassifier();
            Assert.AreEqual(0, classifier.CalculateRiskFor(graph));
        }
    }
}
