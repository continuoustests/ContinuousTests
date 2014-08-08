using System;

namespace AutoTest.Profiler.Tests
{
    public static class TestData
    {
        public static CallChain BuildChainWithPrefix(string prefix)
        {
            var chain = new CallChain(prefix + "Root", prefix + "RootR", 1) { StartTime=0,EndTime=10};
            chain.AddChild(new CallChain(prefix + "Child1", prefix + "Child1R", 2) { StartTime = 1, EndTime = 2 });
            chain.AddChild(new CallChain(prefix + "Child2", prefix + "Child2R", 3) { StartTime = 2, EndTime = 3 });
            var child = new CallChain(prefix + "Child3", prefix + "Child3R", 4) { StartTime = 4, EndTime = 7 };
            child.AddChild(new CallChain(prefix + "GrandChild", prefix + "GrandChildR", 5) { StartTime = 5, EndTime = 6 });
            chain.AddChild(child);
            return chain;
        }

        public static TestRunInformation BuildTestInformatonFor(string name, string prefix)
        {
            var ret = new TestRunInformation();
            ret.Name = name;
            ret.TestChain = BuildChainWithPrefix(prefix);
            return ret;
        }
    }
}