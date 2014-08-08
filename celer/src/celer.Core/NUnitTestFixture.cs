using System;
using System.Collections.Generic;
using System.Linq;
using celer.Core.Extensions;
using celer.Core.TestRunners;
using System.Reflection;

namespace celer.Core
{
    class NUnitTestFixture : ITestFixture
    {
        private readonly Type _type;

        public NUnitTestFixture(Type type)
        {
            _type = type;
        }

        public IEnumerable<RunResult> Run()
        {
            return Run(GetMethodsInFixture());
        }

        public IEnumerable<RunResult> Run(IEnumerable<MethodInfo> methods)
        {
            return GetTestRunnersInFixture(methods).Select(runner => runner.Run()).ToList();
        }

        private IEnumerable<ITestRunner> GetTestRunnersInFixture(IEnumerable<MethodInfo> methods)
        {
            return (from m in methods select new NUnitTestRunner(m)).Cast<ITestRunner>();
        }

        private IEnumerable<MethodInfo> GetMethodsInFixture()
        {
            return (from m in _type.GetMethods() where m.ContainsAttribute("NUnit.Framework.TestAttribute") select m);
        }
    }
}
