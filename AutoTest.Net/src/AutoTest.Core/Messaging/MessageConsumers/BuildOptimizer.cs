using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.DebugLog;
using System.IO;
using AutoTest.Core.Configuration;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	public class BuildOptimizer : IOptimizeBuildConfiguration
	{
		private ICache _cache;
		private IConfiguration _configuration;
		
		public BuildOptimizer(ICache cache, IConfiguration configuration)
		{
			_cache = cache;
			_configuration = configuration;
		}

        public RunInfo[] AssembleBuildConfiguration(string[] projectList)
		{
            return AssembleBuildConfiguration(projectList, false);
		}

        public RunInfo[] AssembleBuildConfiguration(string[] projectList, bool skipMarkingForBuild)
        {
            var runList = getRunInfoList(projectList);
            return assemblefConfiguration(runList, false, skipMarkingForBuild);
        }

        public RunInfo[] AssembleBuildConfiguration(Project[] projectList)
        {
            return AssembleBuildConfiguration(projectList, false);
        }

        public RunInfo[] AssembleBuildConfiguration(Project[] projectList, bool skipMarkingForBuild)
        {
            var runList = getRunInfoList(projectList);
            return assemblefConfiguration(runList, false, skipMarkingForBuild);
        }

        private RunInfo[] assemblefConfiguration(List<RunInfo> runList, bool useBuiltProjectsOutputPath, bool skipMarkingForBuild)
        {
            if (!skipMarkingForBuild || useBuiltProjectsOutputPath)
            {
                markProjectsForBuild(runList);
                detectProjectRebuilds(runList);
            }
            if (useBuiltProjectsOutputPath)
                locateAssemblyDestinationsRecursive(runList);
            else
                locateAssemblyDestinations(runList);
            return runList.ToArray();
        }

        private List<RunInfo> getRunInfoList(Project[] projectList)
        {
            var runList = new List<RunInfo>();
            foreach (var project in projectList)
            {
                if (project.Value != null)
                    runList.Add(new RunInfo(project));
            }
            return runList;
        }
		
		private List<RunInfo> getRunInfoList(string[] projectList)
		{
			var runList = new List<RunInfo>();
            foreach (var project in projectList)
            {
                var projectItem = _cache.Get<Project>(project);
                if (projectItem.Value != null)
                    runList.Add(new RunInfo(projectItem));
            }
			return runList;
		}

        private void markProjectsForBuild(List<RunInfo> runList)
        {
            var shouldBuild = runList.Where<RunInfo>(r => r.Project.Value.ReferencedBy.Length.Equals(0)).ToArray();
            foreach (var item in shouldBuild)
                item.ShouldBuild();
        }

        private void locateAssemblyDestinations(List<RunInfo> runList)
        {
            for (int i = runList.Count - 1; i >= 0; i--)
            {
                var item = runList[i];
                item.SetAssembly(item.Project.GetAssembly(_configuration.CustomOutputPath));
            }
        }

        private void locateAssemblyDestinationsRecursive(List<RunInfo> runList)
        {
            for (int i = runList.Count - 1; i >= 0; i--)
            {
                var item = runList[i];
                if (!item.ShouldBeBuilt)
                    continue;
                item.SetAssembly(item.Project.GetAssembly(_configuration.CustomOutputPath));
                setAssemblyDestinationsRecursive(runList, item.Project, Path.GetDirectoryName(item.Assembly));
            }
        }

        private void setAssemblyDestinationsRecursive(List<RunInfo> runList, Project item, string assemblyPath)
        {
            var builtBy = runList.Where<RunInfo>(r => r.Project.Value.ReferencedBy.Contains(item.Key));
            foreach (var project in builtBy)
            {
                if (project.Assembly != null)
                    continue;
                project.SetAssembly(Path.Combine(assemblyPath, project.Project.Value.AssemblyName));
                setAssemblyDestinationsRecursive(runList, project.Project, assemblyPath);
            }
        }
		
		private void detectProjectRebuilds(List<RunInfo> runList)
		{
			var rebuilds = runList.Where<RunInfo>(r => r.Project.Value.RequiresRebuild);
			foreach (var info in rebuilds)
				markReferencedShouldBuildProjectsForRebuild(info, runList);
			
		}
		
		private void markReferencedShouldBuildProjectsForRebuild(RunInfo info, List<RunInfo> runList)
		{
			foreach (var reference in info.Project.Value.ReferencedBy)
			{
				var item = runList.Where(x => x.Project.Key.Equals(reference)).FirstOrDefault();
                if (item == null)
                {
                    DebugLog.Debug.WriteDebug("Could not find project for reference " + reference);
                    continue;
                }
				if (item.ShouldBeBuilt)
				{
					item.Project.Value.RebuildOnNextRun();
					continue;
				}
				markReferencedShouldBuildProjectsForRebuild(item, runList);
			}
		}
	}
}

