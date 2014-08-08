using System;
using System.Collections.Generic;

namespace AutoTest.Minimizer
{
    public class AffectedGraph
    {
        private readonly Dictionary<string, AffectedGraphNode> _nodes = new Dictionary<string, AffectedGraphNode>();
        private readonly List<NodeConnection> _connections = new List<NodeConnection>();

        public void AddConnection(string from, string to, bool isForward)
        {
            if(!_nodes.ContainsKey(from) || !_nodes.ContainsKey(to))
            {
                throw new InvalidConnectionAddedException(from, to);
            }
            _connections.Add(new NodeConnection(from, to, isForward));
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
            if (!_nodes.ContainsKey(affectedGraphNode.FullName))
            {
                _nodes.Add(affectedGraphNode.FullName, affectedGraphNode);
            }
        }

        public IEnumerable<AffectedGraphNode> AllNodes()
        {
            return _nodes.Values;
        }

        public IEnumerable<NodeConnection> AllConnections()
        {
            return _connections;
        }

        public bool ContainsNode(string key)
        {
            return _nodes.ContainsKey(key);
        }
    }
}