using System.Collections.Generic;

namespace AutoTest.Profiler
{
    public class CallChain
    {
        public readonly string Name;
        public readonly long FunctionId;
        public double StartTime;
        public double EndTime;
         
        public List<CallChain> Children { get { return _children; } }

        private readonly List<CallChain> _children = new List<CallChain>();
        public readonly string Runtime;

        public void AddChild(CallChain child)
        {
            _children.Add(child);
        }

        public CallChain(string name, string runtime, long functionId)
        {
            Name = name;
            Runtime = runtime;
            FunctionId = functionId;
        }

        public IEnumerable<CallChain> IterateNodes()
        {
            yield return this;
            foreach (var item in Children)
            {
                foreach (var i in item.IterateNodes()) yield return i;
            }
        }

        public IEnumerable<string> IterateAll()
        {
            yield return Name;
            foreach(var item in Children)
            {
                foreach (var name in item.IterateAll()) yield return name;
            }
        }

    }
}