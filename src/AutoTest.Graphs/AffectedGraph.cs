using System.Collections.Generic;
using System.Linq;

namespace AutoTest.Graphs
{
    public class AffectedGraph
    {
        private readonly Dictionary<string, AffectedGraphNode> _nodes = new Dictionary<string, AffectedGraphNode>();
        private readonly Dictionary<string, NodeConnection> _connections = new Dictionary<string, NodeConnection>();

        public void AddConnection(string from, string to, bool isForward)
        {
            if (from == null || to == null) return;
            lock (this)
            {
                if (!_nodes.ContainsKey(from) || !_nodes.ContainsKey(to))
                {
                    return;
                }
                var key = from + "!" + to;
                if(!_connections.ContainsKey(key))
                    _connections.Add(key, new NodeConnection(from, to, isForward));
            }
        }

        public AffectedGraphNode GetNode(string name)
        {
            AffectedGraphNode node;
            _nodes.TryGetValue(name, out node);
            return node;
        }

        public void AddNode(AffectedGraphNode affectedGraphNode)
        {
            if (affectedGraphNode == null) return;
            lock (this)
            {
                if (!_nodes.ContainsKey(affectedGraphNode.FullName))
                {
                    _nodes.Add(affectedGraphNode.FullName, affectedGraphNode);
                }
            }
        }

        public IEnumerable<AffectedGraphNode> AllNodes()
        {
            return _nodes.Values;
        }

        public IEnumerable<NodeConnection> AllConnections()
        {
            return _connections.Values;
        }

        public bool ContainsNode(string key)
        {
            return _nodes.ContainsKey(key);
        }

        public AffectedGraph Merge(AffectedGraph graph2)
        {
            var ret = new AffectedGraph();
            AddIfNotExist(ret, this.AllNodes());
            AddIfNotExist(ret, graph2.AllNodes());
            AddConnections(ret, this.AllConnections());
            AddConnections(ret, graph2.AllConnections());
            return ret;
        }

        private void AddConnections(AffectedGraph to, IEnumerable<NodeConnection> allConnections)
        {
            foreach(var c in allConnections) to.AddConnection(c.From, c.To, c.IsForward);
        }

        private void AddIfNotExist(AffectedGraph to, IEnumerable<AffectedGraphNode> nodes)
        {
            int x = 5;
            foreach (var node in nodes)
            {
                if (!to.ContainsNode(node.FullName))
                {
                    to.AddNode(node);
                }
            }
        }

        public AffectedGraphNode GetRootNode()
        {
            return AllNodes().FirstOrDefault(x => x.IsRootNode);
        }
    }
}