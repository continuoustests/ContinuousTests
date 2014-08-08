using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using AutoTest.Test.Core.Caching.Projects.Fakes;
using System.IO;

namespace AutoTest.Test.Core.Caching
{
    [TestFixture]
    public class CacheTest
    {
        private Cache _cache;
        private FakeProjectParser _parser;
        private ProjectDocument _firstDocument;
        private ProjectDocument _secondDocument;
        private ProjectDocument _thirdDocument;
        private string _testProject;
        private string _testProjectVB;
        private string _testProjectFS;

        [SetUp]
        public void SetUp()
        {
            _firstDocument = new ProjectDocument(ProjectType.CSharp);
            _firstDocument.HasBeenReadFromFile();
            _secondDocument = new ProjectDocument(ProjectType.CSharp);
            _secondDocument.HasBeenReadFromFile();
            _thirdDocument = new ProjectDocument(ProjectType.FSharp);
            _thirdDocument.HasBeenReadFromFile();
            _parser = new FakeProjectParser(new ProjectDocument[] { _secondDocument, _thirdDocument, _firstDocument });
            _cache = new Cache(new FakeServiceLocator(_parser, delegate { return _cache; }));
            _testProject = Path.GetFullPath(string.Format("TestResources{0}VS2008{0}CSharpNUnitTestProject.csproj", Path.DirectorySeparatorChar));
            _testProjectVB = Path.GetFullPath(string.Format("TestResources{0}VS2008{0}NUnitTestProjectVisualBasic.vbproj", Path.DirectorySeparatorChar));
            _testProjectFS = Path.GetFullPath(string.Format("TestResources{0}VS2008{0}FSharpNUnitTestProject.fsproj", Path.DirectorySeparatorChar));
        }

        [Test]
        public void Can_Add_And_Get_Project()
        {
            _cache.Add<Project>(_testProject);
            _cache.Get<Project>(_testProject).ShouldNotBeNull();
        }

        [Test]
        public void Can_Get_By_Index()
        {
            _cache.Add<Project>(_testProject);
            _cache.Get<Project>(0).ShouldNotBeNull();
        }

        [Test]
        public void Can_Check_That_Project_Exists()
        {
            _cache.Add<Project>(_testProject);
            _cache.Exists(_testProject).ShouldBeTrue();
        }

        [Test]
        public void Should_Not_Add_Duplicates()
        {
            _cache.Add<Project>(_testProject);
            _cache.Add<Project>(_testProject);
            _cache.Count.ShouldEqual(1);
        }

        [Test]
        public void Should_read_project_only_once()
        {
            _cache.Add<Project>(_testProject);
            _cache.Get<Project>(_testProject).Value.ShouldBeTheSameAs(_firstDocument);
        }

        [Test]
        public void When_Geting_Placeholder_It_Should_Parse_Project()
        {
            _cache.Add<Project>(_testProject);
            _cache.Get<Project>(_testProject).Value.ShouldNotBeNull();
        }

        [Test]
        public void Should_enumerate_cached_items()
        {
            _cache.Add<Project>(_testProject);
            _cache.Add<Project>(_testProjectVB);
            _cache.Add<Project>(_testProjectFS);
            var projects = _cache.GetAll<Project>();
            projects.Length.ShouldEqual(3);
            projects[0].Key.ShouldEqual(_testProject);
            projects[0].Value.ShouldNotBeNull();
            projects[1].Key.ShouldEqual(_testProjectVB);
            projects[1].Value.ShouldNotBeNull();
            projects[2].Key.ShouldEqual(_testProjectFS);
            projects[2].Value.ShouldNotBeNull();
        }

        [Test]
        public void Should_remove_projects_that_cant_be_prepared()
        {
            _parser.ThrowExceptionOnParse();
            try
            {
                _cache.Add<Project>(_testProject);
            }
            catch
            {
            }
            var projects = _cache.GetAll<Project>();
            projects.Length.ShouldEqual(0);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void Should_throw_exception_on_failing_prepare()
        {
            _parser.ThrowExceptionOnParse();
            _cache.Add<Project>(_testProject);
        }

        [Test]
        public void Should_add_and_get_files()
        {
            var files = new List<ProjectFile>();
            files.Add(new ProjectFile("File1", FileType.Resource, "Project1"));
            files.Add(new ProjectFile("File2", FileType.None, "Project1"));
            _cache.Add(files);
            _cache.GetAllProjectFiles().Length.ShouldEqual(2);
        }

        [Test]
        public void Should_invalidate_project_files()
        {
            var files = new List<ProjectFile>();
            files.Add(new ProjectFile("File1", FileType.Resource, "Project1"));
            files.Add(new ProjectFile("File2", FileType.None, "Project1"));
            _cache.Add(files);
            _cache.InvalidateProjectFiles("Project1");
            _cache.GetAllProjectFiles().Length.ShouldEqual(0);
        }
        
        [Test]
        public void Should_add_validate_project_file()
        {
            var files = new List<ProjectFile>();
            files.Add(new ProjectFile("File1", FileType.Resource, "Project1"));
            _cache.Add(files);
            _cache.IsProjectFile("File1").ShouldBeTrue();
            _cache.IsProjectFile("File2").ShouldBeFalse();
        }
    }
}
