using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class ConstantNode : Node
    {
        [NotNull]
        public string Text { get; set; }

        [CanBeNull]
        public string Value { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            walker(Text.CleanupCamelCasing(), Value, depth);
        }
    }
}