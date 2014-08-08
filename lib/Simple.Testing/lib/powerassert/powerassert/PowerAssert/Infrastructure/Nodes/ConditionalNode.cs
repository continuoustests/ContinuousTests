using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class ConditionalNode : Node
    {
        [NotNull]
        public Node Condition { get; set; }

        [NotNull]
        public Node TrueValue { get; set; }

        [NotNull]
        public Node FalseValue { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            walker("(");
            Condition.Walk(walker, depth + 1);
            walker(" ? ");
            TrueValue.Walk(walker, depth + 1);
            walker(" : ");
            FalseValue.Walk(walker, depth + 1);
            walker(")");
        }
    }
}