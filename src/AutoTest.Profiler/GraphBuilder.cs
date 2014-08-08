using System.Collections.Generic;
using AutoTest.Graphs;

namespace AutoTest.Profiler
{
    public class GraphBuilder
    {
        public static AffectedGraph BuildGraphFor(IEnumerable<IEnumerable<string>> paths)
        {
            bool first = true;
            var ret = new AffectedGraph();
            foreach(var path in paths)
            {
                string last = null;
                foreach(var node in path)
                {
                    if(!ret.ContainsNode(node))
                    {
                        ret.AddNode(new AffectedGraphNode(node, false, false, first, node, node, "", "", new List<TestDescriptor>(), false, false, 0));
                        first = false;
                    }
                    if(last != null)
                    {
                        ret.AddConnection(last, node, false);
                    }
                    last = node;
                }
            }
            return ret;
        }
    }
}