using System.Collections.Generic;
using AutoTest.Graphs;

namespace AutoTest.Minimizer.RiskClassifiers
{
    static class GraphNodeHashBuilder
    {
        public static Dictionary<string, RiskNode> GetHashFrom(AffectedGraph graph)
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
}