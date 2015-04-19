using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using AutoTest.TestRunners.Shared.Communication;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;
using Simple.Testing.Framework;

namespace AutoTest.TestRunners.SimpleTesting
{
    public class Runner : IAutoTestNetTestRunner
    {
        private ILogger _logger;
        private Func<string, IReflectionProvider> _reflectionProviderFactory = assembly => Reflect.On(assembly);
        private ITestFeedbackProvider _testFeedbackProviderChannel;
        private List<TestResult> _results;

        public string Identifier
        {
            get { return "SimpleTesting"; }
        }

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
            _testFeedbackProviderChannel = channel;
        }

        public bool IsTest(string assembly, string member)
        {
            using (var locator = _reflectionProviderFactory(assembly))
            {
                var spec = locator.LocateMethod(member);
                if (spec == null)
                    return false;
                var fixture = locator.LocateClass(locator.GetParentType(member));
                if (fixture == null)
                    return false;
                return isTest(fixture, spec);
            }
        }

        public bool ContainsTestsFor(string assembly, string member)
        {
            using (var locator = _reflectionProviderFactory(assembly))
            {
                var fixture = locator.LocateClass(member);
                if (fixture == null)
                    return false;
                return fixture.Methods.Any(x => isTest(fixture, x));
            }
        }

        public bool ContainsTestsFor(string assembly)
        {
            using (var parser = _reflectionProviderFactory(assembly))
            {
                return parser.GetReferences().Count(x => x.FullName.StartsWith("Simple.Testing")) > 0;
            }
        }

        private bool isTest(SimpleClass fixture, SimpleMethod method)
        {
            var specFound = method.ReturnType.StartsWith("Simple.Testing.ClientFramework.Specification");
            var enumerableFound = method.ReturnType.StartsWith("IEnumerable<Simple.Testing.ClientFramework.Specification>");
            return !fixture.IsAbstract && (specFound || enumerableFound);
        }

        public bool Handles(string identifier)
        {
            return identifier.ToLower().Equals(Identifier.ToLower());
        }


        public IEnumerable<TestResult> Run(RunSettings settings)
        {
            _results = new List<TestResult>();
            var assembly = getAssembly(settings.Assembly.Assembly);
            if (_results.Count != 0)
            {

            }
            else
            {
                foreach (var result in runTests(settings, assembly))
                {
                    _results.Add(result);
                }
            }
            return _results;
        }

        private IEnumerable<TestResult> runTests(RunSettings settings, Assembly assembly)
        {
            if (shouldRunAllTests(settings))
            {
                return runAllTests(assembly);
     
            }
            var results = new List<TestResult>();
            foreach (var member in settings.Assembly.Members)
                results.AddRange(runAllOnType(assembly.GetType(member)));
            results.AddRange(runAllNamed(assembly, settings.Assembly.Tests));
            //foreach (var ns in settings.Assembly.Namespaces)
            //    results.AddRange(runAllInNameSpace(assembly));
            return results;
        }

        private IEnumerable<TestResult> runAllNamed(Assembly assembly, IEnumerable<string> names)
        {
            TestResult failresult = null;
            IEnumerable<RunResult> results = null;
            try
            {
                results = SimpleRunner.RunByName(assembly, names);
            }
            catch (Exception ex)
            {
                failresult = new TestResult(Identifier, assembly.FullName, "", 0, "Error while running tests",
                                            TestState.Panic, getMessage(ex) + Environment.NewLine + Environment.NewLine);
                failresult.AddStackLines(getStackLines(ex));
            }
            if (failresult != null)
            {
                yield return failresult;
            }
            else
            {
                if (results == null) yield break;
                foreach (var res in results)
                {
                    yield return BuildTestResult(res);
                }
            }
        }

        private IEnumerable<TestResult> runAllInNameSpace(Assembly assembly)
        {
            TestResult failresult = null;
            IEnumerable<RunResult> results = null;
            try
            {
                results = SimpleRunner.RunAllInAssembly(assembly);
            }
            catch (Exception ex)
            {
                failresult = new TestResult(Identifier, assembly.FullName, "", 0, "Error while running tests",
                                            TestState.Panic, getMessage(ex) + Environment.NewLine + Environment.NewLine);
                failresult.AddStackLines(getStackLines(ex));
            }
            if (failresult != null)
            {
                yield return failresult;
            }
            else
            {
                if (results == null) yield break;
                foreach (var res in results)
                {
                    yield return BuildTestResult(res);
                }
            }
        }

        private IEnumerable<TestResult> runAllOnType(Type type)
        {
            TestResult failresult = null;
            IEnumerable<RunResult> results = null;
            try
            {
                results = SimpleRunner.RunAllInType(type);
            }
            catch (Exception ex)
            {
                failresult = new TestResult(Identifier, type.Assembly.FullName, "", 0, "Error while running tests",
                                            TestState.Panic, getMessage(ex) + Environment.NewLine + Environment.NewLine);
                failresult.AddStackLines(getStackLines(ex));
            }
            if (failresult != null)
            {
                yield return failresult;
            }
            else
            {
                if (results == null) yield break;
                foreach (var res in results)
                {
                    yield return BuildTestResult(res);
                }
            }
        }

        private IEnumerable<TestResult> runAllTests(Assembly assembly)
        {
            TestResult failresult = null;
            IEnumerable<RunResult> results = null;
            try
            {
                results = SimpleRunner.RunAllInAssembly(assembly);
            }
            catch (Exception ex)
            {
                failresult = new TestResult(Identifier, assembly.FullName, "", 0, "Error while running tests",
                                            TestState.Panic, getMessage(ex) + Environment.NewLine + Environment.NewLine);
                failresult.AddStackLines(getStackLines(ex));
            }
            if (failresult != null)
            {
                yield return failresult;
            }
            else
            {
                if (results == null) yield break; 
                foreach (var res in results)
                {
                    yield return BuildTestResult(res);
                }
            }
        }

        private TestResult BuildTestResult(RunResult res)
        {
            var message = BuildMessage(res);
            var state = res.Passed ? TestState.Passed : TestState.Failed;
            var result = new TestResult("SimpleTesting", 
                                  res.FoundOnMemberInfo.DeclaringType.Assembly.FullName,
                                  res.FoundOnMemberInfo.DeclaringType.Name,
                                  0,
                                  res.FoundOnMemberInfo.DeclaringType.FullName + "." + res.FoundOnMemberInfo.Name,
                                  res.Name,
                                  state,
                                  message
                       );
            if(state == TestState.Failed)
            {
                result.AddStackLines(getStackLines(res.Thrown));
            }
            return result;
        }

        private string BuildMessage(RunResult res)
        {
            var ret = res.Message ?? "";
            foreach(var exp in res.Expectations)
            {
                if (!exp.Passed)
                    ret += exp.Text;
            }
            return ret;
        }

        private bool shouldRunAllTests(RunSettings settings)
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
