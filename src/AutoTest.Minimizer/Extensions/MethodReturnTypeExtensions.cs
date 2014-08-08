using System.Collections.Generic;
using Mono.Cecil;

namespace AutoTest.Minimizer.Extensions
{
    static class MethodReturnTypeExtensions
    {
        public static IEnumerable<CustomAttribute> GetEffectiveAttributes(this MethodReturnType methodReturnType)
        {
            var method = (MethodDefinition) methodReturnType.Method;
            bool same = true;
            foreach (var def in method.GetInheritanceHierarchy())
            {
                foreach(var attr in def.MethodReturnType.CustomAttributes)
                {
                    if(same || attr.IsInheritable())
                    yield return attr;
                }
                same = false;
            }
        }
    }
}
