using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Core.Caching.Projects;
using Rhino.Mocks;
using AutoTest.Core.Configuration;
using AutoTest.Core.Messaging;
using AutoTest.Core.FileSystem;
using AutoTest.Messages;
using System.IO;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class XUnitTestRunnerTest
    {
        private XUnitTestRunner _runner;
        private IMessageBus _bus;
        private IConfiguration _configuration;
        private IAssemblyPropertyReader _assemblyReader;
        private IFileSystemService _fsService;

        [SetUp]
        public void SetUp()
        {
            _bus = MockRepository.GenerateMock<IMessageBus>();
            _configuration = MockRepository.GenerateMock<IConfiguration>();
            _assemblyReader = MockRepository.GenerateMock<IAssemblyPropertyReader>();
            _fsService = MockRepository.GenerateMock<IFileSystemService>();
            _runner = new XUnitTestRunner(_bus, _configuration, _assemblyReader, _fsService);
        }
		
		[Test]
		public void Should_check_for_xunit_test_framework_reference()
		{
            _fsService.Stub(f => f.FileExists("")).IgnoreArguments().Return(true);
            _assemblyReader.Stub(a => a.GetTargetFramework("")).Return(new Version());
			_assemblyReader.Stub(r => r.GetReferences("")).Return(new [] { new TypeName("xunit", "xunit") });
			var change = "";
            _runner.CanHandleTestFor(change).ShouldBeTrue();
		}
    }
}
