namespace AutoTest.Minimizer
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

    class bar
    {
        public void Foo()
        {
            var f = new NodeConnection("", "", false);
        }
        public void Foo1()
        {
            var f = new NodeConnection("", "", false);
        }
        public void Foo2()
        {
            var f = new NodeConnection("", "", false);
        }
        public void Foo3()
        {
            var f = new NodeConnection("", "", false);
        }
        public void Foo4()
        {
            var f = new NodeConnection("", "", false);
        }
        public void Foo5()
        {
            var f = new NodeConnection("", "", false);
        }
    }
}