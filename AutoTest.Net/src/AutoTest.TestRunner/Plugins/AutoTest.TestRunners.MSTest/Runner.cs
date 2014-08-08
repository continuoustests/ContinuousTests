using System.Linq;
using System.Collections.Generic;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.MSTest.Extensions;
using AutoTest.TestRunners.Shared.Communication;
using System;

namespace AutoTest.TestRunners.MSTest
{
    public class Runner : IAutoTestNetTestRunner
    {
        private ILogger _logger;
        private Func<string, IReflectionProvider> _reflectionProviderFactory = (assembly) => { return Reflect.On(assembly); };
        private ITestFeedbackProvider _channel = new NullTestFeedbackProvider();

        public string Identifier { get { return "MSTest"; } }

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
            _channel = channel;
        }

        public bool IsTest(string assembly, string member)
        {
            using (var locator = _reflectionProviderFactory(assembly))
            {
                var parentType = locator.GetParentType(member);
                if (parentType == null)
                    return false;
                var parent = locator.LocateClass(parentType);
                if (parent == null)
                    return false;
                var method = locator.LocateMethod(member);
                if (method == null)
                    return false;
                return method.Attributes.Contains("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute") &&
                    !method.IsAbstract &&
                    !parent.IsAbstract;
            }
        }

        public bool ContainsTestsFor(string assembly, string member)
        {
            using (var locator = _reflectionProviderFactory(assembly))
            {
                var cls = locator.LocateClass(member);
                if (cls == null)
                    return false;
                return !cls.IsAbstract && cls.Attributes.Contains("Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute");
            }
        }

        public bool ContainsTestsFor(string assembly)
        {
            using (var parser = _reflectionProviderFactory(assembly))
            {
                return parser.GetReferences().Count(x =>
                {
                    return
                        x.Name.Equals("Microsoft.VisualStudio.QualityTools.UnitTestFramework") ||
                        x.Name.Equals("Worst.Testing.Framework.Ever");
                }) > 0;
            }
        }

        public bool Handles(string identifier)
        {
            return Identifier.ToLower().Equals(identifier.ToLower());
        }

        public IEnumerable<TestResult> Run(RunSettings settings)
        {
            return new CelerRunner(_logger, _reflectionProviderFactory, _channel).Run(settings);
        }
    }
}
