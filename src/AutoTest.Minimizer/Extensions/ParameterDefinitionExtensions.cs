using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace AutoTest.Minimizer.Extensions
{
    public static class ParameterDefinitionExtensions
    {
        public static IEnumerable<CustomAttribute> GetEffectiveAttributes(this ParameterDefinition definition)
        {
            var paramIndex = definition.Method.Parameters.IndexOf(definition);
            var MethodDef = definition.Method as MethodDefinition;
            if(MethodDef == null) return definition.CustomAttributes; //can't walk
            return GetInheritedAttributes(MethodDef, paramIndex);
        }

        private static IEnumerable<CustomAttribute> GetInheritedAttributes(MethodDefinition definition, int paramIndex)
        {
            bool same = true;
            foreach (var method in definition.GetInheritanceHierarchy())
            {
                foreach (var attr in method.Parameters[paramIndex].CustomAttributes)
                {
                    if (same || attr.IsInheritable())
                    {
                        yield return attr;
                    }
                }
                same = false;
            }

        }

        public static TypeReference GetTypeWithGenericResolve(this ParameterDefinition definition)
        {
            return GetTypeWithGenericResolve(definition, null);
        }

        public static TypeReference GetTypeWithGenericResolve(this ParameterDefinition definition, GenericInstanceMethod genericInstanceMethod)
        {
            var ret = definition.ParameterType;
            if (definition.ParameterType.IsGenericParameter)
            {
                var f = (MethodReference)definition.Method;
                if (f.DeclaringType is GenericInstanceType)
                {
                    var position =
                        ((GenericInstanceType)f.DeclaringType).ElementType.GenericParameters.GetIndexByName(
                            definition.ParameterType.Name);
                    if (position != -1)
                        ret = ((GenericInstanceType)f.DeclaringType).GenericArguments[position];
                }
                if (genericInstanceMethod != null)
                {
                    var position =
                        f.GetElementMethod().GenericParameters.GetIndexByName(definition.ParameterType.Name);
                    if (position != -1)
                        ret = genericInstanceMethod.GenericArguments[position];
                }
            }
            return ret;
        }

    }
}