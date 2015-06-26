using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoTest.TestRunners.Shared.Communication;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;
using Xunit;
using Xunit.Abstractions;

namespace AutoTest.TestRunners.XUnit2
{
    class XUnit2Runner
    {
        public IEnumerable<TestResult> Run(RunSettings settings, ITestFeedbackProvider channel)
        {
            XunitProject project = new XunitProject();

            var runner = settings.Assembly;

            XunitProjectAssembly assembly = new XunitProjectAssembly
            {
                AssemblyFilename = runner.Assembly,
                ConfigFilename = null,
                ShadowCopy = false
            };
            project.Add(assembly);

            var results = new List<TestResult>();

            foreach (XunitProjectAssembly asm in project.Assemblies)
            {
                using (var controller = new Xunit2(new NullSourceInformationProvider(), asm.AssemblyFilename, asm.ConfigFilename, asm.ShadowCopy, diagnosticMessageSink: new NullMessageSink()))
                using (var discoveryVisitor = new TestDiscoveryVisitor())
                {
                    try
                    {
                        controller.Find(true, discoveryVisitor, TestFrameworkOptions.ForDiscovery());

                        discoveryVisitor.Finished.WaitOne();

                        var filters = CreateFilter(runner);

                        var filteredTestCases = discoveryVisitor.TestCases.Where(filters.Filter).ToList();

                        var testMessageVisitor = new AutoTestTestMessageVisitor(channel);

                        controller.RunTests(filteredTestCases, testMessageVisitor, TestFrameworkOptions.ForExecution());

                        testMessageVisitor.Finished.WaitOne();

                        results.AddRange(testMessageVisitor.Results);
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return results;
        }

        private static XunitFilters CreateFilter(AssemblyOptions runner)
        {
            //Run all tests
            if (!runner.Tests.Any() && !runner.Members.Any() && !runner.Namespaces.Any())
            {
                return new XunitFilters();
            }
            //Run tests
            if (runner.Tests.Any())
            {
                var filters = new XunitFilters();

                foreach (var test in runner.Tests)
                    filters.IncludedMethods.Add(test);

                return filters;
            }
            //Run classes
            if (runner.Members.Any())
            {
                var filters = new XunitFilters();

                foreach (var test in runner.Members)
                    filters.IncludedClasses.Add(test);

                return filters;
            }
            //Run namespaces
            if (runner.Namespaces.Any())
            {
                var filters = new XunitFilters();
                var loadedAssembly = Assembly.LoadFrom(runner.Assembly);
                var types = loadedAssembly.GetExportedTypes();
                loadedAssembly = null;
                foreach (var ns in runner.Namespaces)
                {
                    foreach (Type type in types)
                        if (ns == null || type.Namespace == ns)
                            filters.IncludedClasses.Add(type.FullName);
                }

                return filters;
            }

            throw new ArgumentException("Could not figure out what to do");
        }

        class TestDiscoveryVisitor : TestMessageVisitor<IDiscoveryCompleteMessage>
        {
            public TestDiscoveryVisitor()
            {
                TestCases = new List<ITestCase>();
            }

            public List<ITestCase> TestCases { get; private set; }

            protected override bool Visit(ITestCaseDiscoveryMessage discovery)
            {
                TestCases.Add(discovery.TestCase);

                return true;
            }
        }

        class AutoTestTestMessageVisitor : TestMessageVisitor<ITestAssemblyFinished>
        {
            private readonly ITestFeedbackProvider _channel;

            private readonly IList<TestResult> _results;

            public AutoTestTestMessageVisitor(ITestFeedbackProvider channel)
            {
                _channel = channel;
                _results = new List<TestResult>();
            }

            public IEnumerable<TestResult> Results
            {
                get { return _results.AsEnumerable(); }
            }

            protected override bool Visit(IBeforeTestStarting beforeTestStarting)
            {
                var result = base.Visit(beforeTestStarting);

                _channel.TestStarted(GetTestName(beforeTestStarting.TestMethod));

                return result;
            }

            protected override bool Visit(ITestPassed testPassed)
            {
                var result = base.Visit(testPassed);

                var testResult = getResult(testPassed.TestAssembly.Assembly.Name, testPassed.ExecutionTime, TestState.Passed,
                    testPassed.TestMethod, testPassed.Output);

                _channel.TestFinished(testResult);
                _results.Add(testResult);

                return result;
            }

            protected override bool Visit(ITestFailed testFailed)
            {
                var result = base.Visit(testFailed);

                var stackTrace = testFailed.StackTraces.SelectMany(s => s.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)).ToArray();

                var testResult = getResult(testFailed.TestAssembly.Assembly.Name, testFailed.ExecutionTime, TestState.Failed,
                    testFailed.TestMethod, testFailed.Output, stackTrace);
                _results.Add(testResult);

                _channel.TestFinished(testResult);

                return result;
            }

            protected override bool Visit(ITestSkipped testSkipped)
            {
                var result = base.Visit(testSkipped);

                var testResult = getResult(testSkipped.TestAssembly.Assembly.Name, testSkipped.ExecutionTime,
                    TestState.Ignored,
                    testSkipped.TestMethod, testSkipped.Output);

                _channel.TestFinished(testResult);
                _results.Add(testResult);

                return result;
            }

            private TestResult getResult(string currentAssembly, decimal durationInSeconds, TestState state, ITestMethod testMethod, string message, params string[] stackLines)
            {
                var name = GetTestName(testMethod);
                var testName = name.IndexOf("(") == -1 ? name : name.Substring(0, name.IndexOf("("));
                var result = new TestResult("XUnit2", currentAssembly, "", ((double)durationInSeconds * 1000), testName, name, state, message);
                if (stackLines != null && stackLines.Any())
                {
                    foreach (var line in stackLines)
                        result.AddStackLine(new StackLine(line));
                }
                return result;
            }

            private static string GetTestName(ITestMethod testMethod)
            {
                return testMethod.TestClass.Class.Name + "." + testMethod.Method.Name;
            }
        }
    }
}
