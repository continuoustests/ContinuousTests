using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using AutoTest.Messages;
using AutoTest.Core.DebugLog;
using AutoTest.Core.Messaging;
using AutoTest.Core.Configuration;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Caching;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.Core.Caching.RunResultCache;

namespace AutoTest.Core.BuildRunners
{
    public interface IBuildSessionRunner
    {
        bool Build(string[] originalProjects, RunInfo[] projectList, RunReport runReport, Func<bool> exit);
    }

    public class BuildSessionRunner : IBuildSessionRunner
    {
        private ICache _cache;
        private IRunResultCache _runCache;
        private BuildConfiguration _buildConfig;
        private IMessageBus _bus;
        private IConfiguration _configuration;
        private IBuildRunner _buildRunner;
        private IPreProcessBuildruns[] _preBuildProcessors;
        private IFileSystemService _fs;
        private Func<bool> _exit;

        public BuildSessionRunner(BuildConfiguration buildConfig, ICache cache, IMessageBus bus, IConfiguration config, IBuildRunner buildRunner, IPreProcessBuildruns[] buildPreProcessors, IFileSystemService fs, IRunResultCache runCache)
        {
            _cache = cache;
            _runCache = runCache;
            _buildConfig = buildConfig;
            _fs = fs;
            _bus = bus;
            _configuration = config;
            _buildRunner = buildRunner;
            _preBuildProcessors = buildPreProcessors;
        }

        public bool Build(string[] originalProjects, RunInfo[] projectList, RunReport runReport, Func<bool> exit)
        {
            _exit = exit;
            projectList = preProcessBuildRun(projectList);
            if (projectList.Where(x => x.ShouldBeBuilt).Select(x => x).Count() == 0)
                return true;

            Debug.WriteInfo("Running builds");
            BuildRunResults results = null;
            if (_configuration.ShouldBuildSolution)
                results = buildSolution(projectList, runReport);
            else
                results = buildProjects(originalProjects, projectList, runReport);
            postProcessBuildRuns(projectList, ref runReport);
            return results == null;
        }

        private RunInfo[] preProcessBuildRun(RunInfo[] runInfos)
        {
            foreach (var preProcessor in _preBuildProcessors)
                runInfos = preProcessor.PreProcess(runInfos);
            return runInfos;
        }

        private BuildRunResults postProcessBuildReports(BuildRunResults report)
        {
            foreach (var preProcessor in _preBuildProcessors)
                report = preProcessor.PostProcessBuildResults(report);
            return report;
        }

        private RunInfo[] postProcessBuildRuns(RunInfo[] runInfos, ref RunReport runReport)
        {
            foreach (var preProcessor in _preBuildProcessors)
                runInfos = preProcessor.PostProcess(runInfos, ref runReport);
            return runInfos;
        }

        private BuildRunResults buildProjects(string[] changedProjects, RunInfo[] projectList, RunReport runReport)
        {
            if (_buildConfig.OptimisticBuildStrategy != null)
            {
                try {
                    return optimisticBuild(changedProjects, projectList, runReport);
                } catch (Exception ex) {
                    Debug.WriteException(ex);
                }
            }
            return buildProjects(projectList, runReport);
        }

        private BuildRunResults optimisticBuild(string[] changedProjects, RunInfo[] projectList, RunReport runReport)
        {
            var projectWithIssues = new List<string>();
            projectWithIssues.AddRange(_runCache
                .Errors
                .GroupBy(x => x.Key)
                .Select(x => x.Key));
            var builtProjects = new List<RunInfo>();
            var indirectlyBuilt = new List<string>();
            foreach (var file in projectList)
            {
                if (changedProjects.Contains(file.Project.Key) || projectWithIssues.Contains(file.Project.Key))
                {
                    Debug.WriteDebug("Optimistic build for project {0}", file.Project.Key);
                    var original = file.Assembly + ".original" + Path.GetExtension(file.Assembly);
                    _fs.CopyFile(file.Assembly, original);
                    var report = build(file, runReport);
                    var optimisticAdviced = _buildConfig.OptimisticBuildStrategy(file.Assembly, original);
                    _fs.DeleteFile(original);
                    if (!optimisticAdviced)
                        throw new Exception("Optimistic build is not adviced for this scenario");
                    if (report != null)
                        return report;
                    builtProjects.Add(file);
                }
                else
                {
                    Debug.WriteDebug("Not set to optimisticly build project {0}", file.Project.Key);
                    indirectlyBuilt.Add(file.Project.Key);
                }
            }
            builtProjects.ForEach(x => copyAssembly(x.Project.Key, x.Assembly));
            foreach (var project in indirectlyBuilt)
                runReport.AddBuild(project, new TimeSpan(0), true);
            return null;
        }

