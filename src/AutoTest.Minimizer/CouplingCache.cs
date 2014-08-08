using System;
using System.Collections.Generic;
using System.Linq;
using AutoTest.Graphs;
using AutoTest.Minimizer.Extensions;
using AutoTest.Profiler;
using Mono.Cecil;

namespace AutoTest.Minimizer
{
    public class CouplingCache
    {
        private readonly IEnumerable<ITestIdentifier> _testIdentifiers;
        private readonly Dictionary<string, CouplingCacheNode> _afferentCache = new Dictionary<string, CouplingCacheNode>();
        private readonly Dictionary<string, CouplingCacheNode> _efferentCache = new Dictionary<string, CouplingCacheNode>();
        private readonly Dictionary<string, bool> _testAssemblies = new Dictionary<string, bool>();

        public CouplingCache(IEnumerable<ITestIdentifier> testIdentifiers)
        {
            if (testIdentifiers == null) throw new ArgumentNullException("testIdentifiers");
            _testIdentifiers = testIdentifiers;
        }

        public int Count
        {
            get { return _efferentCache.Count; }
            
        }

        public void SetCoupling(ScanResult result)
        {
            IEnumerable<MemberAccess> to = result.MemberAccesses;
            var from = result.Definition;
            IEnumerable<MemberAccess> all = _testIdentifiers.Aggregate(to, (current, ider) => current.Concat(ider.GetHiddenDependenciesForTest(from)));
            var isTest = _testIdentifiers.IsTest(from);
            var isSetup = _testIdentifiers.IsSetup(from);
            var isTeardown = _testIdentifiers.IsTeardown(from);
            var isRelatedToTest = isTeardown || isSetup || isTest;
            if (isRelatedToTest)
            {
                var asmName = from.DeclaringType.Module.Assembly.FullName;
                if (!_testAssemblies.ContainsKey(asmName))
                {
                    lock (_testAssemblies)
                    {
                        if (!_testAssemblies.ContainsKey(asmName))
                        {
                            _testAssemblies.Add(asmName, true);
                        }
                    }
                }
            }
            var allSynonyms = SynonymFinder.FindSynonymsFor(from);
            allSynonyms.Add(from);
            AddEfferentCoupling(from, all, result.Complexity);
            foreach(var entry in all)
            {
                AddAfferentCoupling(entry.MemberReference, new [] {new MemberAccess(from, entry.IsReadOnly, entry.IsSelfCall, entry.OnField, entry.ActualMethodDefinition, entry.IsLocalVariable)});
            }
        }

        private void AddAfferentCoupling(MemberReference from, IEnumerable<MemberAccess> to)
        {
            CouplingCacheNode node;
            var toFullName = from.GetCacheName();
            if (!_afferentCache.TryGetValue(toFullName, out node))
            {
                lock (_afferentCache)
                {
                    if (!_afferentCache.TryGetValue(toFullName, out node))
                    {
                        node = new CouplingCacheNode(toFullName, from);
                        _afferentCache.Add(toFullName, node);
                    }
                }
            }
            lock (node)
            {
                foreach (var current in to)
                {
                    node.AddCoupling(new Coupling(current.MemberReference.GetCacheName(), current.IsReadOnly, false,
                                                  current.IsSelfCall, current.OnField, current.ActualMethodDefinition));
                }
            }
        }

        public bool AssemblyHasTests(string name)
        {
            return _testAssemblies.ContainsKey(name);
        }

        private void AddEfferentCoupling(MethodReference from, IEnumerable<MemberAccess> to, int complexity)
        {
            CouplingCacheNode node;
            var fromFullName = from.GetCacheName();
            if (!_efferentCache.TryGetValue(fromFullName, out node))
            {
                lock (_efferentCache)
                {
                    if (!_efferentCache.TryGetValue(fromFullName, out node))
                    {
                        node = new CouplingCacheNode(fromFullName, from);
                        node.Complexity = complexity;
                        _efferentCache.Add(fromFullName, node);
                    }
                }
            }
            lock (node)
            {
                foreach (var current in to)
                {
                    node.AddCoupling(new Coupling(current.MemberReference.GetCacheName(), current.IsReadOnly, false,
                                                  current.IsSelfCall, current.OnField, current.ActualMethodDefinition));
                }
            }
        }

