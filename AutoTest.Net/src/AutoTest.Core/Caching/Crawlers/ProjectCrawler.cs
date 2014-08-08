using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.FileSystem;
using System.IO;
using AutoTest.Core.Messaging;
using AutoTest.Messages;
using AutoTest.Core.DebugLog;

namespace AutoTest.Core.Caching.Crawlers
{
    public class ProjectCrawler
    {
        private const string CSHARP_PROJECT_EXTENTION = "csproj";
        private const string VB_PROJECT_EXTENTION = "vbproj";
        private const string FSHARP_PROJECT_EXTENTION = "fsproj";
        private ICache _cache;
        private IFileSystemService _fsService;
		private IMessageBus _bus;

        public ProjectCrawler(ICache cache, IFileSystemService fsService, IMessageBus bus)
        {
            _cache = cache;
            _fsService = fsService;
			_bus = bus;
        }

        public void Crawl(string path)
        {
            if (!_fsService.DirectoryExists(path))
                return;
            getCSharpProjects(path);
            getVisualBasicProjects(path);
            getFSharpProjects(path);
        }

        private void getCSharpProjects(string path)
        {
            var files = _fsService.GetFiles(
                path,
                string.Format("*.{0}", CSHARP_PROJECT_EXTENTION));
            addProjects(files);
        }

        private void getVisualBasicProjects(string path)
        {
            var files = _fsService.GetFiles(
                path, 
                string.Format("*.{0}", VB_PROJECT_EXTENTION));
            addProjects(files);
        }

        private void getFSharpProjects(string path)
        {
            var files = _fsService.GetFiles(
                path,
                string.Format("*.{0}", FSHARP_PROJECT_EXTENTION));
            addProjects(files);
        }

        private void addProjects(string[] files)
        {
            foreach (var file in files)
                tryAddProject(file);
        }
        
        private void tryAddProject(string project)
		{
			try
			{
				_cache.Add<Project>(Path.GetFullPath(project));
			}
			catch (Exception exception)
			{
				var messageString = string.Format("Failed parsing project {0}. Project will not be built. ({1})", project, exception.Message);
				var message = new InformationMessage(messageString);
				_bus.Publish<InformationMessage>(message);
			}
		}
	}
}
