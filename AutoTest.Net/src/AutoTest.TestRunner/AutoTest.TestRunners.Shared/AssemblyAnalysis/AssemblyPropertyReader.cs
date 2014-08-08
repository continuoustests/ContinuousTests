using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    public interface IAssemblyPropertyReader
    {
        string GetName(string assembly);
        Version GetTargetFramework(string assembly);
        Targeting.Platform GetPlatform(string assembly);
        IEnumerable<TypeName> GetReferences(string assembly);
    }

    public class AssemblyPropertyReader : IAssemblyPropertyReader
    {
        public string GetName(string assembly)
        {
            using (var provider = Reflect.On(assembly))
            {
                return provider.GetName();
            }
        }

        public Version GetTargetFramework(string assembly)
        {
            using (var provider = Reflect.On(assembly))
            {
                return provider.GetTargetFramework();
            }
        }

        public Targeting.Platform GetPlatform(string assembly)
        {
            using (var provider = Reflect.On(assembly))
            {
                return provider.GetPlatform();
            }
        }

        public IEnumerable<TypeName> GetReferences(string assembly)
        {
            using (var provider = Reflect.On(assembly))
            {
                return provider.GetReferences();
            }
        }
    }
}
