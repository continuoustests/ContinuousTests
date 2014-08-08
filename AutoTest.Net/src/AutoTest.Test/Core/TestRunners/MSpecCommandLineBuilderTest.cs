using System.Linq;
using System.Text.RegularExpressions;

using AutoTest.Core.Caching.Projects;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Messages;

using NUnit.Framework;

using Rhino.Mocks;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class MSpecCommandLineBuilderTest
    {
        [SetUp]
        public void SetUp()
        {
            _fileSystem = MockRepository.GenerateStub<IFileSystemService>();
            _builder = new MSpecCommandLineBuilder(_fileSystem);
        }

        MSpecCommandLineBuilder _builder;
        IFileSystemService _fileSystem;

        [Test]
        public void Should_report_the_time_info()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetFramework("framework 1");
            var info = new TestRunInfo(new Project("key 1", document), "assembly 1");

            var infos = new[] { info };
            var run = new MSpecTestRunner.Run { RunInfos = infos };

            var args = _builder.Build(run);

            Assert.That(args, Is.StringContaining("--timeinfo"));
        }
        
        [Test]
        public void Should_create_an_xml_report()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetFramework("framework 1");
            var info = new TestRunInfo(new Project("key 1", document), "assembly 1");

            var infos = new[] { info };
            var run = new MSpecTestRunner.Run { RunInfos = infos };

            var args = _builder.Build(run);

            Assert.That(args, Is.StringContaining("--xml"));
            Assert.That(run.Cleanups.Count(), Is.EqualTo(1));
            Assert.That(run.Harvesters.Count(), Is.EqualTo(1));
        } 
        
        [Test]
        public void Should_create_the_assembly_list()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetFramework("framework 1");
            var info1 = new TestRunInfo(new Project("key 1", document), "assembly 1");
            var info2 = new TestRunInfo(new Project("key 2", document), "assembly 2");

            var infos = new[] { info1, info2 };
            var run = new MSpecTestRunner.Run { RunInfos = infos };

            var args = _builder.Build(run);

            Assert.That(args, Is.StringContaining(" \"assembly 1\""));
            Assert.That(args, Is.StringContaining(" \"assembly 2\""));
        }
        
        [Test]
        public void Should_create_the_assembly_list_from_distinct_assembly_names()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetFramework("framework 1");
            var info1 = new TestRunInfo(new Project("key 1", document), "assembly 1");
            var info2 = new TestRunInfo(new Project("key 2", document), "assembly 1");

            var infos = new[] { info1, info2 };
            var run = new MSpecTestRunner.Run { RunInfos = infos };

            var args = _builder.Build(run);

            var assembly1Count = new Regex("assembly 1").Matches(args).Count;
            Assert.That(assembly1Count, Is.EqualTo(1));
        }
        
        [Test]
        public void Should_create_a_filter_file_when_tests_to_run_are_specified()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetFramework("framework 1");
            var info = new TestRunInfo(new Project("key 1", document), "assembly 1");
            info.AddTestsToRun(new[]
                               {
                                   new TestToRun(TestRunner.MSpec, "test 1"),
                                   new TestToRun(TestRunner.MSpec, "test 2")
                               });

            var infos = new[] { info };
            var run = new MSpecTestRunner.Run { RunInfos = infos };

            var args = _builder.Build(run);

            Assert.That(args, Is.StringContaining("--filter"));
            Assert.That(run.Cleanups.Count(), Is.EqualTo(2), "Should contain cleanup for XML report and filter");
        }

        [Test]
        public void Should_not_create_a_filter_file_when_all_tests_are_run()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetFramework("framework 1");
            var info = new TestRunInfo(new Project("key 1", document), "assembly 1");

            var infos = new[] { info };
            var run = new MSpecTestRunner.Run { RunInfos = infos };

            var args = _builder.Build(run);

            Assert.That(args, Is.Not.StringContaining("--filter"));
        }
    }
}