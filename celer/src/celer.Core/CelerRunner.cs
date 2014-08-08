using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using celer.Core.Extensions;

namespace celer.Core
{
    public class CelerRunner
    {
        public void RunTests(IEnumerable<string> assemblies)
        {
            var testFixtures = LoadAssemblies(assemblies).Select(x => x.GetTestFixtures()).Combined().ToList();
            Console.WriteLine("got " + testFixtures.Count + " tests, running");
            testFixtures.ForkAndJoinTo(4, x => x.Run()).Combined().ToList().ForEach(x => PrintResult(x));
            Console.WriteLine("done.");
        }

        private static object PrintResult(RunResult result)
        {
            if(!result.Passed)
            {
                Console.WriteLine();
                Console.WriteLine(result.Test.Name + " failed.");
            } else
            {
                Console.Write(".");
            }
            return null;
        }

        private static IEnumerable<Assembly> LoadAssemblies(IEnumerable<string> assemblies)
        {
            return assemblies.Select(Assembly.LoadFrom);
        }
    }
}
