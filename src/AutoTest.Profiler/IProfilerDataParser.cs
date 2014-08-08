using System.Collections.Generic;
using System.IO;

namespace AutoTest.Profiler
{
    public interface IProfilerDataParser
    {
        IEnumerable<ProfilerEntry> Parse(Stream stream);
    }
}