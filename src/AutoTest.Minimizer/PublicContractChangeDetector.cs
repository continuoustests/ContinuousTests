using System;
using System.Collections.Generic;
using Mono.Cecil;
using AutoTest.Minimizer.Extensions;

namespace AutoTest.Minimizer
{
    public class PublicContractChangeDetector
    {
        public IEnumerable<Change<string>> GetAllPublicContractChangesBetween(string oldAssembly, string newAssembly)
        {
            var olda = AssemblyDefinition.ReadAssembly(oldAssembly);
            var newa = AssemblyDefinition.ReadAssembly(newAssembly);
            var changes = GetAllPublicContractChangesBetween(olda, newa);
            olda.Dispose();
            newa.Dispose();
            return changes;
        }

        public IEnumerable<Change<string>> GetAllPublicContractChangesBetween(AssemblyDefinition oldAssembly, AssemblyDefinition newAssembly)
        {
            var ret = new List<Change<string>>();
            try
            {
                if (newAssembly.ContainsAttribute("InternalsVisibleToAttribute"))
                {
                    ret.Add(new Change<string>(ChangeType.Modify, "Internals Are Visible, rebuild!"));
                };
                var oldCache = AssemblyPubliclyVisibleStuffMapBuilder.BuildPublicOnlyFor(oldAssembly);
                var newCache = AssemblyPubliclyVisibleStuffMapBuilder.BuildPublicOnlyFor(newAssembly);
                var differences = MapKeyDifferenceFinder.GetChangesBetween(oldCache, newCache);
                foreach (var c in differences)
                {
                    ret.Add(new Change<string>(c.ChangeType, c.ItemChanged.FullName));
                }
            }
            catch (Exception ex)
            {
                ret.Add(new Change<string>(ChangeType.Modify, "An exception occured finding changes" + ex));
            }
            return ret;
        }
    }

    public class AssemblyPubliclyVisibleStuffMapBuilder
    {
        public static Dictionary<string, MemberReference> BuildPublicOnlyFor(AssemblyDefinition assemblyDefinition)
        {
            var ret = new Dictionary<string, MemberReference>();
            foreach (var m in assemblyDefinition.AllPubliclyVisisble())
            {
                if (!ret.ContainsKey(m.FullName))
                {
                    ret.Add(m.FullName, m);
                }
            }
            return ret;
        }
    }
}
