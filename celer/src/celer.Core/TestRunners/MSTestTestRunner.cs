using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;
using celer.Core.Extensions;
using System.Diagnostics;
using System.IO;

namespace celer.Core.TestRunners
{
    public class MSTestTestRunner : ITestRunner
    {
        private readonly MethodInfo _method;
        private object _fixture;

        public static void RunClassSetup(Type fixture)
        {
            copyFixtureDeploymentItems(fixture);
            foreach (var m in fixture.GetMethods())
            {
                if (m.ContainsAttribute("Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute"))
                    m.Invoke(null, new object[] { null });
            }
        }

        public static void RunClassTeardown(Type fixture)
        {
            foreach (var m in fixture.GetMethods())
            {
                if (m.ContainsAttribute("Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute"))
                    m.Invoke(null, new object[] { });
            }
        }

        public MSTestTestRunner(MethodInfo method)
        {
            _method = method;
        }

        public RunResult Run()
        {
            var timer = new Stopwatch();
            timer.Start();
            var caught = run();
            timer.Stop();
            return new RunResult(_method, isNotInconclusive(caught), IsExpectedException(caught, _method), caught, timer.ElapsedMilliseconds);
        }

        private Exception run()
        {
            Exception caught = null;
            _fixture = Activator.CreateInstance(_method.DeclaringType);
            try
            {
                copyDeploymentItems(_method);
                runTestSetup();
                _method.Invoke(_fixture, BindingFlags.Public | BindingFlags.Instance, null, null, CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    caught = ex.InnerException;
                else
                    caught = ex;
            }
            finally
            {
                runTestTeardown();
            }
            return caught;
        }

        private bool isNotInconclusive(Exception caught)
        {
            if (caught == null)
                return true;
            return !caught.GetType().FullName.Equals("Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException");
        }

        private static void copyFixtureDeploymentItems(Type fixture)
        {
            var assembly = new Uri(fixture.Assembly.CodeBase).LocalPath;
            var outputDir = Path.GetDirectoryName(assembly);
            fixture.GetDeploymentItems().ForEach(x => copyItem(x, outputDir));
        }

        private void copyDeploymentItems(MethodInfo method)
        {
            var assembly = new Uri(_fixture.GetType().Assembly.CodeBase).LocalPath;
            var outputDir = Path.GetDirectoryName(assembly);
            method.GetDeploymentItems().ForEach(x => copyItem(x, outputDir));
        }

        private static void copyItem(DeploymentItem x, string outputDir)
        {
            var source = getDeploymentSource(x, outputDir);
            var destination = getDeploymentDestination(source, x, outputDir);
            if (source.Equals(destination))
                return;
            if (hasChanged(source, destination))
                File.Copy(source, destination, true);
        }

        private static string getDeploymentDestination(string source, DeploymentItem x, string outputDir)
        {
            var destination = outputDir;
            if (x.OutputDirectory != null)
            {
                if (Path.IsPathRooted(x.OutputDirectory))
                    destination = x.OutputDirectory;
                else
                    destination = Path.Combine(outputDir, x.OutputDirectory);
                if (!Directory.Exists(destination))
                    Directory.CreateDirectory(destination);
            }
            return Path.Combine(destination, Path.GetFileName(source));
        }

        private static string getDeploymentSource(DeploymentItem x, string outputDir)
        {
            var source = x.Path;
            if (!Path.IsPathRooted(source))
                source = Path.Combine(outputDir, source);
            return source;
        }

        private static bool hasChanged(string source, string destination)
        {
            if (!File.Exists(destination))
                return true;
            var sourceInfo = new FileInfo(source);
            var destinationInfo = new FileInfo(destination);
            return sourceInfo.Length != destinationInfo.Length || sourceInfo.LastWriteTime > destinationInfo.CreationTime;
        }

        private void runTestSetup()
        {
            foreach (var m in _fixture.GetType().GetMethods())
            {
                if (m.ContainsAttribute("Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute"))
                {
                    m.Invoke(_fixture, BindingFlags.Public | BindingFlags.Instance, null, null,
                             CultureInfo.CurrentCulture);
                }
            }
        }

        private void runTestTeardown()
        {
            foreach (var m in _fixture.GetType().GetMethods())
            {
                if (m.ContainsAttribute("Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute"))
                {
                    m.Invoke(_fixture, BindingFlags.Public | BindingFlags.Instance, null, null,
                             CultureInfo.CurrentCulture);
                }
            }
        }

        private bool IsExpectedException(Exception caught, MethodInfo method)
        {
            var attributes = method.GetCustomAttributes(false);
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i].GetType().FullName == "Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedExceptionAttribute")
                {

                    var attrdata = CustomAttributeData.GetCustomAttributes(method);
                    var expected = (Type)attrdata[i].ConstructorArguments[0].Value;
                    if (caught == null)
                        return false;
                    return expected == caught.GetType();
                }
            }
            return caught == null;
        }
    }
}
