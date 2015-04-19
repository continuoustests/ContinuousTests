using System;
using System.Collections.Generic;
using AutoTest.TestRunners.Shared.Targeting;

namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    public static class Reflect
    {
        private static Func<string, IReflectionProvider> _reflectionProviderFactory = (assembly) => { return new SystemReflectionProvider(assembly); };

        public static void ScratchThat_InsteadUseThisAwesome(Func<string, IReflectionProvider> reflectionProviderFactory)
        {
            _reflectionProviderFactory = reflectionProviderFactory;
        }

        public static IReflectionProvider On(string assembly)
        {
            return _reflectionProviderFactory(assembly);
        }

        public static Version GetTargetFramework(string assembly)
        {
            using (var provider = _reflectionProviderFactory(assembly))
            {
                return provider.GetTargetFramework();
            }
        }

        public static Platform GetPlatform(string assembly)
        {
            using (var provider = _reflectionProviderFactory(assembly))
            {
                return provider.GetPlatform();
            }
        }

        public static IEnumerable<TypeName> GetReferences(string assembly)
        {
            using (var provider = _reflectionProviderFactory(assembly))
            {
                return provider.GetReferences();
            }
        }
    }
}