        private void copyAssembly(string key, string source)
        {
            var project = _cache.Get<Project>(key);
            if (project == null || project.Value == null)
                return;
            var destination =
                Path.Combine(
                    Path.GetDirectoryName(project.GetAssembly(_configuration.CustomOutputPath)),
                    Path.GetFileName(source));
            if (source != destination)
            {
                retryCopy(source, destination, 10);
                retryCopy(extension(source, ".pdb"), extension(destination, ".pdb"), 10);
                retryCopy(extension(source, ".mdb"), extension(destination, ".mdb"), 10);
            }
            foreach (var reference in project.Value.ReferencedBy)
                copyAssembly(reference, source);
        }

        private string extension(string source, string extension)
        {
            var dir = Path.GetDirectoryName(source);
            var filename = Path.GetFileNameWithoutExtension(source);
            return Path.Combine(dir, filename + extension);
        }

        private void retryCopy(string source, string destination, int retries)
        {
            if (!_fs.FileExists(source))
                return;
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    _fs.CopyFile(source, destination);
                    return;
                }
                catch (Exception ex)
                {
                    Debug.WriteDebug("Failed to copy file {0} to {1}", source, destination);
                    Debug.WriteException(ex);
                    Thread.Sleep(100);
                }
            }
            throw new Exception("Failed to copy optimistic assemblies");
        }

        private BuildRunResults buildProjects(RunInfo[] projectList, RunReport runReport)
        {
            var indirectlyBuilt = new List<string>();
            foreach (var file in projectList)
            {
                if (file.ShouldBeBuilt)
                {
                    Debug.WriteDebug("Set to build project {0}", file.Project.Key);
                    var report = build(file, runReport);
                    if (report != null)
                        return report;
                }
                else
                {
                    Debug.WriteDebug("Not set to build project {0}", file.Project.Key);
                    indirectlyBuilt.Add(file.Project.Key);
                }
            }
            foreach (var project in indirectlyBuilt)
                runReport.AddBuild(project, new TimeSpan(0), true);
            return null;
        }

        private BuildRunResults buildSolution(RunInfo[] projectList, RunReport runReport)
        {
            var buildExecutable = _configuration.BuildExecutable(new ProjectDocument(ProjectType.None));
            if (File.Exists(buildExecutable))
            {
                _bus.Publish(new RunInformationMessage(InformationType.Build, _configuration.SolutionToBuild, "", typeof(MSBuildRunner)));

                var buildReport = _buildRunner.RunBuild(_configuration.SolutionToBuild, projectList.Where(p => p.Project.Value.RequiresRebuild).Count() > 0, buildExecutable, () => { return _exit(); });
                buildReport = postProcessBuildReports(buildReport);
                var succeeded = buildReport.ErrorCount == 0;
                runReport.AddBuild(_configuration.WatchToken, buildReport.TimeSpent, succeeded);
                _bus.Publish(new BuildRunMessage(buildReport));
                if (succeeded)
                    return null;
                else
                    return buildReport;
            }
            return null;
        }

        private BuildRunResults build(RunInfo info, RunReport runReport)
        {
            if (File.Exists(_configuration.BuildExecutable(info.Project.Value)))
            {
                notifyAboutBuild(info);
                return buildProject(info, runReport);
            }

            return null;
        }

        private void notifyAboutBuild(RunInfo info)
        {
            _bus.Publish(new RunInformationMessage(
                                 InformationType.Build,
                                 info.Project.Key,
                                 info.Assembly,
                                 typeof(MSBuildRunner)));
        }

        private BuildRunResults buildProject(RunInfo info, RunReport report)
        {
            var project = info.Project;
            var buildReport = _buildRunner.RunBuild(info, _configuration.BuildExecutable(project.Value), () => { return _exit(); });
            buildReport = postProcessBuildReports(buildReport);
            var succeeded = buildReport.ErrorCount == 0;
            report.AddBuild(buildReport.Project, buildReport.TimeSpent, succeeded);
            _bus.Publish(new BuildRunMessage(buildReport));
            if (succeeded)
                return null;
            else
                return buildReport;
        }
    }
}
