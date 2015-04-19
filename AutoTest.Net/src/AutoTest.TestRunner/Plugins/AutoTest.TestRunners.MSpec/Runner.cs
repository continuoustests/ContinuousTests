using System;
using System.Collections.Generic;
using System.Linq;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Communication;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using AutoTest.TestRunners.Shared.Results;
using System.Reflection;
using System.IO;

namespace AutoTest.TestRunners.MSpec
{
    public class Runner : IAutoTestNetTestRunner
    {
        //TODO GFY not used
        private ILogger _logger;
        private Func<string, IReflectionProvider> _reflectionProviderFactory = (assembly) => { return Reflect.On(assembly); };
        private ITestFeedbackProvider _feedback;
        private List<TestResult> _results;

        public string Identifier { get { return "MSpec"; } }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void SetReflectionProvider(Func<string, IReflectionProvider> reflectionProviderFactory)
        {
            _reflectionProviderFactory = reflectionProviderFactory;
        }

        public void SetLiveFeedbackChannel(ITestFeedbackProvider channel)
        {
            _feedback = channel;
        }

        public bool IsTest(string assembly, string member)
        {
            using (var locator = _reflectionProviderFactory(assembly))
            {
                var fixture = locator.LocateClass(member);
                if (fixture == null)
                    return false;
                return !fixture.IsAbstract &&
                    fixture.Fields.Count(x =>
                    x.FieldType == "Machine.Specifications.Establish" ||
                    x.FieldType == "Machine.Specifications.It" ||
                    x.FieldType == "Machine.Specifications.Because") > 0;
            }
        }

        public bool ContainsTestsFor(string assembly, string member)
        {
            return IsTest(assembly, member);
        }

        public bool ContainsTestsFor(string assembly)
        {
            using (var parser = _reflectionProviderFactory(assembly))
            {
                return parser.GetReferences().Count(x => x.FullName.StartsWith("Machine.Specifications")) > 0;
            }
        }

        public bool Handles(string identifier)
        {
            return identifier.ToLower().Equals(Identifier.ToLower());
        }

        public IEnumerable<TestResult> Run(RunSettings settings)
        {
            var assembly = getAssembly(settings.Assembly.Assembly);

            var generator = new LinFu.DynamicProxy.ProxyFactory();
            var proxy = new TestListenerProxy(_feedback, settings.Assembly.Assembly);

            _results = new List<TestResult>();
            var mspec = Assembly.LoadFrom("Machine.Specifications.dll");
            var type = mspec.GetType("Machine.Specifications.Runner.Impl.AppDomainRunner");
            var iTestListener = mspec.GetType("Machine.Specifications.Runner.ISpecificationRunListener");
            var dflt = mspec.GetType("Machine.Specifications.Runner.RunOptions").GetProperty("Default").GetValue(null, null);
            var listener = generator.CreateProxy(iTestListener, proxy, new Type[] {});

            var runner = Activator.CreateInstance(type, new object[] {listener, dflt});
            runTests(settings, assembly, runner);
            _results.AddRange(proxy.Results);
            return _results;
        }

        private void runTests(RunSettings settings, Assembly assembly, object runner)
        {
            if (runAllTests(settings))
            {
                runner.Run("RunAssembly", new object[] {assembly});
                return;
            }
            foreach (var member in settings.Assembly.Tests)
                runner.Run("RunMember", new object[] {assembly, assembly.GetType(member)});
            foreach (var member in settings.Assembly.Members)
                runner.Run("RunMember", new object[] { assembly, assembly.GetType(member)});
            foreach (var ns in settings.Assembly.Namespaces)
                runner.Run("RunNamespace", new object[] {assembly, ns});
        }

        private bool runAllTests(RunSettings settings)
        {
            return
                !settings.Assembly.Tests.Any() &&
                !settings.Assembly.Members.Any() &&
                !settings.Assembly.Namespaces.Any();
        }

        private Assembly getAssembly(string assembly)
        {
            try
            {
                return Assembly.Load(getAssemblySignature(assembly));
            }
            catch (FileLoadException ex)
            {
                _results.Add(new TestResult(Identifier, assembly, "", 0, "Error while preparing runner", TestState.Panic, getMessage(ex) + Environment.NewLine + ex.FileName + Environment.NewLine + ex.FusionLog));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
                return null;
            }
            catch (FileNotFoundException ex)
            {
                _results.Add(new TestResult(Identifier, assembly, "", 0, "Error while preparing runner", TestState.Panic, getMessage(ex) + Environment.NewLine + ex.FileName + Environment.NewLine + ex.FusionLog));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
                return null;
            }
            catch (BadImageFormatException ex)
            {
                _results.Add(new TestResult(Identifier, assembly, "", 0, "Error while preparing runner", TestState.Panic, getMessage(ex) + Environment.NewLine + ex.FileName + Environment.NewLine + ex.FusionLog));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
                return null;
            }
            catch (Exception ex)
            {
                _results.Add(new TestResult(Identifier, assembly, "", 0, "Error while preparing runner", TestState.Panic, getMessage(ex)));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
                return null;
            }
        }

        private string getAssemblySignature(string assembly)
        {
            using (var provider = _reflectionProviderFactory(assembly))
            {
                return provider.GetName();
            }
        }

        private string getMessage(Exception ex)
        {
            var message = ex.Message;
            if (ex.InnerException != null)
                message += Environment.NewLine + getMessage(ex.InnerException);
            return message;
        }

        private StackLine[] getStackLines(Exception ex)
        {
            if (ex == null)
                return new StackLine[] { };
            var stackLines = new List<StackLine>();
            if (ex.InnerException != null)
                stackLines.AddRange(getStackLines(ex.InnerException));
            stackLines.AddRange(ex.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(x => new StackLine(x)));
            return stackLines.ToArray();
        }
    }
}
