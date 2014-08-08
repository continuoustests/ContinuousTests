using System;
using System.Collections.Generic;
using System.Linq;
using celer.Core.TestRunners;
using System.Reflection;
using celer.Core.Extensions;

namespace celer.Core
{
    public class MSTestTestFixture : ITestFixture
    {
        private readonly Type _type;

        public MSTestTestFixture(Type type)
        {
            _type = type;
        }

        public IEnumerable<RunResult> Run()
        {
            return Run(GetMethodsInFixture());
        }

        public IEnumerable<RunResult> Run(IEnumerable<MethodInfo> methods)
        {
            var results = new List<RunResult>();
            try
            {
                classSetup();
                results.AddRange(GetTestRunnersInFixture(methods).Select(runner => runner.Run()).ToList());
            }
            catch (Exception ex)
            {
                results.AddRange(GetMethodsInFixture().Select(m => new RunResult(m, true, false, ex, 0)));
            }
            finally
            {
                results.AddRange(classTeardown());
            }
            return results;
        }

        private IEnumerable<RunResult> classTeardown()
        {
            try
            {
                MSTestTestRunner.RunClassTeardown(_type);
                return new RunResult[] { };
            }
            catch (Exception ex)
            {
                return GetMethodsInFixture().Select(m => new RunResult(m, true, false, ex, 0));
            }

        }

        private void classSetup()
        {
            MSTestTestRunner.RunClassSetup(_type);
        }

        private IEnumerable<ITestRunner> GetTestRunnersInFixture(IEnumerable<MethodInfo> methods)
        {
            return (from m in methods select new MSTestTestRunner(m)).Cast<ITestRunner>();
        }

        private IEnumerable<MethodInfo> GetMethodsInFixture()
        {
            return (from m in _type.GetMethods() where m.ContainsAttribute("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute") select m);
        }
    }
}
