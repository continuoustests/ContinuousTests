using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.FileSystem;
using NUnit.Framework;
using System.IO;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Caching.Crawlers;
using AutoTest.Test.Core.Caching.Projects.Fakes;
using AutoTest.Core.Messaging;
using Rhino.Mocks;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Caching.Projects
{
    [TestFixture]
    public class ProjectCrawlerTest
    {
        private ProjectCrawler _crawler;
        private FakeCache _cache;
        private FakeFileSystemService _fsService;
		private IMessageBus _bus;

        [SetUp]
        public void setUp()
        {
			_bus = MockRepository.GenerateMock<IMessageBus>();
            _fsService = new FakeFileSystemService();
            _cache = new FakeCache();
            _crawler = new ProjectCrawler(_cache, _fsService, _bus);
        }

        [Test]
        public void Should_exit_if_invalid_folder()
        {
            _fsService.WhenValidatingDirectoryReturn(false);
            _crawler.Crawl("");
            _fsService.GetFilesWasNotCalled();
        }

        [Test]
        public void Should_Find_CSharp_Projects()
        {
            _fsService.WhenCrawlingFor("*.csproj").Return("AProject.csproj");
            _crawler.Crawl("");
            _cache.ShouldHaveBeenAdded(Path.GetFullPath("AProject.csproj"));
        }

        [Test]
        public void Should_Find_VisualBasic_Projects()
        {
            _fsService.WhenCrawlingFor("*.vbproj").Return("AProject.vbproj");
            _crawler.Crawl("");
            _cache.ShouldHaveBeenAdded(Path.GetFullPath("AProject.vbproj"));
        }

        [Test]
        public void Should_Find_FSharp_Projects()
        {
            _fsService.WhenCrawlingFor("*.fsproj").Return("AProject.fsproj");
            _crawler.Crawl("");
            _cache.ShouldHaveBeenAdded(Path.GetFullPath("AProject.fsproj"));
        }
		
		[Test]
		public void Should_log_information_message_if_add_project_fails()
		{
			_fsService.WhenCrawlingFor("*.csproj").Return("a_project.csproj");
			_cache.ShouldThrowErrorOnAdd();
			_crawler.Crawl("");
			
			_bus.AssertWasCalled(b => b.Publish<InformationMessage>(new InformationMessage("")), b => b.IgnoreArguments());
		}
    }
}
