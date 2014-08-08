using AutoTest.Minimizer.Extensions;
using Mono.Cecil;

namespace AutoTest.Minimizer.TestIdentifiers
{
    internal class MethodBuildingHelper
    {
        public static MemberAccess BuildMemberAccess(MethodReference refernece, MethodDefinition method)
        {
            var derivedType = refernece.DeclaringType.ThreadSafeResolve();
            var baseType = method.DeclaringType;
            if (baseType.HasGenericParameters)
            {
                var genericMethod = method.MakeReference(derivedType.GetInInheritance(baseType));
                return new MemberAccess(genericMethod, true, true, null, genericMethod, false);
            }
            return new MemberAccess(method, true, true, null, method, false);
        }
    }
}