using System.Collections.Generic;
using Mono.Cecil;

namespace AutoTest.Minimizer.Extensions
{
    public static class AssemblyDefinitionExtensions
    {
        public static IEnumerable<MethodDefinition> AllNonIgnoredMethods(this AssemblyDefinition assemblyDefinition)
        {
            var ret = new List<MethodDefinition>();
            if (assemblyDefinition.ContainsIgnoreAttribute()) return ret;
            foreach(var module in assemblyDefinition.Modules)
            {
                if (module.ContainsIgnoreAttribute()) continue;
                foreach(var type in module.Types)
                {
                    GetMethodsFromType(ret, type);
                }
            }
            return ret;
        }

        public static IEnumerable<MemberReference> AllPubliclyVisisble(this AssemblyDefinition assemblyDefinition)
        {
            var ret = new List<MemberReference>();
            foreach (var module in assemblyDefinition.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (!type.IsPublic) continue;
                    ret.Add(type);
                    type.GetPublicsAndProtectedsFromType(ret);
                }
            }
            return ret;
        }


        private static void GetMethodsFromType(List<MethodDefinition> ret, TypeDefinition type)
        {
            if (type.ContainsIgnoreAttribute()) return;
            foreach(var method in type.GetMethods())
            {
                if (method.ContainsIgnoreAttribute()) continue;
                ret.Add(method);
            }
            
            foreach(var nested in type.NestedTypes)
            {
                GetMethodsFromType(ret, nested);
            }
        }
    }
}