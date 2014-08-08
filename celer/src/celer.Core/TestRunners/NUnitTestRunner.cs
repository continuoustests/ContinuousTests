using System;
using System.Globalization;
using System.Reflection;
using celer.Core.Extensions;
using System.Diagnostics;

namespace celer.Core.TestRunners
{
    public class NUnitTestRunner : ITestRunner
    {
        private readonly MethodInfo _testMethod;

        public NUnitTestRunner(MethodInfo testMethod)
        {
            _testMethod = testMethod;
        }

        public RunResult Run()
        {
            var timer = new Stopwatch();
            timer.Start();

            Exception caught = null;
            var fixture = BuildFixtureInstance(_testMethod.DeclaringType);
            try
            {
                RunSetups(fixture);
                _testMethod.Invoke(fixture, BindingFlags.Public | BindingFlags.Instance, null, null,
                                   CultureInfo.CurrentCulture);
                RunTeardowns(fixture);
            }
            catch (Exception ex)
            {
                caught = ex.InnerException;
            }
            timer.Stop();
            return new RunResult(_testMethod, true, IsExpectedException(caught), caught, timer.ElapsedMilliseconds);
        }

        private void RunSetups(object fixture)
        {
            foreach (var m in _testMethod.DeclaringType.GetMethods())
            {
                if (m.ContainsAttribute("NUnit.Framework.SetUpAttribute"))
                {
                    m.Invoke(fixture, BindingFlags.Public | BindingFlags.Instance, null, null,
                             CultureInfo.CurrentCulture);
                }
            }
        }

        private void RunTeardowns(object fixture)
        {
            foreach (var m in _testMethod.DeclaringType.GetMethods())
            {
                if (m.ContainsAttribute("NUnit.Framework.TearDownAttribute"))
                {
                    m.Invoke(fixture, BindingFlags.Public | BindingFlags.Instance, null, null,
                             CultureInfo.CurrentCulture);
                }
            }
        }

        private bool IsExpectedException(Exception caught)
        {
            var attributes = _testMethod.GetCustomAttributes(false);
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i].GetType().FullName == "NUnit.Framework.ExpectedExceptionAttribute")
                {

                    var attrdata = CustomAttributeData.GetCustomAttributes(_testMethod);
                    var expected = (Type)attrdata[i].ConstructorArguments[0].Value;
                    return expected == caught.GetType();
                }
            }
            return caught == null;
        }

        private static object BuildFixtureInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}