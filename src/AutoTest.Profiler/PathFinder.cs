using System.Collections.Generic;

namespace AutoTest.Profiler
{
    public static class PathFinder
    {


        public static IEnumerable<IEnumerable<string>> FindPathsTo(TestRunInformation test, string method)
        {
            int x = 5;
            foreach (var setup in test.Setups)
            {
                foreach (var p in RecurseTo(setup, method, GetTestNode(test))) yield return p;
            }
            foreach (var p in RecurseTo(test.TestChain, method, null)) yield return p;
            foreach (var teardown in test.Teardowns)
            {
                foreach (var p in RecurseTo(teardown, method, GetTestNode(test))) yield return p;
            }
        }

        private static string GetTestNode(TestRunInformation test)
        {
            if(test.TestChain != null)
            {
                return test.TestChain.Name;
            }
            return "";
        }

        private static IEnumerable<IEnumerable<string>> RecurseTo(CallChain chain, string method, string test)
        {
            var stack = new Stack<string>();
            if(test != null)
            {
                stack.Push(test);
            }
            return RecurseToInternal(stack, chain, method);
        }
        private static IEnumerable<IEnumerable<string>> RecurseToInternal(Stack<string> currentStack, CallChain chain, string method)
        {
            if (chain == null) yield break;
            currentStack.Push(chain.Name);
            if (chain.Name == method)
            {
                yield return currentStack.ToArray();
            }
            foreach (var c in chain.Children)
            {
                foreach (var item in RecurseToInternal(currentStack, c, method)) yield return item;
            }
            currentStack.Pop();
        }
    }
}