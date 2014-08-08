using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Communication;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners.Shared
{
    public interface IAutoTestNetTestRunner
    {
        string Identifier { get; }

        void SetLogger(ILogger logger);
        // Reflection provider factory takes an assembly path as parameter and returns a reflection provider
        void SetReflectionProvider(Func<string, IReflectionProvider> reflectionProviderFactory);
        void SetLiveFeedbackChannel(ITestFeedbackProvider channel);

        bool IsTest(string assembly, string member);
        bool ContainsTestsFor(string assembly, string member);
        bool ContainsTestsFor(string assembly);

        bool Handles(string identifier);
        IEnumerable<TestResult> Run(RunSettings settings);
    }
}
