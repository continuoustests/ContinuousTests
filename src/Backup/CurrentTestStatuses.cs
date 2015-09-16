using System.Collections.Generic;
using AutoTest.Client.Logging;
using AutoTest.Messages;

namespace AutoTest.VS.RiskClassifier
{
    public static class CurrentTestStatuses
    {
        private static readonly Dictionary<string, TestStatusNode> _status = new Dictionary<string, TestStatusNode>();

        public static TestStatusNode GetStatusOf(string signature)
        {
            Logger.Write("Getting status of " + signature);
            TestStatusNode status;
            var search = RemoveParens(signature) + "()";
            Logger.Write("Searching for: " + search);
            if(_status.TryGetValue(search, out status))
            {
                return status;
            }
            search = RemoveParens(signature);
            Logger.Write("not found trying: " + search);
            if (_status.TryGetValue(search, out status))
            {
                return status;
            }
            Logger.Write("not found");
            foreach(var i in _status)
            {
                var tmp = "mspec" + signature;
                if(tmp.StartsWith(i.Key))
                {
                    return i.Value;
                }
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
                _status[str] = new TestStatusNode {Name=item.Test.Name, status = TestStatus.Fail, text = item.Test.Message};
                EntryCache.InvalidateNoUpdate(str);
            }
            foreach(var item in message.IgnoredToAdd)
            {
                var str = ConvertFromRunner(item);
                Logger.Write("Adding ignored test. " + str);
                _status[str] = new TestStatusNode { Name=item.Test.Name, status = TestStatus.Ignored, text = item.Test.Message };
                EntryCache.InvalidateNoUpdate(str);
            }
        }

        private static string ConvertFromRunner(CacheTestMessage s)
        {
            var x = s.Test.Name;
            var lastdot = x.LastIndexOf('.');
            if(lastdot == -1) return "";
            string ret;
            if(s.Test.Runner == TestRunner.MSpec)
            {
                ret = "mspecMachine.Specifications.It " + x.Substring(0, lastdot) + "." +
                      x.Substring(lastdot + 1, x.Length - lastdot - 1);
            } else if(s.Test.Runner == TestRunner.SimpleTesting)
            {
                ret = "Simple.Testing.ClientFramework.Specification " + x.Substring(0, lastdot) + "::" +
                      x.Substring(lastdot + 1, x.Length - lastdot - 1) + "()";
            } else if(x.Contains("("))
            {
                ret = "System.Void " + x.Substring(0, lastdot) + "::" +
                      RemoveParens(x.Substring(lastdot + 1, x.Length - lastdot - 1)) + "()";
            } 
            else
            {
                ret = "System.Void " + x.Substring(0, lastdot) + "::" +
                      x.Substring(lastdot + 1, x.Length - lastdot - 1) + "()";
                
            }
            return ret.Replace("+", "/");
        }

        private static string RemoveParens(string signature)
        {
            var start = signature.IndexOf('(');
            var end = signature.IndexOf(')');
            if(start >= 0 && end >= 0)
            {
                signature =  signature.Substring(0, start) + signature.Substring(end + 1, signature.Length - end - 1) ;
            }
            return signature;
        }


        public static void Clear()
        {
            _status.Clear();
        }
    }
}