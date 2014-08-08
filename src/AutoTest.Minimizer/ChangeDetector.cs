using System.Collections.Generic;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;

namespace AutoTest.Minimizer
{
    public class ChangeDetector
    {
        public static void Foo() {}
        public IEnumerable<Change<MethodReference>> GetChangesBetween(AssemblyDefinition oldAssembly, AssemblyDefinition newAssembly)
        {
            var changeDetector = new MethodILChangeDetector();
            var oldCache = AssemblyTypeMethodMapBuilder.BuildFor(oldAssembly);
            var newCache = AssemblyTypeMethodMapBuilder.BuildFor(newAssembly);
            var differences = MapKeyDifferenceFinder.GetChangesBetween(oldCache, newCache);
            foreach (var method in newAssembly.AllNonIgnoredMethods())
            {
                var current = oldAssembly.MainModule.GetType(method.DeclaringType.FullName);
                if (current == null) continue;
                if (method.ContainsIgnoreAttribute()) continue;
                foreach (var oldMethod in current.Methods)
                {
                    if (oldMethod.GetChangeCacheName() == method.GetChangeCacheName())
                    {
                        if (changeDetector.AreDifferentIL(oldMethod,
                                                          method))
                        {
                            differences.Add(
                                            new Change<MethodReference>(
                                ChangeType.Modify, method.ThreadSafeResolve()));
                            break;
                        }
                    }
                }
            }
            differences = IEnumerableResolver.ResolveCompilerGeneratedCode(differences);
            return differences;
        }

    }

    public static class IEnumerableResolver
    {
        public static List<Change<MethodReference>> ResolveCompilerGeneratedCode(IEnumerable<Change<MethodReference>> differences)
        {
            var ret = new List<Change<MethodReference>>();
            foreach (var change in differences)
            {
                var resolved = change.ItemChanged.ThreadSafeResolve();
                var methodName = GetOtherMethodNameFrom(resolved.DeclaringType.Name);
                if(methodName == resolved.DeclaringType.Name)
                {
                    ret.Add(change);
                    continue;
                }
                foreach (var c in GetMethodsByName(change.ItemChanged, methodName))
                {
                    ret.Add(c);
                }
            }
            return ret;
        }


        private static IEnumerable<Change<MethodReference>> GetMethodsByName(MethodReference itemChanged, string methodName)
        {
            
            var ret = new List<Change<MethodReference>>();
            var parent = itemChanged.DeclaringType.ThreadSafeResolve();
            if (parent.DeclaringType == null) return ret;
            var grandparent = parent.DeclaringType;
            foreach(var m in grandparent.Methods)
            {
                if(m.Name == methodName)
                {
                    ret.Add(new Change<MethodReference>(ChangeType.Modify, m));
                }
            }
            return ret;
        }

        public static string GetOtherMethodNameFrom(string name)
        {
            if (!name.Contains("<") || !name.Contains(">")) return name;
            var start = name.IndexOf('<');
            var end = name.IndexOf('>');
            var newname = name.Substring(start + 1, end - start - 1);
            return newname;
        }
    }
}       