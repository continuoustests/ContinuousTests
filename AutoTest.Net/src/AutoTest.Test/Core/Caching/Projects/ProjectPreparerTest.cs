using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Caching.Projects;
using AutoTest.Test.Core.Caching.Projects.Fakes;
using System.IO;
using AutoTest.Core.Caching;

namespace AutoTest.Test.Core.Caching.Projects
{
    [TestFixture]
    public class ProjectPreparerTest
    {
        private ProjectPreparer _preparer;
        private FakeCache _cache;
        private FakeProjectParser _parser;
        private string _testProject;
        private ProjectDocument _parsedDocument;

        [SetUp]
        public void SetUp()
        {
            _parsedDocument = new ProjectDocument(ProjectType.CSharp);
            _parsedDocument.AddReference("ReferencedProject");
            _parser = new FakeProjectParser(new ProjectDocument[] { _parsedDocument });
            _cache = new FakeCache();
            _preparer = new ProjectPreparer(_parser, _cache);
            _testProject = Path.GetFullPath(string.Format("TestResources{0}VS2008{0}CSharpNUnitTestProject.csproj", Path.DirectorySeparatorChar));
        }

        [Test]
        public void When_already_prepared_return_null()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.HasBeenReadFromFile();
            var record = new Project("someproject", document);
            var project = _preparer.Prepare(record);
            project.ShouldBeTheSameAs(record);
        }

        [Test]
        public void Should_prepare_project()
        {
            var record = new Project("someproject", null);
            _cache.WhenGeting("ReferencedProject")
                .Return(new Project("", new ProjectDocument(ProjectType.CSharp)));
            var project = _preparer.Prepare(record);
            project.Value.ShouldNotBeNull();
        }

        [Test]
        public void Should_Add_ReferencedProjects()
        {
            _cache.WhenGeting("ReferencedProject")
                .Return(new Project("NonExisting", new ProjectDocument(ProjectType.CSharp)));
            var record = new Project(_testProject, null);
            var project = _preparer.Prepare(record);
            project.ShouldNotBeNull();
            project.Value.References.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_populate_referenced_by()
        {
            var record = new Project("someproject", null);
            var referencedProject = new ProjectDocument(ProjectType.CSharp);
            _cache.WhenGeting("ReferencedProject")
                .Return(new Project("", referencedProject));
            var project = _preparer.Prepare(record);
            project.ShouldNotBeNull();
            referencedProject.ReferencedBy[0].ShouldEqual("someproject");
        }

        [Test]
        public void Should_return_null_when_parse_fails()
        {
            var record = new Project("someproject", null);
            _parser.ThrowExceptionOnParse();
            var project = _preparer.Prepare(record);
            project.ShouldBeNull();
        }

        [Test]
        public void Should_add_files()
        {
            _parsedDocument.AddFile(new ProjectFile("File1", FileType.Compile, "someproject"));
            var record = new Project("someproject", _parsedDocument);
            _cache.WhenGeting("ReferencedProject").Return(new Project("", new ProjectDocument(ProjectType.CSharp)));
            _preparer.Prepare(record);
            _cache.GetAllProjectFiles().Length.ShouldEqual(1);
        }

        [Test]
        public void Should_invalidate_files()
        {
            _parsedDocument.AddFile(new ProjectFile("File1", FileType.Compile, "someproject"));
            _cache.Add(new ProjectFile[] { new ProjectFile("File2", FileType.Compile, "someproject") });
            var record = new Project("someproject", _parsedDocument);
            _cache.WhenGeting("ReferencedProject").Return(new Project("", new ProjectDocument(ProjectType.CSharp)));
            _preparer.Prepare(record);
            _cache.GetAllProjectFiles().Length.ShouldEqual(1);
            _cache.GetAllProjectFiles()[0].File.ShouldEqual("File1");
        }
    }
}
