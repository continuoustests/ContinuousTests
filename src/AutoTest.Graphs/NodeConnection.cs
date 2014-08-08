namespace AutoTest.Graphs
{
    public class NodeConnection
    {
        public readonly string From;
        public readonly string To;
        public readonly bool IsForward;

        public NodeConnection(string @from, string to, bool isForward)
        {
            From = from;
            IsForward = isForward;
            To = to;
        }
    }
}