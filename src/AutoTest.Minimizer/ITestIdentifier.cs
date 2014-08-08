using System.Collections.Generic;
using AutoTest.Graphs;
using Mono.Cecil;

namespace AutoTest.Minimizer
{
    public interface ITestIdentifier
    {
        bool IsTest(MemberReference reference);
        List<MemberAccess> GetHiddenDependenciesForTest(MethodReference refernece);
        TestDescriptor GetTestDescriptorFor(MemberReference reference);
        string Tester { get; }
        bool IsTeardown(MethodReference target);
        bool IsSetup(MethodReference target);
        bool IsInFixture(MethodReference target);
        string GetSpecificallyMangledName(MethodReference target);
        bool IsProfilerTeardown(MethodReference target);
        bool IsProfilerSetup(MethodReference target);
        bool IsProfilerTest(MemberReference reference);
        
    }
}