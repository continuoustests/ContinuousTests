using System.Collections.Generic;
using Mono.Cecil;

namespace AssemblyChangeDetector.TestIdentifiers
{
    public interface ITestIdentifier
    {
        bool IsTest(MethodReference reference);
        List<MemberAccess> GetHiddenDependencies(MethodReference refernece);
    }
}