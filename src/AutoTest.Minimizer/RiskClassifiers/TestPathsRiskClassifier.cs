using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Graphs;

namespace AutoTest.Minimizer.RiskClassifiers
{
    struct RiskCount
    {
        public int nottested;
        public int tested;
    }
    public class TestPathsRiskClassifier : IGraphRiskClassifier
    {
        public int CalculateRiskFor(AffectedGraph g)
        {
            var root = g.AllNodes().FirstOrDefault(x => x.IsRootNode);
            if (root == null) return -1;
            var connections = GetHashFrom(g);
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
                foreach (var i in item.connections)
                {
                    var x = RecurseRisk(i, graph, visited);
                    ret.tested += x.tested;
                    ret.nottested += x.nottested;
                }
                if (item.Node.IsInterface)
                {
                    //do nothing
                }
                else if (item.Node.IsTest || ret.tested > 0)
                {
                    ret.tested += 1;
                }
                else if (item.Node.InTestAssembly) { }
                else
                {
                    ret.nottested += 1;
                }
            }
            return ret;
        }

        private Dictionary<string, RiskNode> GetHashFrom(AffectedGraph graph)
        {
            var ret = new Dictionary<string, RiskNode>();
            foreach (var node in graph.AllNodes())
            {
                ret.Add(node.FullName, new RiskNode() { Node = node });
            }
            foreach (var connection in graph.AllConnections())
            {
                RiskNode item;
                if (!ret.TryGetValue(connection.From, out item))
                {
                    ret.Add(connection.From, item);
                }
                item.connections.Add(connection.To);
            }
            return ret;
        }
    }



    internal class RiskNode
    {
        public List<string> connections = new List<string>();
        public AffectedGraphNode Node;
    }
}
