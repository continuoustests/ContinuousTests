using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE80;
using EnvDTE;
using System.IO;
using AutoTest.Messages;

namespace AutoTest.VS.Util.Builds
{
    class OutputParser
    {
        private DTE2 _application;
        private CacheMessages _message = new CacheMessages();
        private List<string> _projects = new List<string>();
        private string _currentProject = null;
        private string _currentProjectID = null;
        private string _currentAssembly = null;
        private bool _currentWasCompiled = false;
        private List<CacheBuildMessage> _currentErrors = new List<CacheBuildMessage>();
        private List<CacheBuildMessage> _currentWarnings = new List<CacheBuildMessage>();
        private DateTime _timeOfLastOutputParsing;

        public OutputParser(DTE2 application, DateTime lastParse)
        {
            _application = application;
            _timeOfLastOutputParsing = lastParse;
        }

        public IEnumerable<string> GetProjects()
        {
            return _projects.GroupBy(x => x).Select(x => x.Key);
        }

        public CacheMessages Parse(string output)
        {
            var lines = output.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
                return null;

            foreach (var line in lines)
                parseLine(line);

            addPreviousProject();
            if (_projects.Count == 0)
                return null;
            return _message;
        }

        private bool containsLineStartingWith(string expression, string[] lines)
        {
            foreach (var line in lines)
                if (line.StartsWith(expression))
                    return true;
            return false;
        }

        private void parseLine(string line)
        {
            if (line.Contains("------ Build started: Project:"))
                getProject(line);
            if (line.StartsWith("Compile complete --"))
                _currentWasCompiled = true;
            if (line.StartsWith(string.Format("  {0} -> ", _currentProjectID)))
                _currentAssembly = getAssembly(line);
            string id = null;
            if (line.Contains(": error"))
                id = ": error";
            if (line.StartsWith("fatal error"))
                id = "fatal error";
            if (line.Contains(": warning"))
                id = ": warning";
            if (id == null)
                return;
            var fileRaw = line.Substring(0, line.IndexOf(id));
            string file = "";
            int lineNumber = 0;
            int column = 0;
            if (fileRaw.IndexOf("(") != -1)
            {
                file = fileRaw.Substring(0, fileRaw.IndexOf("("));
                var locationRaw = fileRaw.Substring(fileRaw.IndexOf("(") + 1, fileRaw.IndexOf(")") - (fileRaw.IndexOf("(") + 1));
                var location = locationRaw.Split(new char[] { ',' });
                lineNumber = int.Parse(location[0]);
                column = int.Parse(location[1]);
            }
            var message = line.Substring(line.IndexOf(id) + id.Length, line.Length - (line.IndexOf(id) + id.Length)).Trim();
            var cacheMsg = new CacheBuildMessage(_currentProject, new Messages.BuildMessage() { File = file, LineNumber = lineNumber, LinePosition = column, ErrorMessage = message });
            if (id == ": error")
            {
                if (_currentErrors.Count(x => x.Equals(cacheMsg)) == 0)
                    _currentErrors.Add(cacheMsg);
            }
            else
            {
                if (_currentWarnings.Count(x => x.Equals(cacheMsg)) == 0)
                    _currentWarnings.Add(cacheMsg);
            }
        }

        private string getAssembly(string line)
        {
            var start = string.Format("  {0} -> ", _currentProjectID).Length;
            return line.Substring(start, line.Length - start).Trim();
        }

        private void getProject(string line)
        {
            addPreviousProject();
            var start = "------ Build started: Project:";
            var end = ", Configuration";
            var projectName = line.Substring(line.IndexOf(start) + start.Length, line.IndexOf(end) - (line.IndexOf(start) + start.Length)).Trim();
            _currentProjectID = projectName;
            if (matchToProjects(projectName, _application.Solution.Projects))
                return;
            _currentProject = projectName;
        }

        private void addPreviousProject()
        {
            if (_currentProject != null)
            {
                if (_currentWasCompiled || assemblyWasUpdated())
                {
                    _currentErrors
                        .Where(x => _message.ErrorsToAdd.Count(y => y.Equals(x)) == 0).ToList()
                        .ForEach(x => _message.AddError(x));
                    _currentWarnings
                        .Where(x => _message.WarningsToAdd.Count(y => y.Equals(x)) == 0).ToList()
                        .ForEach(x => _message.AddWarning(x));
                    _projects.Add(_currentProject);
                }
                _currentWasCompiled = false;
            }
        }

        private bool assemblyWasUpdated()
        {
            return _currentAssembly != null && _timeOfLastOutputParsing < new FileInfo(_currentAssembly).LastWriteTime;
        }

        private bool matchToProjects(string name, Projects projects)
        {
            for (int i = 1; i <= projects.Count; i++)
                if (matchToProject(name, projects.Item(i)))
                    return true;
            return false;
        }

        private bool matchToProject(string name, Project project)
        {
            if (project.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
            {
                foreach (ProjectItem item in project.ProjectItems)
                    if (matchToProject(name, item.SubProject))
                        return true;
            }

            if (project.Name == name)
            {
                _currentProject = project.FullName;
                return true;
            }
            return false;
        }
    }
}
