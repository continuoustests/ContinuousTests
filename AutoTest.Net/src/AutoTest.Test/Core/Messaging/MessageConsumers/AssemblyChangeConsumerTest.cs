using System;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.TestRunners;
using AutoTest.Core.Messaging;
using AutoTest.Core.FileSystem;
using Rhino.Mocks;
using AutoTest.Messages;
using AutoTest.Core.Caching;
using AutoTest.Core.Configuration;
using AutoTest.Core.Caching.Projects;
namespace AutoTest.Test
{
	[TestFixture]
	public class AssemblyChangeConsumerTest
	{
		private AssemblyChangeConsumer _consumer;
        private IMessageBus _bus;
        private ITestRunner _testRunner;
		private IDetermineIfAssemblyShouldBeTested _testAssemblyValidator;
        private IPreProcessTestruns _preProcessor;
        private ILocateRemovedTests _removedTestLocator;
		private ICache _cache;
		private IConfiguration _config;

        [SetUp]
        public void SetUp()
        {
            _bus = MockRepository.GenerateMock<IMessageBus>();
            _testRunner = MockRepository.GenerateMock<ITestRunner>();
			_testAssemblyValidator = MockRepository.GenerateMock<IDetermineIfAssemblyShouldBeTested>();
            _preProcessor = MockRepository.GenerateMock<IPreProcessTestruns>();
            var preProcessors = new IPreProcessTestruns[] { _preProcessor };
            _removedTestLocator = MockRepository.GenerateMock<ILocateRemovedTests>();
			_cache = MockRepository.GenerateMock<ICache>();
			_config = MockRepository.GenerateMock<IConfiguration>();
			_cache.Stub(x => x.GetAll<Project>()).Return(new Project[] {});

            _consumer = new AssemblyChangeConsumer(new ITestRunner[] { _testRunner }, _bus, _testAssemblyValidator, preProcessors, _removedTestLocator, _cache, _config);
			_testRunner.Stub(r => r.RunTests(null, null, null)).IgnoreArguments().Return(new TestRunResults[] {});
        }
		
		[Test]
		public void Should_run_tests()
		{
			_testAssemblyValidator.Stub(t => t.ShouldNotTestAssembly(null)).Return(false);
			_testRunner.Stub(t => t.CanHandleTestFor("")).IgnoreArguments().Return(true);
            _preProcessor.Stub(p => p.PreProcess(null)).IgnoreArguments().Return(new PreProcessedTesRuns(null, new RunInfo[] { new RunInfo(new AutoTest.Core.Caching.Projects.Project("", null)) })); 
			var message = new AssemblyChangeMessage();
			message.AddFile(new ChangedFile());
			_testAssemblyValidator.Stub(t => t.ShouldNotTestAssembly(null)).IgnoreArguments().Return(true);
			_consumer.Consume(message);
			_testRunner.AssertWasCalled(t => t.RunTests(null, null, null), t => t.IgnoreArguments());
		}

        [Test]
        public void Should_run_pre_processors()
        {
            _testAssemblyValidator.Stub(t => t.ShouldNotTestAssembly("")).Return(false);
            _testRunner.Stub(t => t.CanHandleTestFor("")).IgnoreArguments().Return(true);
            var message = new AssemblyChangeMessage();
            message.AddFile(new ChangedFile());
            _testAssemblyValidator.Stub(t => t.ShouldNotTestAssembly(null)).IgnoreArguments().Return(true);
            _consumer.Consume(message);
            _preProcessor.AssertWasCalled(p => p.PreProcess(null), p => p.IgnoreArguments());
        }
		
		[Test]
		public void Should_not_run_tests_for_assemblies_that_runner_doesnt_support()
		{
			_testAssemblyValidator.Stub(t => t.ShouldNotTestAssembly("")).IgnoreArguments().Return(false);
			_testRunner.Stub(t => t.CanHandleTestFor("")).IgnoreArguments().Return(false);
			var message = new AssemblyChangeMessage();
			message.AddFile(new ChangedFile());
			_testAssemblyValidator.Stub(t => t.ShouldNotTestAssembly(null)).IgnoreArguments().Return(true);
			_consumer.Consume(message);
			_testRunner.AssertWasNotCalled(t => t.RunTests(null, null, null), t => t.IgnoreArguments());
		}
		
		[Test]
		public void Should_ignore_test_assembly()
		{
			_testAssemblyValidator.Stub(t => t.ShouldNotTestAssembly("")).Return(true);
			_testRunner.Stub(t => t.CanHandleTestFor("")).IgnoreArguments().Return(true);
			var message = new AssemblyChangeMessage();
			message.AddFile(new ChangedFile());
			_testAssemblyValidator.Stub(t => t.ShouldNotTestAssembly(null)).IgnoreArguments().Return(true);
			_consumer.Consume(message);
			_testRunner.AssertWasNotCalled(t => t.RunTests(null, null, null), t => t.IgnoreArguments());
		}
	}
}

