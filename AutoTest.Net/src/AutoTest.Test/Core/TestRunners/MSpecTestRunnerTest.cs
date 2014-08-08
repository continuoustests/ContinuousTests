using System;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Configuration;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Messaging;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Messages;
using NUnit.Framework;
using Rhino.Mocks;
using System.IO;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class MSpecTestRunnerTest
    {
        [SetUp]
        public void SetUp()
        {
            _configuration = MockRepository.GenerateMock<IConfiguration>();
            _assemblyReader = MockRepository.GenerateMock<AutoTest.TestRunners.Shared.AssemblyAnalysis.IAssemblyPropertyReader>();
            _fileSystem = MockRepository.GenerateStub<IFileSystemService>();
            _externalProcess = MockRepository.GenerateStub<IExternalProcess>();
            _commandLineBuilder = MockRepository.GenerateStub<IMSpecCommandLineBuilder>();

            _runner = new MSpecTestRunner(_assemblyReader,
                                          _configuration,
                                          _fileSystem,
                                          _externalProcess,
                                          _commandLineBuilder);
        }

        MSpecTestRunner _runner;
        IConfiguration _configuration;
        AutoTest.TestRunners.Shared.AssemblyAnalysis.IAssemblyPropertyReader _assemblyReader;
        IFileSystemService _fileSystem;
        IExternalProcess _externalProcess;
        IMSpecCommandLineBuilder _commandLineBuilder;

        [Test]
        public void Should_build_the_command_line_for_each_run()
        {
            _configuration
                .Stub(x => x.MSpecTestRunner("framework 1"))
                .Return("c:\\runner 1.exe");

            _configuration
                .Stub(x => x.MSpecTestRunner("framework 2"))
                .Return("c:\\runner 2.exe");
            
            _fileSystem
                .Stub(x => x.FileExists(null))
                .IgnoreArguments()
                .Return(true);

            var document1 = new ProjectDocument(ProjectType.CSharp);
            document1.SetFramework("framework 1");
            var info1 = new TestRunInfo(new Project("key 1", document1), "assembly 1");

            var document2 = new ProjectDocument(ProjectType.CSharp);
            document2.SetFramework("framework 2");
            var info2 = new TestRunInfo(new Project("key 2", document2), "assembly 2");

            var testRunInfos = new[] { info1, info2 };

            _runner.RunTests(testRunInfos, null, null);

            _commandLineBuilder.AssertWasCalled(x => x.Build(null),
                                                o => o.IgnoreArguments().Repeat.Twice());
        }

        [Test]
        public void Should_check_for_mspec_test_framework_reference()
        {
            var assembly = String.Empty;
            _fileSystem.Stub(f => f.FileExists(assembly)).IgnoreArguments().Return(true);
            _assemblyReader.Stub(a => a.GetTargetFramework(assembly)).Return(new Version());
            _assemblyReader.Stub(r => r.GetReferences(assembly)).Return(new[] { new TypeName("Machine.Specifications", "Machine.Specifications") });

            var handles = _runner.CanHandleTestFor(assembly);

            handles.ShouldBeTrue();
        }

        [Test]
        public void Should_check_the_runner_exe_for_each_framework()
        {
            _configuration
                .Stub(x => x.MSpecTestRunner("framework 1"))
                .Return("c:\\runner 1.exe");

            _configuration
                .Stub(x => x.MSpecTestRunner("framework 2"))
                .Return("c:\\runner 2.exe");

            var document1 = new ProjectDocument(ProjectType.CSharp);
            document1.SetFramework("framework 1");
            var info1 = new TestRunInfo(new Project("key 1", document1), "assembly 1");

            var document2 = new ProjectDocument(ProjectType.CSharp);
            document2.SetFramework("framework 2");
            var info2 = new TestRunInfo(new Project("key 2", document2), "assembly 2");

            var testRunInfos = new[] { info1, info2 };

            _runner.RunTests(testRunInfos, null, null);

            _fileSystem.AssertWasCalled(x => x.FileExists("c:\\runner 1.exe"));
            _fileSystem.AssertWasCalled(x => x.FileExists("c:\\runner 2.exe"));
        }

        [Test]
        public void Should_run_tests_for_each_framework_with_an_existing_runner()
        {
            _configuration
                .Stub(x => x.MSpecTestRunner("framework 1"))
                .Return("c:\\runner 1.exe");

            _configuration
                .Stub(x => x.MSpecTestRunner("framework 2"))
                .Return("c:\\runner 2.exe");

            _fileSystem
                .Stub(x => x.FileExists("c:\\runner 2.exe"))
                .Return(true);

            var document1 = new ProjectDocument(ProjectType.CSharp);
            document1.SetFramework("framework 1");
            var info1 = new TestRunInfo(new Project("key 1", document1), "assembly 1");

            var document2 = new ProjectDocument(ProjectType.CSharp);
            document2.SetFramework("framework 2");
            var info2 = new TestRunInfo(new Project("key 2", document2), "assembly 2");
            info2.AddTestsToRun(new[]
                                {
                                    new TestToRun(TestRunner.MSpec, "test 1"),
                                });

            var testRunInfos = new[] { info1, info2 };

            _runner.RunTests(testRunInfos, null, null);

            _externalProcess.AssertWasNotCalled(
                x => x.CreateAndWaitForExit(Arg<string>.Matches(y => y == "c:\\runner 1.exe"),
                                                   Arg<string>.Is.Anything));

            _externalProcess.AssertWasCalled(
                x => x.CreateAndWaitForExit(Arg<string>.Matches(y => y == "c:\\runner 2.exe"),
                                                   Arg<string>.Is.Anything));
        }
    }
}