using System;
using System.Collections.Generic;
using System.Linq;
using AutoTest.Core.Configuration;
using System.Diagnostics;
using System.IO;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.FileSystem;
using AutoTest.Messages;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.Core.TestRunners.TestRunners
{
    public class MSTestRunner : ITestRunner
    {
        private readonly IConfiguration _configuration;
        private readonly IAssemblyPropertyReader _assemblyReader;
        private readonly IFileSystemService _fsService;

        public MSTestRunner(IConfiguration configuration, IAssemblyPropertyReader referenceResolver, IFileSystemService fsService)
        {
            _configuration = configuration;
			_assemblyReader = referenceResolver;
            _fsService = fsService;
        }

        #region ITestRunner Members

        public bool CanHandleTestFor(string assembly)
		{
            var framework = _assemblyReader.GetTargetFramework(assembly);

            string runner = _configuration.MSTestRunner(string.Format("v{0}.{1}", framework.Major, framework.Minor));

            if (!_fsService.FileExists(runner))
            {
                DebugLog.Debug.WriteInfo(string.Format("File not found: {0}", runner));

                return false;
            }

            return _assemblyReader.GetReferences(assembly).Count(x => x.Name.Equals("Microsoft.VisualStudio.QualityTools.UnitTestFramework")) > 0;
		}

        public TestRunResults[] RunTests(TestRunInfo[] runInfos, Action<AutoTest.TestRunners.Shared.Targeting.Platform, Version, Action<ProcessStartInfo, bool>> processWrapper, Func<bool> abortWhen)
        {
			var results = new List<TestRunResults>();
			foreach (var runInfo in runInfos)
			{			
	            var timer = Stopwatch.StartNew();
	            var unitTestExe = _configuration.MSTestRunner(getFramework(runInfo));
	            if (!File.Exists(unitTestExe))
				{
					var project = "";
					if (runInfo.Project != null)
						project = runInfo.Project.Key;
	                results.Add(new TestRunResults(project, runInfo.Assembly, false, TestRunner.MSTest, new TestResult[] { }));
					continue;
				}
				
                if (runInfo.OnlyRunSpcifiedTestsFor(TestRunner.MSTest) && runInfo.GetTestsFor(TestRunner.MSTest).Length.Equals(0))
                    continue;
				var calc = new MaxCmdLengthCalculator();
				var tests = getTestsList(runInfo);
                var testRunConfig = getTestrunConfigArguments();
				var arguments = "/testcontainer:\"" + runInfo.Assembly + "\" " + tests + " /detail:errorstacktrace /detail:errormessage" + testRunConfig;
                var runAllTests = (arguments.Length + unitTestExe.Length) > calc.GetLength();
				if (runAllTests)
                    arguments = "/testcontainer:\"" + runInfo.Assembly + "\"" + " /detail:errorstacktrace /detail:errormessage" + testRunConfig;
				DebugLog.Debug.WriteInfo("Running tests: {0} {1}", unitTestExe, arguments); 
	            var proc = new Process();
	            proc.StartInfo = new ProcessStartInfo(unitTestExe, arguments);
	            proc.StartInfo.RedirectStandardOutput = true;
	            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(runInfo.Assembly);
	            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
	            proc.StartInfo.UseShellExecute = false;
	            proc.StartInfo.CreateNoWindow = true;
	
	            proc.Start();
	            string line;
	            var parser = new MSTestResponseParser(runInfo.Project.Key, runInfo.Assembly, !runAllTests);
	            while ((line = proc.StandardOutput.ReadLine()) != null)
	                parser.ParseLine(line);
	            proc.WaitForExit();
	            timer.Stop();
	            parser.Result.SetTimeSpent(timer.Elapsed);
	            results.Add(parser.Result);
			}
			return results.ToArray();
        }

        private string getTestrunConfigArguments()
        {
            var parser = new MSTestRunConfigParser(_configuration, _fsService);
            var testRunConfig = parser.GetConfig();
            if (testRunConfig == null)
                return "";
            return string.Format(" /runconfig:\"{0}\"", testRunConfig);
        }

        #endregion
		
		private string getTestsList(TestRunInfo runInfo)
		{
			var tests = "";
			foreach (var test in runInfo.GetTestsFor(TestRunner.MSTest))
				tests += string.Format("/test:{0} ", test);
			return tests;
		}
		
		private string getFramework(TestRunInfo runInfo)
		{
			if (runInfo.Project == null)
				return "";
			return runInfo.Project.Value.Framework;
		}
    }
}
