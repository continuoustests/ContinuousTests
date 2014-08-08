using System.Collections.Generic;
using AutoTest.Profiler;
using Mono.Cecil;
namespace AutoTest.Minimizer
{
    public class CouplingCacheTestInformationEnricher : ITestInformationEnricher
    {
        private readonly CouplingCache _cache;

        Dictionary<string, StaticData> enrichCache = new Dictionary<string, StaticData>();
        
        public CouplingCacheTestInformationEnricher(CouplingCache cache)
        {
            _cache = cache;
        }

        public void ClearCache()
        {
            enrichCache.Clear();
        }

        public IEnumerable<ProfilerEntry> Enrich(IEnumerable<ProfilerEntry> entries)
        {
            foreach(var entry in entries)
            {
                StaticData found;
                if (enrichCache.TryGetValue(entry.Method, out found))
                {
                    entry.IsTest = found.IsTest;
                    entry.IsSetup = found.IsSetup;
                    entry.IsTeardown = found.IsTeardown;
                    entry.Found = true;
                    entry.Method = found.Name;
                    entry.IsFixtureConstructor = found.IsFixtureConstructor;
                    entry.Reference = found.Reference;
                }
                else
                {
                    string name = entry.Method;
                    _cache.Enrich(entry);
                        enrichCache.Add(name, new StaticData()
                                                  {
                                                      IsTest = entry.IsTest,
                                                      IsSetup = entry.IsSetup,
                                                      IsTeardown = entry.IsTeardown,
                                                      IsFixtureConstructor = entry.IsFixtureConstructor,
                                                      Reference = entry.Reference,
                                                      Name = entry.Method
                                                  });
                }
                yield return entry;
            }
        }
    }

    public class StaticData
    {
        public string Name;
        public bool IsTest;
        public bool IsSetup;
        public bool IsFixtureConstructor;
        public bool IsTeardown;
        public MemberReference Reference;
    }
}