        private static CouplingCacheNode GetNode(string from, IDictionary<string, CouplingCacheNode> cache)
        {
            if (cache == null) throw new ArgumentNullException("cache");
            CouplingCacheNode node;
            if (!cache.TryGetValue(from, out node))
            {
                throw new CouplingEntryDoesNotExistsException();
            }
            return node;
        }

        public IEnumerable<Coupling> GetAfferentCoupling(string from)
        {
            if (from == null) throw new ArgumentNullException("from");
            var node = GetNode(from, _afferentCache);
            return node.Couplings;
        }

        public IEnumerable<Coupling> GetEfferentCoupling(string to)
        {
            if (to == null) throw new ArgumentNullException("to");
            var node = GetNode(to, _efferentCache);
            return node.Couplings;
        }

        public void Invalidate(string method)
        {
            if (method == null) throw new ArgumentNullException("method");
            CouplingCacheNode node;
            if (_efferentCache.TryGetValue(method, out node))
            {
                _efferentCache.Remove(method);
                foreach (var current in node.Couplings)
                {
                    CouplingCacheNode coupled;
                    if (_afferentCache.TryGetValue(current.To, out coupled))
                    {
                        for (int i = 0; i < coupled.Couplings.Count; i++)
                        {
                            if (coupled.Couplings[i].To == method)
                            {
                                coupled.Couplings.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
        }

        public CouplingCacheNode TryGetAfferentCouplingNode(string fullName)
        {
            CouplingCacheNode node;
            _afferentCache.TryGetValue(fullName, out node);
            return node;
        }

        public CouplingCacheNode TryGetEfferentCouplingNode(string fullName)
        {
            CouplingCacheNode node;
            _efferentCache.TryGetValue(fullName, out node);
            return node;
        }

        public void Clear()
        {
            _afferentCache.Clear();
            _efferentCache.Clear();
            _testAssemblies.Clear();
        }

        public void Enrich(ProfilerEntry entry)
        {
            var node = TryGetEfferentCouplingNode(entry.Runtime.Replace('+', '/'));
            entry.Found = node != null;
            if (node != null)
            {
                var tmpname = _testIdentifiers.GetSpecificallyMangledName(node.MemberReference as MethodReference);
                
                if(tmpname != null)
                {
                    entry.Method = tmpname;
                    entry.Runtime = tmpname;
                }
                entry.IsTest = _testIdentifiers.IsProfilerTest(node.MemberReference as MethodReference);
                entry.IsSetup = _testIdentifiers.IsProfilerSetup(node.MemberReference as MethodReference);
                entry.IsTeardown = _testIdentifiers.IsProfilerTeardown(node.MemberReference as MethodReference);
                entry.IsFixtureConstructor = IsFixtureConstructor(node);
                entry.Reference = node.MemberReference;
            }
        }

        public bool IsFixtureConstructor(CouplingCacheNode node)
        {
            var methodref = node.MemberReference as MethodReference;
            if (methodref == null) return false;
            var methoddef = methodref.ThreadSafeResolve();
            if (!methoddef.IsConstructor) return false;
            return _testIdentifiers.IsInFixture(methoddef);
        }

        public void EnrichGraph(AffectedGraph graph)
        {
            foreach (var node in graph.AllNodes())
            {
                var cachenode = TryGetEfferentCouplingNode(node.FullName.Replace('+', '/'));
                if (cachenode != null)
                {
                    var r = cachenode.MemberReference;
                    if (r != null)
                    {
                        node.DisplayName = r.DeclaringType.Name + "::" + r.Name;
                        node.Assembly = r.Module.Assembly.FullName;
                        node.IsInterface = r.DeclaringType.ThreadSafeResolve().IsInterface;
                    }
                    node.Profiled = true;
                    node.IsTest = _testIdentifiers.IsTest(cachenode.MemberReference);
                }
            }
        }
    }


    public class ForEveryFixtureConstructorOrFixtureChangeContextChangeFinder : IContextChangeFinder
    {
        private TypeDefinition last = null;

        public bool contextChangesWhen(ProfilerEntry entry)
        {
            if (entry.IsSetup)
            {
                return true;
            }
            return entry.Type == ProfileType.Enter && entry.IsFixtureConstructor;
        }
    }
}