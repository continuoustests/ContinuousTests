using System;
using System.Collections.Generic;
using NUnit.Core;
using NUnit.Util;
using NUnit.Core.Filters;
using AutoTest.TestRunners.Shared.Errors;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners.NUnit
{
    class NUnitRunner
    {
        private readonly ITestFeedbackProvider _channel;

        public NUnitRunner(ITestFeedbackProvider channel)
        {
            _channel = channel;
        }

        public void Initialize()
        {
            var settingsService = new SettingsService();
            ServiceManager.Services.AddService(settingsService);
            ServiceManager.Services.AddService(new DomainManager());
            ServiceManager.Services.AddService(new ProjectService());
            ServiceManager.Services.AddService(new AddinRegistry());
            ServiceManager.Services.AddService(new AddinManager());
            ServiceManager.Services.AddService(new TestAgency());
            
            ServiceManager.Services.InitializeServices();
        }

		public IEnumerable<Shared.Results.TestResult> Execute(Options options)
		{
            var package = createPackage(options);
            using (var testRunner = new DefaultTestRunnerFactory().MakeTestRunner(package))
            {
                return runTests(options, package, testRunner);
            }
		}

        private IEnumerable<Shared.Results.TestResult> runTests(Options options, TestPackage package, TestRunner testRunner)
        {
            testRunner.Load(package);
            if (testRunner.Test == null)
            {
                testRunner.Unload();
                return new[] { ErrorHandler.GetError("NUnit", "Unable to locate fixture") };
            }

            var harvester = new TestHarvester(_channel);
            var testFilter = getTestFilter(options);
            string savedDirectory = Environment.CurrentDirectory;
            var result = run(testRunner, harvester, testFilter, savedDirectory);

            if (result != null)
                return harvester.Results;
            return harvester.Results;
        }

        private TestResult run(TestRunner testRunner, TestHarvester harvester, TestFilter testFilter, string savedDirectory)
        {
            TestResult result = null;
            try
            {
                //result = testRunner.Run(harvester, testFilter, false, LoggingThreshold.Off);
                result = testRunner.Run(harvester, testFilter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Environment.CurrentDirectory = savedDirectory;
            }
            return result;
        }

        private TestFilter getTestFilter(Options options)
        {
            var testFilter = TestFilter.Empty;
            if (!string.IsNullOrEmpty(options.Tests))
                testFilter = new SimpleNameFilter(options.Tests);

            if (!string.IsNullOrEmpty(options.Categories))
            {
                TestFilter excludeFilter = new NotFilter(new CategoryExpression(options.Categories).Filter);
                if (testFilter.IsEmpty)
                    testFilter = excludeFilter;
                else if (testFilter is AndFilter)
                    ((AndFilter)testFilter).Add(excludeFilter);
                else
                    testFilter = new AndFilter(testFilter, excludeFilter);
            }

            var notFilter = testFilter as NotFilter;
            if (notFilter != null)
                notFilter.TopLevel = true;
            return testFilter;
        }
        
        private TestPackage createPackage(Options options)
        {
            const ProcessModel processModel = ProcessModel.Default;
            
            var package = new TestPackage(options.Assembly);
            var domainUsage = DomainUsage.Single;
			package.TestName = null;
            
            package.Settings["ProcessModel"] = processModel;
            package.Settings["DomainUsage"] = domainUsage;
            //if (framework != null)
            //package.Settings["RuntimeFramework"] = Environment.Version.ToString();
            
            //TODO GFY THIS IS ALWAYS FALSE
            if (domainUsage == DomainUsage.None)
                CoreExtensions.Host.AddinRegistry = Services.AddinRegistry;

            package.Settings["ShadowCopyFiles"] = false;
			package.Settings["UseThreadedRunner"] = false;
            package.Settings["DefaultTimeout"] = 0;

            return package;
		}
    }
}
