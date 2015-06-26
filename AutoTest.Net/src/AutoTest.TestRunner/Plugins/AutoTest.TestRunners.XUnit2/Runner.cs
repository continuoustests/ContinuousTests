using System;
using System.Collections.Generic;
using System.Linq;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using AutoTest.TestRunners.Shared.Communication;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;

namespace AutoTest.TestRunners.XUnit2
{
    public class Runner : IAutoTestNetTestRunner
    {
        private Func<string, IReflectionProvider> _reflectionProviderFactory = (assembly) => { return Reflect.On(assembly); };
        private ITestFeedbackProvider _channel = null;

        public string Identifier { get { return "XUnit"; } }

        public void SetLogger(ILogger logger)
        {
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
                var method = locator.LocateMethod(member);
                if (method == null)
                    return false;
                return method.Attributes.Contains("Xunit.FactAttribute") &&
                    !method.IsAbstract;
            }
        }

        public bool ContainsTestsFor(string assembly, string member)
        {
            using (var locator = _reflectionProviderFactory(assembly))
            {
                var cls = locator.LocateClass(member);
                if (cls == null)
                    return false;
                return !cls.IsAbstract && cls.Methods.Where(x => x.Attributes.Contains("Xunit.FactAttribute")).Count() > 0;
            }
        }

        public bool ContainsTestsFor(string assembly)
        {
            using (var parser = _reflectionProviderFactory(assembly))
            {
                return parser.GetReferences().Count(x => x.Name.Equals("xunit.core")) > 0;
            }
        }

        public bool Handles(string identifier)
        {
            return identifier.ToLower().Equals(Identifier.ToLower());
        }

        public IEnumerable<TestResult> Run(RunSettings settings)
        {
            var runner = new XUnit2Runner();
            return runner.Run(settings, _channel);
        }
    }
}
