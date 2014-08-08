using System.Collections.Generic;
using Mono.Cecil;

namespace AutoTest.Minimizer
{
    public class ScanResult
    {
        public readonly IList<MemberAccess> MemberAccesses;
        public readonly MethodDefinition Definition;
        public readonly int Complexity;

        public ScanResult(IList<MemberAccess> memberAccesses, MethodDefinition definition, int complexity)
        {
            this.MemberAccesses = memberAccesses;
            
            Complexity = complexity;
            Definition = definition;
        }

    }
}