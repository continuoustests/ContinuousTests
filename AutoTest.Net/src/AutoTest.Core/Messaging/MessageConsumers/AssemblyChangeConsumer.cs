using System;
using AutoTest.Core.TestRunners;
using System.Collections.Generic;
using AutoTest.Core.DebugLog;
using AutoTest.Messages;
using System.Threading;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using System.Linq;
using AutoTest.Core.Configuration;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	public class AssemblyChangeConsumer : IOverridingConsumer<AssemblyChangeMessage>, IConsumerOf<AbortMessage>
	{
		private ITestRunner[] _testRunners;
		private IMessageBus _bus;
		private IDetermineIfAssemblyShouldBeTested _testAssemblyValidator;
        private IPreProcessTestruns[] _preProcessors;
        private ILocateRemovedTests _removedTestLocator;
		private ICache _cache;
		private IConfiguration _config;
        private bool _isRunning = false;
        private bool _exit = false;
        private List<RunInfo> _abortedTestRuns = new List<RunInfo>();

        public bool IsRunning { get { return _isRunning; } }

        public AssemblyChangeConsumer(ITestRunner[] testRunners, IMessageBus bus, IDetermineIfAssemblyShouldBeTested testAssemblyValidator, IPreProcessTestruns[] preProcessors, ILocateRemovedTests removedTestLocator, ICache cache, IConfiguration config)
		{
			_testRunners = testRunners;
			_bus = bus;
			_testAssemblyValidator = testAssemblyValidator;
            _preProcessors = preProcessors;
            _removedTestLocator = removedTestLocator;
			_cache = cache;
			_config = config;
		}
		
		#region IConsumerOf[AssemblyChangeMessage] implementation
		public void Consume (AssemblyChangeMessage message)
		{
            _isRunning = true;
            var runReport = new RunReport();
            try
            {
			    informParticipants(message);
                var runInfos = getRunInfos(message);
                var preProcessed = preProcess(runInfos);
                preProcessed = new PreProcessedTesRuns(preProcessed.ProcessWrapper, new TestRunInfoMerger(preProcessed.RunInfos).MergeByAssembly(_abortedTestRuns).ToArray());
                foreach (var runner in _testRunners)
                {
                    runTest(runner, preProcessed, runReport);
                    if (_exit)
                    {
                        _abortedTestRuns.Clear();
                        _abortedTestRuns.AddRange(preProcessed.RunInfos);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                var result = new TestRunResults("", "", false, TestRunner.Any, new TestResult[] { new TestResult(TestRunner.Any, TestRunStatus.Failed, "AutoTest.Net internal error", ex.ToString()) });
                _bus.Publish(new TestRunMessage(result));
            }
            if (_exit)
                runReport.WasAborted();
            _bus.Publish(new RunFinishedMessage(runReport));
            if (!_exit)
                _abortedTestRuns.Clear();
            _exit = false;
            _isRunning = false;
		}

        public void Consume(AbortMessage message)
        {
            Terminate();
        }

        public void Terminate()
        {
            if (!_isRunning)
                return;
            Debug.WriteDebug("Initiating termination of current run");
            _exit = true;
            var timeout = DateTime.Now.AddSeconds(4);
            while (_isRunning && timeout > DateTime.Now)
                Thread.Sleep(10);
        }

		#endregion

        private PreProcessedTesRuns preProcess(RunInfo[] runInfos)
        {
            var preProcessed = new PreProcessedTesRuns(null, runInfos);
            foreach (var preProcessor in _preProcessors)
                preProcessed = preProcessor.PreProcess(preProcessed);
            return preProcessed;
        }
		private RunInfo[] getRunInfos(AssemblyChangeMessage message)
        {
			var projects = _cache.GetAll<Project>();
            var runInfos = new List<RunInfo>();
            foreach (var file in message.Files)
            {
				var project = projects.Where(x => x.GetAssembly(_config.CustomOutputPath).Equals(file.FullName)).FirstOrDefault();
                var runInfo = new RunInfo(project);
                runInfo.SetAssembly(file.FullName);
                runInfos.Add(runInfo);
            }
            return runInfos.ToArray();
        }

		private void informParticipants(AssemblyChangeMessage message)
		{
			Debug.ConsumingAssemblyChangeMessage(message);
            _bus.Publish(new RunStartedMessage(message.Files));
		}

        private void runTest(ITestRunner runner, PreProcessedTesRuns preProcessed, RunReport report)
		{
			var testRunInfos = new List<TestRunInfo>();
            foreach (var runInfo in preProcessed.RunInfos)
			{
				if (_testAssemblyValidator.ShouldNotTestAssembly(runInfo.Assembly))
					return;
                if (runner.CanHandleTestFor(runInfo.Assembly))
                {
                    testRunInfos.Add(runInfo.CloneToTestRunInfo());
                    _bus.Publish(new RunInformationMessage(InformationType.TestRun, "", runInfo.Assembly, runner.GetType()));
                }
			}
            if (testRunInfos.Count == 0)
				return;
            var results = runner.RunTests(testRunInfos.ToArray(), preProcessed.ProcessWrapper, () => { return _exit; });
            if (_exit)
                return;
			mergeReport(results, report, testRunInfos.ToArray());
            reRunTests(runner, report, testRunInfos, preProcessed.ProcessWrapper);
		}

        private void reRunTests(ITestRunner runner, RunReport report, List<TestRunInfo> testRunInfos, Action<AutoTest.TestRunners.Shared.Targeting.Platform, Version, Action<System.Diagnostics.ProcessStartInfo, bool>> processWrapper)
        {
            var rerunInfos = new List<TestRunInfo>();
            foreach (var info in testRunInfos)
            {
                if (info.RerunAllTestWhenFinishedForAny())
                    rerunInfos.Add(new TestRunInfo(info.Project, info.Assembly));
            }
            if (rerunInfos.Count > 0)
            {
                Debug.WriteDebug("Rerunning all tests for runner " + runner.GetType().ToString());
                var results = runner.RunTests(testRunInfos.ToArray(), processWrapper, () => { return _exit; });
                mergeReport(results, report, testRunInfos.ToArray());
            }
        }
		
		private void mergeReport(TestRunResults[] results, RunReport report, TestRunInfo[] runInfos)
		{
            var modifiedResults = new List<TestRunResults>();
			foreach (var result in results)
			{
                var modified = _removedTestLocator.SetRemovedTestsAsPassed(result, runInfos);
	            report.AddTestRun(
                    modified.Project,
                    modified.Assembly,
                    modified.TimeSpent,
                    modified.Passed.Length,
                    modified.Ignored.Length,
                    modified.Failed.Length);
                _bus.Publish(new TestRunMessage(modified));
                modifiedResults.Add(modified);
			}
            informPreProcessor(modifiedResults.ToArray());
		}

        private void informPreProcessor(TestRunResults[] results)
        {
            foreach (var preProcess in _preProcessors)
                preProcess.RunFinished(results);
        }
	}
}

