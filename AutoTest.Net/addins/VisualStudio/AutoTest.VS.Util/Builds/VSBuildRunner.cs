using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE80;
using System.IO;
using AutoTest.Messages;
using AutoTest.VS.Util.DTEHacks;

namespace AutoTest.VS.Util.Builds
{
    public class VSBuildRunner : IVSBuildRunner
    {
        private DTE2 _application;
        private Func<bool> _runBuilds;
        private DateTime _timeOfLastOutputParsing = DateTime.MinValue;
        private Action<string> _setOutputPath;
        private Action<object> _notify;
        private Action<string> _clear;

        private static string _configuration = "";

        public VSBuildRunner(DTE2 application, Func<bool> runBuilds, Action<string> setOutputPath, Action<object> notify, Action<string> clearBuildItemsForProject)
        {
            _application = application;
            _runBuilds = runBuilds;
            _setOutputPath = setOutputPath;
            _notify = notify;
            _clear = clearBuildItemsForProject;
        }

        public bool Build()
        {
            if (!SolutionStateHandler.IsDirty)
                return true;
            prepareOutputPath();
            _notify(new RunStartedMessage(new[] { new ChangedFile(Path.GetFileName(_application.Solution.FullName)) }));
            _application.Solution.SolutionBuild.Build(true);
            var state = _application.Solution.SolutionBuild.LastBuildInfo == 0;
            if (state)
                SolutionStateHandler.Reset();

            return state;
        }

        public bool Build(IEnumerable<string> projects)
        {
            bool state = true;
            if (!SolutionStateHandler.IsDirty)
                return true;
            prepareOutputPath();
            foreach (var project in projects)
            {
                if (project == null)
                    continue;
                _application.Solution.SolutionBuild.BuildProject(
                    getProjectConfiguration(project),
                    project,
                    true);
                state = _application.Solution.SolutionBuild.LastBuildInfo == 0;
                if (!state)
                    break;
            }
            if (state)
                SolutionStateHandler.Reset();
            return state;
        }

        private string getProjectConfiguration(string project)
        {
            try
            {
                foreach (EnvDTE.Project prj in ProjectHandling.GetAll(_application))
                {
                    if (prj.FullName.Equals(project))
                        return prj.ConfigurationManager.ActiveConfiguration.ConfigurationName;
                }
            }
            catch (Exception ex)
            {
            }
            return _application.Solution.SolutionBuild.ActiveConfiguration.Name;
        }

        private void prepareOutputPath()
        {
            if (_application.Solution.SolutionBuild.ActiveConfiguration == null)
                return;
            var path = Path.Combine("bin", _application.Solution.SolutionBuild.ActiveConfiguration.Name);
            if (path.Equals(_configuration))
                return;
            _configuration = path;
            _setOutputPath(_configuration);
        }

        public void PusblishBuildErrors()
        {
            if (_application.Solution.SolutionBuild.LastBuildInfo > 0)
            {
                extractBuildOutput(false);
                var report = new Messages.RunReport();
                report.AddBuild(_application.Solution.FullName, new TimeSpan(), false);
                _notify(new RunFinishedMessage(report));
            }
            else
            {
                extractBuildOutput(true);
                if (_runBuilds())
                {
                    var report = new Messages.RunReport();
                    report.AddBuild(_application.Solution.FullName, new TimeSpan(), true);
                    _notify(new RunFinishedMessage(report));
                }
            }
        }

        private void extractBuildOutput(bool success)
        {
            try
            {
                for (int i = 1; i <= _application.ToolWindows.OutputWindow.OutputWindowPanes.Count; i++)
                {
                    var pane = _application.ToolWindows.OutputWindow.OutputWindowPanes.Item(i);
                    if (pane.Name == "Build")
                    {
                        pane.TextDocument.Selection.SelectAll();
                        var context = pane.TextDocument.Selection.Text;
                        pane.TextDocument.Selection.EndOfDocument();

                        var parser = new OutputParser(_application, _timeOfLastOutputParsing);
                        var message = parser.Parse(context);
                        if (message != null && (((message.ErrorsToAdd.Length > 0 || message.WarningsToAdd.Length > 0) && !success) || success))
                        {
                            var projects = parser.GetProjects();
                            foreach (var project in projects)
                                _clear(project);
                            _notify(message);
                        }
                        break;
                    }
                }
            }
            catch
            {
            }
            _timeOfLastOutputParsing = DateTime.Now;
        }

        private string getProject(ErrorItem errorItem)
        {
            try
            {
                return Path.Combine(Path.GetDirectoryName(_application.Solution.FullName), errorItem.Project);
            }
            catch
            {
                return errorItem.Project;
            }
        }

        private Messages.BuildMessage getBuildMessage(ErrorItem errorItem)
        {
            try
            {
                return new Messages.BuildMessage()
                            {
                                ErrorMessage = errorItem.Description,
                                File = errorItem.FileName,
                                LineNumber = errorItem.Line,
                                LinePosition = errorItem.Column
                            };
            }
            catch
            {
                return null;
            }
        }

        private int getColumn(ErrorItem errorItem)
        {
            try
            {
                return errorItem.Column;
            }
            catch
            {
                return 0;
            }
        }

        private int getLine(ErrorItem errorItem)
        {
            try
            {
                return errorItem.Line;
            }
            catch
            {
                return 0;
            }
        }

        private void goToErrors()
        {
            try
            {
                var window = (Window2) _application.ToolWindows.ErrorList.Parent;
                window.Activate();
            }
            catch (Exception ex)
            {
                AutoTest.Core.DebugLog.Debug.WriteException(ex);
            }
        }
    }
}
