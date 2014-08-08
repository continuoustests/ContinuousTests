using System.Collections.Generic;
using AutoTest.Graphs;
using Mono.Cecil;

namespace AutoTest.Minimizer
{
    internal interface ITestSearchStrategy
    {
        IEnumerable<TestDescriptor> GetTestsForChanges(IEnumerable<Change<MethodReference>> changes);
    }
}