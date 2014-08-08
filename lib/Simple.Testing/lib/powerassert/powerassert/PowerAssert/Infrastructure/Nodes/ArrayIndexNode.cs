using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class ArrayIndexNode : Node
    {
        [NotNull]
        public Node Array { get; set; }
        
        [NotNull]
        public Node Index { get; set; }

        [NotNull]
        public string Value { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            Array.Walk(walker, depth + 1);
            walker("[", Value, depth);
            Index.Walk(walker, depth + 1);
            walker("]");
        }
    }
}