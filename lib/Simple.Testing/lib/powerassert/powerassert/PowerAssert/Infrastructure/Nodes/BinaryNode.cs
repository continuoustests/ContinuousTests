using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class BinaryNode : Node
    {
        [NotNull]
        public Node Left { get; set; }

        [NotNull]
        public Node Right { get; set; }

        [NotNull]
        public string Operator { get; set; }

        [CanBeNull]
        public string Value { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            Left.Walk(walker, depth+1);
            walker(" ");
            walker(Operator, Value, depth);
            walker(" ");
            Right.Walk(walker, depth + 1);
        }
    }
}