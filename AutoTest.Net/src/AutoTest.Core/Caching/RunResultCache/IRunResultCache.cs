using AutoTest.Core.BuildRunners;
using AutoTest.Core.TestRunners;

namespace AutoTest.Core.Caching.RunResultCache
{
    public interface IRunResultCache
    {
        BuildItem[] Errors { get; }
        BuildItem[] Warnings { get; }
        TestItem[] Failed { get; }
        TestItem[] Ignored { get; }

        void EnabledDeltas();
        RunResultCacheDeltas PopDeltas();
    }
}