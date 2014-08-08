using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutoTest.Core.DebugLog;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;
using System.Reflection;

namespace AutoTest.Core.BuildRunners
{
    class MSTestCrossPlatformPreProcessor : IPreProcessBuildruns
    {
        private List<string> _tmpProjects = new List<string>();

        public RunInfo[] PreProcess(RunInfo[] details)
        {
            var switcher = new MSTestSwitcharoo(Environment.OSVersion.Platform,
                                                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            details
                .Where(x => x.Project != null).ToList()
                .ForEach(x =>
                             {
                                 if (File.Exists(x.TemporaryBuildProject))
                                 {
                                     var project = File.ReadAllText(x.TemporaryBuildProject);
                                     if (switcher.IsGuyInCloset(project)) {
                                         File.WriteAllText(x.TemporaryBuildProject, switcher.PerformSwitch(project));
                                         Debug.WriteDebug("Switched to our mstest impl. for {0}", x.TemporaryBuildProject);
                                     }
                                 }
                                 else
                                 {
                                     if (File.Exists(x.Project.Key))
                                     {
                                         var project = File.ReadAllText(x.Project.Key);
                                         if (switcher.IsGuyInCloset(project))
                                         {
                                             var tmpProject = getTempProject(x.Project.Key);
                                             // We can do this because we know they are ordered in the right order
                                             _tmpProjects.ForEach(y =>
                                                                  project = project
                                                                                .Replace(
                                                                                    "\\" +
                                                                                    Path.GetFileName(
                                                                                        getOriginalProject(y)),
                                                                                    "\\" + Path.GetFileName(y))
                                                                                .Replace(
                                                                                    "\"" +
                                                                                    Path.GetFileName(
                                                                                        getOriginalProject(y)),
                                                                                    "\"" + Path.GetFileName(y)));
                                             File.WriteAllText(tmpProject, switcher.PerformSwitch(project));
                                             _tmpProjects.Add(tmpProject);
                                             x.BuildTemporaryProject(tmpProject);
                                             Debug.WriteDebug("Switched to our mstest impl. for {0}", x.Project.Key);
                                         }
                                     }
                                 }
                             });
            return details;
        }

        public BuildRunResults PostProcessBuildResults(BuildRunResults runResults)
        {
            var buildProject = runResults.Project;
            if (_tmpProjects.Contains(buildProject))
                runResults.UpdateProject(getOriginalProject(buildProject));
            return runResults;
        }

        public RunInfo[] PostProcess(RunInfo[] details, ref RunReport runReport)
        {
            _tmpProjects
                .ForEach(x =>
                             {
                                 if (File.Exists(x))
                                     File.Delete(x);
                             });
            _tmpProjects.Clear();
            return details;
        }

        private string getOriginalProject(string project)
        {
            return Path.Combine(Path.GetDirectoryName(project),
                Path.GetFileName(project).Replace("_rltm_build_fl_", ""));
        }
        
        private string getTempProject(string project)
        {
            return Path.Combine(Path.GetDirectoryName(project),
                "_rltm_build_fl_" + Path.GetFileName(project));
        }
    }
}
