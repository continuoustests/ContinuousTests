using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Simple.Testing.Framework
{
    public static class SimpleRunner
    {
        public static IEnumerable<RunResult> RunAllInAssembly(string assemblyName)
        {
            var assembly = Assembly.LoadFrom(assemblyName);
            return RunAllInAssembly(assembly);
        }

        public static IEnumerable<RunResult> RunAllInAssembly(Assembly assembly)
        {
            var generator = new RootGenerator(assembly);;
            var runner = new SpecificationRunner();
            return generator.GetSpecifications().Select(runner.RunSpecifciation);
        }
    }
}
