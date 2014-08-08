using System.Collections.Generic;
using AutoTest.Graphs;

namespace AutoTest.Minimizer.RiskClassifiers
{
    public class TestPathsGraphRiskClassifier : IGraphRiskClassifier
    {
        public int CalculateRiskFor(AffectedGraph graph)
        {
            if (graph == null) return 0;
            var root = graph.GetRootNode();
            if (root == null) return 0;
            var connections = GraphNodeHashBuilder.GetHashFrom(graph);
            var risk = RecurseRisk(root.FullName, connections, new Dictionary<string, bool>());
            if (risk.nottested + risk.tested == 0) return 0;
            return (int)(risk.tested / (decimal)(risk.nottested + risk.tested) * 100.0m);
        }

        private RiskCount RecurseRisk(string fullName, Dictionary<string, RiskNode> graph, Dictionary<string, bool> visited)
        {
            var ret = new RiskCount();
            RiskNode item;
            if (visited.ContainsKey(fullName)) return ret;
            visited.Add(fullName, true);
            if (graph.TryGetValue(fullName, out item))
            {
                if (item.connections != null)
                {
                    foreach (var i in item.connections)
                    {
                        var x = RecurseRisk(i, graph, visited);
                        ret.tested += x.tested;
                        ret.nottested += x.nottested;
                    }
                }
                if (item.Node != null)
                {
                    if (item.Node.IsInterface)
                    {
                        //do nothing
                    }
                    else if (item.Node.IsTest || ret.tested > 0)
                    {
                        ret.tested += 1;
                    }
                    else if (item.Node.InTestAssembly)
                    {
                    }
                    else
                    {
                        ret.nottested += 1;
                    }
                }
            }
            return ret;
        }
    }

    struct RiskCount
    {
        public int nottested;
        public int tested;
    }
}
