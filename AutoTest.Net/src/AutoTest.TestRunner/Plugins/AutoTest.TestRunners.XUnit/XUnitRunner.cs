using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Options;
using Xunit;
using System.Reflection;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners.XUnit
{
    class XUnitRunner
    {
        public IEnumerable<AutoTest.TestRunners.Shared.Results.TestResult> Run(RunSettings settings, ITestFeedbackProvider channel)
        {
            var logger = new XUnitLogger(channel);
            XunitProject project = new XunitProject();

            var runner = settings.Assembly;
            // Set assembly externally as XUnit screws up the casing
            logger.SetCurrentAssembly(runner.Assembly);

            XunitProjectAssembly assembly = new XunitProjectAssembly
            {
                AssemblyFilename = runner.Assembly,
                ConfigFilename = null,
                ShadowCopy = false
            };
            project.AddAssembly(assembly);

            foreach (XunitProjectAssembly asm in project.Assemblies)
            {
                using (ExecutorWrapper wrapper = new ExecutorWrapper(asm.AssemblyFilename, asm.ConfigFilename, asm.ShadowCopy))
                {
                    try
                    {
                        var xunitRunner = new TestRunner(wrapper, logger);
                        //Run all tests
                        if (runner.Tests.Count() == 0 && runner.Members.Count() == 0 && runner.Namespaces.Count() == 0)
                            xunitRunner.RunAssembly();
                        //Run tests
                        if (runner.Tests.Count() > 0)
                        {
                            foreach (var test in runner.Tests)
                                xunitRunner.RunTest(test.Substring(0, test.LastIndexOf(".")), test.Substring(test.LastIndexOf(".") + 1, test.Length - (test.LastIndexOf(".") + 1)));
                        }
                        //Run members
                        if (runner.Members.Count() > 0)
                        {
                            foreach (var member in runner.Members)
                                xunitRunner.RunClass(member);
                        }
                        //Run namespaces
                        if (runner.Namespaces.Count() > 0)
                        {
                            var loadedAssembly = Assembly.LoadFrom(runner.Assembly);
                            var types = loadedAssembly.GetExportedTypes();
                            loadedAssembly = null;
                            foreach (var ns in runner.Namespaces)
                            {
                                foreach (Type type in types)
                                    if (ns == null || type.Namespace == ns)
                                        xunitRunner.RunClass(type.FullName);
                            }
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return logger.Results;
        }
    }
}
