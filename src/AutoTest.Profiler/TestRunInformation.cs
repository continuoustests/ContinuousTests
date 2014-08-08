using System.Collections.Generic;
using System.Linq;

namespace AutoTest.Profiler
{
    public class TestRunInformation
    {
        public CallChain TestChain { get; set; }
        public List<CallChain> Teardowns { get; set; }
        public List<CallChain> Setups { get; set; }

        public string Name { get; set; }

        public TestRunInformation()
        {
            Setups = new List<CallChain>();
            Teardowns = new List<CallChain>();
        }

        public void AddSetup(CallChain chain)
        {
            Setups.Add(chain);
        }

        public void AddTearDown(CallChain chain)
        {
            Teardowns.Add(chain);
        }

        public IEnumerable<string> IterateAll()
        {
            foreach (var callChain in Setups)
            {
                foreach (var item in callChain.IterateAll()) yield return item;
            }
            if (TestChain != null)
            {
                foreach (var item in TestChain.IterateAll()) yield return item;
            }
            foreach (var callChain in Teardowns)
            {
                foreach (var item in callChain.IterateAll()) yield return item;
            }
        }

        public IEnumerable<CallChain> IterateNodes()
        {
            foreach (var callChain in Setups)
            {
                foreach (var item in callChain.IterateNodes()) yield return item;
            }
            if (TestChain != null)
            {
                foreach (var item in TestChain.IterateNodes()) yield return item;
            }
            foreach (var callChain in Teardowns)
            {
                foreach (var item in callChain.IterateNodes()) yield return item;
            }
        }


        private static string BuildString(TestRunInformation entry)
        {
            var ret = "\n\n";
            ret += "name = " + entry.Name;
            ret += "Setups=" + entry.Setups.Select(BuildChain);
            ret += "Test=" + BuildChain(entry.TestChain);
            ret += entry.Teardowns.Select(BuildChain);
            return ret;
        }

        private static string BuildChain(CallChain chain)
        {
            if (chain == null) return "NULL!";
            string ret = chain.Name;
            ret += chain.Children.Select(BuildChain);
            return ret;
        }

        public override string ToString()
        {
            return BuildString(this);
        }
    }
}