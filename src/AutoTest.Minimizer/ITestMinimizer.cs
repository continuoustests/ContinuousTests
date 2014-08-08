using System;
namespace AssemblyChangeDetector
{
    public interface ITestMinimizer
    {
        void DoInitialIndexOf(System.Collections.Generic.List<string> assemblies);
        System.Collections.Generic.List<TestEntry> GetTestsFor(System.Collections.Generic.List<string> assemblies);
        void LoadOldCachedFiles(System.Collections.Generic.List<string> assemblies);
    }
}
