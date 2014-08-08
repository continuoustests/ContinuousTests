using System.Collections.Generic;
using System.Linq;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;
using Mono.Cecil.Rocks;
namespace AutoTest.Minimizer
{
    public class AssemblyTypeMethodMapBuilder
    {
        public static Dictionary<string, MethodReference> BuildFor(AssemblyDefinition assemblyDefinition)
        {
            var ret = new Dictionary<string, MethodReference>();
            foreach (var m in assemblyDefinition.AllNonIgnoredMethods())
            {
                ret.Add(m.GetChangeCacheName(), m);
            }
            return ret;
        }
    }

}