using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Communication;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners.Tests.Plugins
{
    public class Plugin1 : AutoTest.TestRunners.Shared.IAutoTestNetTestRunner
    {
        public string Identifier { get { return ""; } }

        public void SetLogger(ILogger logger)
        {
        }

        public void SetReflectionProvider(Func<string, IReflectionProvider> reflectionProviderFactory)
        {
        }

        public void SetLiveFeedbackChannel(ITestFeedbackProvider channel)
        {
        }

        public bool IsTest(string assembly, string type)
        {
            return true;
        }

        public bool ContainsTestsFor(string assembly, string type)
        {
            return true;
        }

        public bool ContainsTestsFor(string assembly)
        {
            return true;
        }

        public bool Handles(string identifier)
        {
            return true;
        }

        public IEnumerable<Shared.Results.TestResult> Run(RunSettings settings)
        {
            return null;
        }
    }
}
