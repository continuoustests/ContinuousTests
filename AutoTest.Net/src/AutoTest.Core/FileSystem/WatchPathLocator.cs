using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using System.IO;

namespace AutoTest.Core.FileSystem
{
    public class WatchPathLocator : AutoTest.Core.FileSystem.IWatchPathLocator
    {
        private ICache _cache;

        public WatchPathLocator(ICache cache)
        {
            _cache = cache;
        }

        public string Locate(string path)
        {
            var projects = _cache.GetAll<Project>();
            foreach (var project in projects)
                path = getLowestCommonDenominator(path, project.Key);
            return path;
        }

        private string getLowestCommonDenominator(string path, string project)
        {
            if (project == null)
                return path;
            if (project.Trim().Length == 0)
                return path;
            var projectPath = Path.GetDirectoryName(project);
            if (projectPath == null)
                return path;
            if (isRoot(projectPath))
                return path;
            if (projectPath.StartsWith(path))
                return path;
            if (path.StartsWith(projectPath))
                return projectPath;
            return getLowestCommonDenominator(path, projectPath);
        }

        private bool isRoot(string projectPath)
        {
            return Path.GetDirectoryName(projectPath) == null;
        }
    }
}
