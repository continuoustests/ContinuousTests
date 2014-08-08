using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Messaging;
using AutoTest.Core.Configuration;
using AutoTest.Core.Caching;
using AutoTest.Messages;
using AutoTest.Core.Caching.Projects;
using System.IO;

namespace AutoTest.VM.RealtimeChanges
{
    class RealtimeChangePreProcessor : IPreProcessBuildruns
    {
        private IMessageBus _bus;
        private IConfiguration _configuration;
        private ICache _cache;
        private bool _isActive = false;
        private TempFiles _files = null;

        public RealtimeChangePreProcessor(IMessageBus bus, IConfiguration config, ICache cache)
        {
            _bus = bus;
            _configuration = config;
            _cache = cache;
        }

        public void Invoke(TempFiles files)
        {
            _files = files;
            var message = getProjects(files);
            _isActive = true;
            _bus.Publish(message);
        }

        private ProjectChangeMessage getProjects(TempFiles files)
        {
            var message = new ProjectChangeMessage();
            foreach (var file in files.Files)
            {
                var project = _cache.Get<Project>(file.Original);
                if (project == null)
                    continue;
                message.AddFile(new ChangedFile(project.Key));
            }
            return message;
        }

        public RunInfo[] PreProcess(RunInfo[] details)
        {
            if (_isActive && _files != null)
            {
                _configuration.OverrideSolution(_files.Solution.Tempfile);
                details
                    .Where(x => x.Project != null).ToList()
                    .ForEach(x =>
                        {
                            Logger.WriteDebug("Checking for temp projects for " + x.Project.Key);
                            var temp = _files.Files.FirstOrDefault(y => y.Original.Equals(x.Project.Key));
                            if (temp != null)
                            {
                                Logger.WriteDebug("\tFound " + temp.Tempfile);
                                x.BuildTemporaryProject(temp.Tempfile);
                            }
                        });
            }
            return details;
        }

        public BuildRunResults PostProcessBuildResults(BuildRunResults buildResults)
        {
            if (!_isActive)
                return buildResults;
            if (buildResults == null)
                return null;

            //rewritePdbs(buildResults);

            updateBuildProject(ref buildResults);

            foreach (var x in buildResults.Errors)
            {
                updateMessage(x);
            }
            foreach (var x in buildResults.Warnings)
            {
                updateMessage(x);
            }
            return buildResults;
        }

        private void updateMessage(BuildMessage x)
        {
            foreach (var file in _files.Files)
            {
                if (compare(file.Tempfile, x.File))
                    x.UpdateFile(file.Original);
                x.ErrorMessage = x.ErrorMessage.Replace(file.Tempfile, file.Original);
            }

        }

        private static bool compare(string tempfile, string resultoutput)
        {
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
                return tempfile.Equals(resultoutput);
            else
                return tempfile.ToLower().Equals(resultoutput.ToLower());
        }

        public RunInfo[] PostProcess(RunInfo[] details, ref RunReport runReport)
        {
            _isActive = false;
            _configuration.ResetSolution();
            if (_files == null)
                return details;
            updateReport(ref runReport);
            if (File.Exists(_files.Solution.Tempfile))
                File.Delete(_files.Solution.Tempfile);
            _files.Files.ToList()
                .ForEach(x =>
                {
                    if (File.Exists(x.Tempfile))
                        File.Delete(x.Tempfile);
                });
            _files = null;
            return details;
        }

        private void updateReport(ref RunReport runReport)
        {
            foreach (var x in runReport.RunActions)
            {
                if (compare(x.Project, _files.Solution.Tempfile))
                    x.UpdateProject(_files.Solution.Original);
                else
                {
                    var project = _files.Files.FirstOrDefault(f => compare(f.Tempfile, x.Project));
                    if (project != null)
                        x.UpdateProject(project.Original);
                }
            }
        }

        private void updateBuildProject(ref BuildRunResults buildResults)
        {
            if (compare(buildResults.Project, _files.Solution.Tempfile))
                buildResults.UpdateProject(_files.Solution.Original);
            else
            {
                var buildProject = buildResults.Project;
                var project = _files.Files.FirstOrDefault(f => compare(f.Tempfile, buildProject));
                if (project != null)
                    buildResults.UpdateProject(project.Original);
            }
        }
    }
}