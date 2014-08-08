using System.Collections.Generic;
using AutoTest.Graphs;

namespace AutoTest.Minimizer.RiskClassifiers
{
    public class CoverageDistanceAndComplexityGraphRiskClassifier : IGraphRiskClassifier
    {
        public int CalculateRiskFor(AffectedGraph graph)
        {
            //TODO make interface take the hash not the graph (so it canbe reused between strategies instead of built n times)
            if (graph == null) return 0;
            var root = graph.GetRootNode();
            if (root == null) return 0;
            var visited = new Dictionary<string, int>();
            var hash = GraphNodeHashBuilder.GetHashFrom(graph);
            var testsScore = RecurseFrom(root.FullName, hash, 0, 0, visited);
            var complexity = root.Complexity > 1.0m ? root.Complexity : 1.0m;
            var overallScore = testsScore/complexity;
            var ret = overallScore > 1.0m ? 1.0m : overallScore;
            return (int) (ret * 100m);
        }

        private decimal RecurseFrom(string fullName, Dictionary<string, RiskNode> graph,int depth, int complexity, Dictionary<string, int> visited)
        {
            RiskNode item;
            decimal underScore = 0;
            if (visited.ContainsKey(fullName)) return 0;
            visited.Add(fullName, 1);
            if (graph.TryGetValue(fullName, out item))
            {
                if (item.connections != null)
                {
                    var depthAddition = 0;
                    var complexityAddition = 0;
                        
                    if (item.Node != null)
                    {
                        if (item.Node.IsTest && item.Node.Profiled)
                        {
                            var penalty = 1;//complexity/(depth + 1.0m);
                            if (penalty < 1) penalty = 1;
                            return 1/penalty;
                        }
                        if (item.Node.IsInterface || item.Node.InTestAssembly)
                        {
                            //do nothing
                        }
                        else
                        {
                            depthAddition++;
                            complexityAddition = item.Node.Complexity*depth/5;
                        }
                    }
                    foreach (var i in item.connections)
                    {
                        underScore += RecurseFrom(i, graph, depth + depthAddition, complexity + complexityAddition, visited);
                    }
                }
            }
            return underScore;
        }
    }
}
