using System;
using System.Collections.Generic;

namespace AutoTest.Profiler
{
    enum States
    {
        Awaiting,
        InTest,
        InSetup,
        InTeardown
    }
    public class TestRunInfoAssembler
    {
        private readonly IContextChangeFinder _changeFinder;
        private int ignoredcount;

        public TestRunInfoAssembler() : this(new ForEveryFixtureContextChangeFinder())
        {   
        }

        public TestRunInfoAssembler(IContextChangeFinder changeFinder)
        {
            _changeFinder = changeFinder;
        }

        public IEnumerable<TestRunInformation> Assemble(IEnumerable<ProfilerEntry> entries)
        {
            var currentTests = new Dictionary<int, StateInfo>();
            var tmp = new List<TestRunInformation>();
            var enumerator = entries.GetEnumerator();
            ProfilerEntry entry;
            bool done = MoveEnumerator(enumerator, out entry);
            while(!done)
            {
               
                if(!currentTests.ContainsKey(entry.Thread))
                    currentTests.Add(entry.Thread, new StateInfo());
                var threadContext = currentTests[entry.Thread];
                bool unexpected;
                switch(threadContext.State)
                {
                    case States.Awaiting:
                        if (entry.Type == ProfileType.Enter && entry.IsTest)
                        {
                            if(_changeFinder.contextChangesWhen(entry))
                            {
                                foreach (var x in tmp) yield return x;
                                threadContext.Setups.Clear();
                                threadContext.Teardowns.Clear();
                                tmp.Clear();
                            }
                            threadContext.State = States.InTest;
                            threadContext.Info = new TestRunInformation
                                                     {
                                                         Name = entry.Runtime,
                                                         Setups = new List<CallChain>(threadContext.Setups.ToArray()),
                                                         Teardowns = new List<CallChain>()
                                                     };
                            threadContext.Awaiting = entry.Functionid;
                            BuildChain(threadContext.CallChain, entry, false);
                        }
                        if (entry.Type == ProfileType.Enter && entry.IsSetup)
                        {
                            if (_changeFinder.contextChangesWhen(entry))
                            {
                                foreach (var x in tmp) yield return x;
                                threadContext.Setups.Clear();
                                threadContext.Teardowns.Clear();
                                tmp.Clear();
                            }
                            threadContext.State = States.InSetup;
                            threadContext.Awaiting = entry.Functionid;
                            BuildChain(threadContext.CallChain, entry, false);
                        }
                        if (entry.Type == ProfileType.Enter && entry.IsTeardown)
                        {
                            threadContext.State = States.InTeardown;
                            threadContext.Awaiting = entry.Functionid;
                            BuildChain(threadContext.CallChain, entry, false);
                        }
                        if(!done)
                            done = MoveEnumerator(enumerator, out entry);
                        break;
                    case States.InTest:
                        unexpected = entry.IsSetup || entry.IsTeardown || (entry.Type == ProfileType.Enter && entry.IsTest);
                        var item1 = BuildChain(threadContext.CallChain, entry, unexpected);
                        if (entry.Functionid == threadContext.Awaiting || unexpected)
                        {
                            threadContext.State = States.Awaiting;
                            var testRunInformation = currentTests[entry.Thread];
                            testRunInformation.Info.TestChain = item1;
                            tmp.Add(testRunInformation.Info);
                        }
                        if(!unexpected)
                            done = MoveEnumerator(enumerator, out entry);
                        break;
                    case States.InSetup:
                        unexpected = entry.IsTest || entry.IsTeardown || (entry.IsSetup && entry.Type == ProfileType.Enter);
                        var item = BuildChain(threadContext.CallChain, entry, unexpected);
                        if(entry.Functionid == threadContext.Awaiting || unexpected)
                        {
                            threadContext.State = States.Awaiting;
                            threadContext.Setups.Push(item);
                        }
                        if(!unexpected)
                            done = MoveEnumerator(enumerator, out entry);
                        break;
                    case States.InTeardown:
                        unexpected = entry.IsTest || (entry.Type == ProfileType.Enter && entry.IsTeardown) || entry.IsSetup;
                        var td = BuildChain(threadContext.CallChain, entry, unexpected);
                        if (entry.Functionid == threadContext.Awaiting || unexpected)
                        {
                            threadContext.State = States.Awaiting;
                            foreach (var q in tmp) q.Teardowns.Add(td);
                        }
                        if(!unexpected)
                            done = MoveEnumerator(enumerator, out entry);
                        break;
                }
            }
            foreach (var x in tmp) yield return x;
            foreach(var item in currentTests.Values)
            {
                if (item.State == States.InTest)
                {
                    item.Info.TestChain = BuildChain(item.CallChain,
                                                     new ProfilerEntry(ProfileType.Leave, 0, 0, item.Awaiting, 0, "", ""),
                                                     true);
                    yield return item.Info;
                }

            }
        }

        private static bool MoveEnumerator(IEnumerator<ProfilerEntry> enumerator, out ProfilerEntry entry)
        {
            var hasmore = enumerator.MoveNext();
            entry = null;
            if(hasmore)
                entry = enumerator.Current;
            return !hasmore;
        }

        private CallChain BuildChain(Stack<CallChain> chain, ProfilerEntry entry, bool unExpected)
        {
            if(entry.Type == ProfileType.Leave || unExpected)
            {
                if(ignoredcount > 0)
                {
                    ignoredcount--;
                    return null;
                }
                while (chain.Count > 0)
                {
                    var current = chain.Pop();
                    if (current.FunctionId == entry.Functionid || chain.Count == 0)
                    {
                        current.EndTime = entry.Time;
                        return current;
                    }
                }
            }
            if(entry.Type == ProfileType.Enter)
            {
                if(chain.Count > 200)
                {
                    ignoredcount++;
                    return null;
                }
                var chainEntry = new CallChain(entry.Runtime.Replace(", ", ","),entry.Runtime, entry.Functionid);
                chainEntry.StartTime = entry.Time;
                if (chain.Count != 0)
                {
                    var parent = chain.Peek();
                    if(parent.Children.Count < 1000)
                         parent.AddChild(chainEntry);
                }
                chain.Push(chainEntry);
            }
            return null;
        }
    }
}