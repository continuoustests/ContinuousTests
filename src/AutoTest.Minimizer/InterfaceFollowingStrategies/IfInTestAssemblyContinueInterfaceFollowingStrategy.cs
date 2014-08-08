using System;
using Mono.Cecil;

namespace AutoTest.Minimizer.InterfaceFollowingStrategies
{
    public class IfInTestAssemblyContinueInterfaceFollowingStrategy : IInterfaceFollowingStrategy
    {
        CouplingCache _cache;
        public IfInTestAssemblyContinueInterfaceFollowingStrategy(CouplingCache cache)
        {
            _cache = cache;
        }

        public bool ShouldContinueAfter(MemberReference memberReference)
        {
            if (memberReference == null) return false;
            var t = (TypeDefinition)memberReference.DeclaringType;
            return _cache.AssemblyHasTests(t.Module.Assembly.FullName);
        }
    }
}
