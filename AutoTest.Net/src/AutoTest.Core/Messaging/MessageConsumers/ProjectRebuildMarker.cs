using System;
using AutoTest.Messages;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.DebugLog;
using AutoTest.Core.Configuration;
using System.IO;
namespace AutoTest.Core
{
	public class ProjectRebuildMarker : IMarkProjectsForRebuild
	{
        private IConfiguration _configuration;
		private ICache _cache;

        public ProjectRebuildMarker(ICache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
		}
		
		public void HandleProjects(ChangedFile file)
		{
            try
            {
                if (file.FullName.Contains("_rltm_build_fl_"))
                    return;
                handleProject(".csproj", file);
                handleProject(".vbproj", file);
                handleProject(".fsproj", file);
            }
            catch (Exception ex)
            {
                Debug.WriteException(ex);
            }
		}
		
		private bool handleProject(string extension, ChangedFile file)
		{
            var isWatchingSolution = File.Exists(_configuration.WatchToken);
			if (file.Extension.ToLower().Equals(extension))
			{
				var project = _cache.Get<Project>(file.FullName);
                if (project == null)
                {
                    if (!isWatchingSolution)
                    {
                        Debug.WriteDebug("Adding and marking project for rebuild " + file.FullName);
                        _cache.Add<Project>(file.FullName);
                        project = _cache.Get<Project>(file.FullName);
                    }
                }
                else
                {
                    Debug.WriteDebug("Reloading and marking project for rebuild " + file.FullName);
                    _cache.Reload<Project>(file.FullName);
                    project = _cache.Get<Project>(file.FullName);
                }
				return true;
			}
			return false;
		}
	}
}

