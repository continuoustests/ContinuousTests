using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class UnaryNode : Node
    {
        [CanBeNull]
        public string Prefix { get; set; }

        [CanBeNull]
        public string Suffix { get; set; }

        [NotNull]
        public Node Operand { get; set; }

        [CanBeNull]
        public string PrefixValue { get; set; }

        [CanBeNull]
        public string SuffixValue { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            if (!string.IsNullOrEmpty(Prefix))
            {
                walker(Prefix, PrefixValue, depth + 1);
            }
            Operand.Walk(walker, depth);
            if (!string.IsNullOrEmpty(Suffix))
            {
                walker(Suffix, SuffixValue, depth + 1);
            }
        }
    }
}