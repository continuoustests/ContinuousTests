using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace AutoTest.Minimizer.Extensions
{
    public static class MethodDefinitionExtensions
    {
        public static bool IsGeneratedMethod(this MethodDefinition methodDefinition)
        {
            return methodDefinition.ContainsAttribute("CompilerGeneratedAttribute");
        }

        public static IEnumerable<CustomAttribute> GetEffectiveAttributes(this MethodDefinition definition)
        {
            var same = true;
            foreach(var def in definition.GetInheritanceHierarchy())
            {
                foreach (var attr in def.CustomAttributes)
                {
                    if(same || attr.IsInheritable()) 
                        yield return attr;
                }
                same = false;
            }    
        }

        public static IEnumerable<MethodDefinition> GetInheritanceHierarchy(this MethodDefinition definition)
        {
            bool first = true;
            foreach(var t in definition.DeclaringType.WalkInheritance())
            {
                var def = t.ThreadSafeResolve();
                var cur = def == null ? null : def.GetMethodDefinitionMatching(definition, first);
                if (cur != null) yield return cur;
                else yield break;
                first = false;
            }
        }

        public static bool IsBaseMethodOf(this MethodDefinition definition, MethodDefinition other)
        {
            return other.GetInheritanceHierarchy().Contains(definition);
        }

        public static string GetChangeCacheName(this MethodDefinition node)
        {
            string ret = "";
            if (node.IsStatic) ret += "s ";
            if(node.HasGenericParameters)
            {
                ret += node.GenericParameters.Count;
            }
            ret += node.FullName;
            return ret;
        }

        public static bool MethodMatch(this MethodDefinition candidate, MethodDefinition method)
        {
            return MethodMatch(candidate, method, false);
        }

        public static bool MethodMatch(this MethodDefinition candidate, MethodDefinition method, bool AllowShadows) {
            if (!candidate.IsVirtual && !AllowShadows)
                return false; 
            if (candidate.Name != method.Name)
                return false;

            if (!TypeMatch(candidate.ReturnType, method.ReturnType))
                return false;

            if (candidate.Parameters.Count != method.Parameters.Count)
                return false;

            for (int i = 0; i < candidate.Parameters.Count; i++)
                if (!TypeMatch(candidate.Parameters[i].ParameterType, method.Parameters[i].ParameterType))
                    return false;

            return true;
        }

        static bool TypeMatch(TypeReference a, TypeReference b)
        {
            if (a is GenericParameter)
                return true;

            if (a is TypeSpecification || b is TypeSpecification)
            {
                if (a.GetType() != b.GetType())
                    return false;

                return TypeMatch((TypeSpecification)a, (TypeSpecification)b);
            }

            return a.FullName == b.FullName;
        }

        static bool TypeMatch(TypeSpecification a, TypeSpecification b)
        {
            if (a is GenericInstanceType)
                return TypeMatch((GenericInstanceType)a, (GenericInstanceType)b);

            return TypeMatch(a.ElementType, b.ElementType);
        }

        static bool TypeMatch(GenericInstanceType a, GenericInstanceType b)
        {
            if (!TypeMatch(a.ElementType, b.ElementType))
                return false;

            if (a.GenericArguments.Count != b.GenericArguments.Count)
                return false;

            if (a.GenericArguments.Count == 0)
                return true;

            for (int i = 0; i < a.GenericArguments.Count; i++)
                if (!TypeMatch(a.GenericArguments[i], b.GenericArguments[i]))
                    return false;

            return true;
        }

    }
}