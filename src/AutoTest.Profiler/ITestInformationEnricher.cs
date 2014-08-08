using System.Collections.Generic;

namespace AutoTest.Profiler
{
    public interface ITestInformationEnricher
    {
        IEnumerable<ProfilerEntry> Enrich(IEnumerable<ProfilerEntry> entries);
        void ClearCache();
    }
}