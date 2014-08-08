using System.Collections.Generic;
using AutoTest.Graphs;

namespace AutoTest.Minimizer.RiskClassifiers
{
    internal class RiskNode
    {
        public List<string> connections = new List<string>();
        public AffectedGraphNode Node;
    }
}