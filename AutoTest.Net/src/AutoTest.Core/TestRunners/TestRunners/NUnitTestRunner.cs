using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using System.IO;
using System.Diagnostics;
using Castle.Core.Logging;
using AutoTest.Core.Messaging;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;
using AutoTest.Core.FileSystem;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.Core.TestRunners.TestRunners
{
    public class NUnitTestRunner : ITestRunner
    {
        private IMessageBus _bus;
        private IConfiguration _configuration;
		private IAssemblyPropertyReader _assemblyReader;
        private IFileSystemService _fsService;

        public NUnitTestRunner(IMessageBus bus, IConfiguration configuration, IAssemblyPropertyReader assemblyReader, IFileSystemService fsService)
        {
            _bus = bus;
            _configuration = configuration;
			_assemblyReader = assemblyReader;
            _fsService = fsService;
        }

        #region ITestRunner Members

        public bool CanHandleTestFor(string assembly)
		{
            var framework = _assemblyReader.GetTargetFramework(assembly);
            if (!_fsService.FileExists(_configuration.NunitTestRunner(string.Format("v{0}.{1}", framework.Major, framework.Minor))))
                return false;

            return _assemblyReader.GetReferences(assembly).Count(x => x.Name.Equals("nunit.framework")) > 0;
		}

        public TestRunResults[] RunTests(TestRunInfo[] runInfos, Action<AutoTest.TestRunners.Shared.Targeting.Platform, Version, Action<ProcessStartInfo, bool>> processWrapper, Func<bool> abortWhen)
        {
			var results = new List<TestRunResults>();
			// Get a list of the various nunit executables specified pr. framework version
			var nUnitExes = getNUnitExes(runInfos);
			foreach (var nUnitExe in nUnitExes)
			{
				// Get the assemblies that should be run under this nunit executable
				string tests;
				var assemblies = getAssembliesAndTestsForTestRunner(nUnitExe.Exe, runInfos, out tests);
                if (assemblies == null)
                    continue;
				var arguments = getExecutableArguments(nUnitExe, assemblies, tests, runInfos);
                DebugLog.Debug.WriteInfo("Running tests: {0} {1}", nUnitExe.Exe, arguments); 
	            var proc = new Process();
	            proc.StartInfo = new ProcessStartInfo(nUnitExe.Exe, arguments);
	            proc.StartInfo.RedirectStandardOutput = true;
	            //proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(runInfo.Assembly);
	            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
	            proc.StartInfo.UseShellExecute = false;
	            proc.StartInfo.CreateNoWindow = true;
	
	            proc.Start();
	            var parser = new NUnitTestResponseParser(_bus, TestRunner.NUnit);
                var nUnitResult = getNUnitOutput(proc.StandardOutput);
			    parser.Parse(nUnitResult, runInfos, containsTests(arguments));
				foreach (var result in parser.Result)
		            results.Add(result);
			}
			return results.ToArray();
        }

        private bool containsTests(string arguments)
        {
            return arguments.Contains(getArgumentSeparator() + "run=");
        }
		
		private RunnerExe[] getNUnitExes(TestRunInfo[] runInfos)
		{
			var testRunnerExes = new List<RunnerExe>();
			foreach (var runInfo in runInfos)
			{
				var framework = getFramework(runInfo);
				var unitTestExe = _configuration.NunitTestRunner(framework);
				if (_configuration.GetSpesificNunitTestRunner(framework) == null)
					framework = "";
	            if (File.Exists(unitTestExe))
				{
					if (!testRunnerExes.Exists(x => x.Equals(new RunnerExe(unitTestExe, framework))))
	                	testRunnerExes.Add(new RunnerExe(unitTestExe, framework));
				}
			}
			return testRunnerExes.ToArray();
		}
		
		private string getFramework(TestRunInfo runInfo)
		{
			if (runInfo.Project == null)
				return "";
			return runInfo.Project.Value.Framework;
		}
		
		private string getAssembliesAndTestsForTestRunner(string testRunnerExes, TestRunInfo[] runInfos, out string tests)
		{
			var separator = getArgumentSeparator();
			var assemblies = "";
			tests = "";
			foreach (var runInfo in runInfos)
			{
                DebugLog.Debug.WriteDetail("About to add {0}", runInfo.Assembly);
				var unitTestExe = _configuration.NunitTestRunner(getFramework(runInfo));
				if (unitTestExe.Equals(testRunnerExes))
				{
                    DebugLog.Debug.WriteDetail("It only run specified tests is {0} and test count is {1}", runInfo.OnlyRunSpcifiedTestsFor(TestRunner.NUnit), runInfo.GetTestsFor(TestRunner.NUnit).Length);
                    if (runInfo.OnlyRunSpcifiedTestsFor(TestRunner.NUnit) && runInfo.GetTestsFor(TestRunner.NUnit).Length.Equals(0))
                        continue;
					assemblies += string.Format("\"{0}\"", runInfo.Assembly) + " ";
					var assemblyTests = getTestsList(runInfo);
                    DebugLog.Debug.WriteDetail("Test list is {0}", assemblyTests);
					if (assemblyTests.Length > 0)
						tests += (tests.Length > 0 ? "," : "") + assemblyTests;
				}
			}
			if (tests.Length > 0)
				tests = string.Format("{0}run={1}", separator, tests);
            if (assemblies.Length.Equals(0))
                return null;
			return assemblies;
		}
        
        string getExecutableArguments (RunnerExe exe, string assemblyName, string tests, TestRunInfo[] runInfos)
		{
			var calc = new MaxCmdLengthCalculator();
			var separator = getArgumentSeparator();
			string framework = "";
			// only use framework for windows as the default runner on linux has no framework parameter
			if (OS.IsWindows)
			{
				if (exe.Version.Length > 0)
					framework = string.Format(" {0}framework:{1}", separator, exe.Version);
			}
			var categoryList = getCategoryIgnoreList();
			var arguments = string.Format("{0}noshadow{2} {0}xmlconsole {1}", separator, categoryList, framework) + assemblyName + " " + tests;
			if ((arguments.Length + exe.Exe.Length) > calc.GetLength())
				arguments = string.Format("{0}noshadow{2} {0}xmlconsole {1}", separator, categoryList, framework) + assemblyName;
			return arguments;
		}

        #endregion
		
		private string getTestsList(TestRunInfo runInfo)
		{
			var tests = "";
			foreach (var test in runInfo.GetTestsFor(TestRunner.NUnit))
				tests += (tests.Length > 0 ? "," : "") + test;
			return tests;
		}
		
		private string getCategoryIgnoreList()
		{
			var separator = getArgumentSeparator();
			string categoryList = "";
			foreach (var category in _configuration.TestCategoriesToIgnore)
			{
				categoryList += (categoryList.Length > 0 ? "," : "") + category;
			}
			if (categoryList.Length > 0)
				categoryList = separator + "exclude=" + categoryList + " ";
			return categoryList;
		}
		
		private string getArgumentSeparator()
		{
			if (OS.IsWindows)
			{
				return "/";
			}
			else
			{
				return "--";
			}
		}

        private string getNUnitOutput(StreamReader streamReader)
        {
            var stringBuilder = new StringBuilder();

            while (streamReader.EndOfStream == false)
            {
                var readLine = streamReader.ReadLine();
                stringBuilder.Append(readLine);

                // checking for the last expected line because the test 
                // runner suspends after the last line is hit and leaves the agent hanging
                if (nunitEndOfStream(readLine))
                    return stringBuilder.ToString();
            }

            return stringBuilder.ToString();
        }

        private static bool nunitEndOfStream(string readLine)
        {
            return string.IsNullOrEmpty(readLine) == false && readLine.EndsWith("</test-results>");
        }
    }
}
