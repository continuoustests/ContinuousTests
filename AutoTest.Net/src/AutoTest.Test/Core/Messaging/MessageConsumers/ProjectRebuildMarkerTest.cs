using System;
using NUnit.Framework;
using AutoTest.Messages;
using Rhino.Mocks;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using System.IO;
using AutoTest.Core;
using AutoTest.Core.Configuration;
namespace AutoTest.Test
{
	[TestFixture]
	public class ProjectRebuildMarkerTest
	{
        [Test]
        public void Should_add_projects_that_doesnt_exist()
        {
            var project = new Project("", new ProjectDocument(ProjectType.VisualBasic));
            var cache = MockRepository.GenerateMock<ICache>();
            var config = MockRepository.GenerateMock<IConfiguration>();
            var file = new ChangedFile(string.Format("TestResources{0}VS2008{0}NUnitTestProjectVisualBasic.vbproj", Path.DirectorySeparatorChar));
            cache.Stub(c => c.Get<Project>(file.FullName)).Return(null).Repeat.Once();
            cache.Stub(c => c.Get<Project>(file.FullName)).Return(project).Repeat.Once();

            var marker = new ProjectRebuildMarker(cache, config);
            marker.HandleProjects(file);

            cache.AssertWasCalled(c => c.Add<Project>(file.FullName));
        }

        [Test]
        public void Should_never_handle_realtime_tests()
        {
            var project = new Project("", new ProjectDocument(ProjectType.VisualBasic));
            var cache = MockRepository.GenerateMock<ICache>();
            var config = MockRepository.GenerateMock<IConfiguration>();
            var file = new ChangedFile(string.Format("TestResources{0}VS2008{0}_rltm_build_fl_Bleh.csproj", Path.DirectorySeparatorChar));
            cache.Stub(c => c.Get<Project>(file.FullName)).Return(null).Repeat.Once();
            cache.Stub(c => c.Get<Project>(file.FullName)).Return(project).Repeat.Once();

            var marker = new ProjectRebuildMarker(cache, config);
            marker.HandleProjects(file);

            cache.AssertWasNotCalled(c => c.Add<Project>(file.FullName));
        }
	}
}

