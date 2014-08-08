using System.Collections.Generic;
using AutoTest.Client.Logging;
using AutoTest.Messages;

namespace AutoTest.VS.RiskClassifier
{
    public static class FailingTests
    {
        private static readonly Dictionary<string, TestStatusNode> _status = new Dictionary<string, TestStatusNode>();

        public static TestStatusNode GetStatusOf(string signature)
        {
            TestStatusNode status;
            if(_status.TryGetValue(signature, out status))
            {
                return status;
            }
            return new TestStatusNode {status = TestStatus.Pass, text =""};
        }


        public static void UpdateWith(CacheMessages message)
        {
            foreach (var item in message.TestsToRemove)
            {
                var str = ConvertFromRunner(item);
                Logger.Write("removing test. " + str);
                _status.Remove(str);
                EntryCache.InvalidateNoUpdate(str);
            }
            foreach(var item in message.FailedToAdd)
            {
                var str = ConvertFromRunner(item);
                Logger.Write("Adding failed test. " + str);
                _status[str] = new TestStatusNode {status = TestStatus.Fail, text = item.Test.Message};
                EntryCache.InvalidateNoUpdate(str);
            }
            foreach(var item in message.IgnoredToAdd)
            {
                var str = ConvertFromRunner(item);
                Logger.Write("Adding ignored test. " + str);
                _status[str] = new TestStatusNode { status = TestStatus.Ignored, text = item.Test.Message };
                EntryCache.InvalidateNoUpdate(str);
            }
        }

        private static string ConvertFromRunner(CacheTestMessage s)
        {
            var x = s.Test.Name;
            var lastdot = x.LastIndexOf('.');
            if(lastdot == -1) return "";
            string ret;
            if (s.Test.Runner != TestRunner.SimpleTesting)
            {
                ret = "System.Void " + x.Substring(0, lastdot) + "::" +
                      x.Substring(lastdot + 1, x.Length - lastdot - 1) + "()";
            } else if(s.Test.Runner == TestRunner.MSpec)
            {
                ret = "Machine.Specifications.It " + x.Substring(0, lastdot) + "::" +
                      x.Substring(lastdot + 1, x.Length - lastdot - 1);
            } else
            {
                ret = "Simple.Testing.ClientFramework.Specification " + x.Substring(0, lastdot) + "::" +
                      x.Substring(lastdot + 1, x.Length - lastdot - 1) + "()";
            }
            return ret.Replace("+", "/");
        }

        public static void Clear()
        {
            _status.Clear();
        }
    }
}