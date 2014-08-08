using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Messaging;
using AutoTest.Messages;
using AutoTest.Core.DebugLog;
using System.IO;
using System.Text.RegularExpressions;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Core.Caching.Crawlers
{
    public class SolutionCrawler : ISolutionParser
    {
        private IFileSystemService _fsService;
        private IMessageBus _bus;
        private ICache _cache;

        public SolutionCrawler(IFileSystemService fsService, IMessageBus bus, ICache cache)
        {
            _fsService = fsService;
            _bus = bus;
            _cache = cache;
        }

        public void Crawl(string solutionFile)
        {
            if (!validateFile(solutionFile))
                return;
            var content = _fsService.ReadFileAsText(solutionFile);
            addProjects(content, solutionFile);
        }

        private void addProjects(string content, string solutionFile)
        {
            var matches = getProjectChunks(content);
            foreach (var projectChunk in matches)
            {
                var chunks = projectChunk.Split(new char[] { ',' });
                if (chunks.Length != 3)
                    continue;
                var path = chunks[1].Replace('\\', Path.DirectorySeparatorChar).Replace("\"", "").Trim();
                var project = Path.Combine(Path.GetDirectoryName(solutionFile), path);
                if (_fsService.DirectoryExists(project))
                    continue;
                if (!_fsService.FileExists(project))
                {
                    Debug.WriteDebug("Could not find project {0}", project);
                    continue;
                }
                tryAddProject(project);
            }
        }

        private string[] getProjectChunks(string content)
        {
            var projectChunks = new List<string>();
            int offset = 0;
            while (true)
            {
                var start = getStart(content, offset);
                if (start == -1)
                    break;
                offset = start;
                var end = getEnd(content, offset);
                if (end == -1)
                    break;
                offset = end;
                projectChunks.Add(content.Substring(start, end - start));
            }
            return projectChunks.ToArray();
        }

        private int getStart(string content, int offset)
        {
            var start = content.IndexOf("Project(", offset);
            if (start == -1)
                return -1;
            return start + "Project(".Length;
        }

        private int getEnd(string content, int offset)
        {
            var end = content.IndexOf("EndProject", offset);
            if (end == -1)
                return -1;
            return end;
        }

        private void tryAddProject(string project)
        {
            try
            {
                if (!_cache.Exists(project))
                    _cache.Add<Project>(Path.GetFullPath(project));
            }
            catch (Exception exception)
            {
                var messageString = string.Format("Failed parsing project {0}. Project will not be built. ({1})", project, exception.Message);
                var message = new InformationMessage(messageString);
                _bus.Publish<InformationMessage>(message);
            }
        }

        private bool validateFile(string solutionFile)
        {
            if (_fsService.FileExists(solutionFile))
                return true;
            var message = string.Format("{0} is not a valid solution file", solutionFile);
            _bus.Publish(new InformationMessage(message));
            Debug.WriteInfo(message);
            return false;
        }
    }
}
