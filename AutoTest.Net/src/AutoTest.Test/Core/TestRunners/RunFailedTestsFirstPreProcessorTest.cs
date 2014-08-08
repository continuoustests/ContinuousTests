using System;
using NUnit.Framework;
using AutoTest.Core.Caching.RunResultCache;
using Rhino.Mocks;
using AutoTest.Core.TestRunners;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Caching.Projects;
using AutoTest.Messages;
namespace AutoTest.Test
{
	[TestFixture]
	public class RunFailedTestsFirstPreProcessorTest
	{
		private IRunResultCache _resultCache;
		private RunFailedTestsFirstPreProcessor _preProcessor;
		
		[SetUp]
		public void SetUp()
		{
			_resultCache = MockRepository.GenerateMock<IRunResultCache>();
			_preProcessor = new RunFailedTestsFirstPreProcessor(_resultCache);
		}
		
		[Test]
		public void Should_run_cached_failed_and_ignored_tests_and_mark_for_rerun()
		{
			_resultCache.Stub(r => r.Failed).Return(new TestItem[] { new TestItem("assembly", "project", new TestResult(TestRunner.MSTest, TestRunStatus.Failed, "sometests")) });
			_resultCache.Stub(r => r.Ignored).Return(new TestItem[] { new TestItem("assembly", "project", new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "someignoredtests")) });
			var details = new RunInfo(new Project("project", new ProjectDocument(ProjectType.CSharp)));
			details.SetAssembly("assembly");
			
			_preProcessor.PreProcess(new PreProcessedTesRuns(null, new RunInfo[] { details }));
			details.GetTests().Length.ShouldEqual(2);
            details.GetTests()[0].Test.ShouldEqual("sometests");
            details.GetTests()[1].Test.ShouldEqual("someignoredtests");
			details.OnlyRunSpcifiedTestsFor(TestRunner.Any).ShouldBeTrue();
			details.RerunAllTestWhenFinishedFor(TestRunner.Any).ShouldBeTrue();
		}
	}
}

