using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.VM.Messages;
using System.IO;

namespace AutoTest.VM.RealtimeChanges
{
    class SolutionAssembler
    {
        private IEnumerable<RealtimeChangeMessage> _messages;
        private string _solution;
        private string _solutionContent;

        public SolutionAssembler(IEnumerable<RealtimeChangeMessage> messages)
        {
            _messages = messages;
        }

        public TempFiles AssembleTo(string watchToken)
        {
            _solution = watchToken;
            _solutionContent = File.ReadAllText(watchToken);
            var projects = createTempFilesForProjects();

            var newSolution = TempFileFromFile(watchToken);
            File.WriteAllText(newSolution, _solutionContent);
            Logger.WriteDebug("Solution written " + newSolution);
            return new TempFiles(new TempFile(null, newSolution, _solution), projects);
        }

        private List<TempFile> createTempFilesForProjects()
        {
            var files = new List<TempFile>();
            var projects = _messages
                .GroupBy(x => x.Project)
                .Select(x => x.Key).ToList();
            _messages
                .GroupBy(x => x.Project).ToList()
                .ForEach(x => files.AddRange(createTempFilesForProject(x, projects)));
            return files;
        }

        private IEnumerable<TempFile> createTempFilesForProject(IGrouping<string, RealtimeChangeMessage> project, List<string> projects)
        {
            var files = new List<TempFile>();
            var file = TempFileFromFile(project.Key);
            files.Add(new TempFile(null, file, project.Key));
            var content = File.ReadAllText(project.Key);
            if (project.Count(x => x.File != null) > 0)
                project.ToList().ForEach(x => files.Add(updateContent(ref content, x)));
            projects.ForEach(x => files.Add(updateProjectReference(ref content, x)));
            File.WriteAllText(file, content);
            Logger.WriteDebug("Project written " + file);
            var relativePath = getRelativePath(_solution, project.Key);
            _solutionContent = _solutionContent.Replace("\"" + relativePath + "\"", "\"" + getRelativePath(_solution, file) + "\"");
            _solutionContent = _solutionContent.Replace("\"" + project.Key + "\"", "\"" + getRelativePath(_solution, file) + "\"");
            Logger.WriteDebug(string.Format("Replacing {0} or {1} with {2}", relativePath, project.Key, file));
            return files;
        }

        private TempFile updateProjectReference(ref string content, string x)
        {
            var tempName = getFileName(x);
            var project = Path.GetFileName(x);
            content = content.Replace("\\" + project + "\"", "\\" + tempName + "\"");
            content = content.Replace("\"" + project + "\"", "\"" + tempName + "\"");
            Logger.WriteDebug(string.Format("Replaced {0} with {1}", x, project));
            return new TempFile(null, Path.Combine(Path.GetDirectoryName(x), tempName), x);
        }

        private TempFile updateContent(ref string content, RealtimeChangeMessage x)
        {
            var file = Path.Combine(Path.GetTempPath(), getFileName(x.File));
            var relativePath = getRelativePath(x.Project, x.File);
            content = content.Replace("\"" + relativePath + "\"", "\"" + file + "\"");
            content = content.Replace("\"" + x.File + "\"", "\"" + file + "\"");
            Logger.WriteDebug(string.Format("Replacing {0} or {1} with {2}", relativePath, x.File, file));
            File.WriteAllText(file, x.Content);
            Logger.WriteDebug("File written " + file);
            return new TempFile(x.Project, file, x.File);
        }

        private static string getFileName(string path)
        {
            return "_rltm_build_fl_" + Path.GetFileName(path);
        }

        private string getRelativePath(string project, string file)
        {
            var projectDir = Path.GetDirectoryName(project) + Path.DirectorySeparatorChar;
            return file.Replace(projectDir, "");
        }

        public string TempFileFromFile(string file)
        {
            return Path.Combine(
                Path.GetDirectoryName(file),
                "_rltm_build_fl_" + Path.GetFileName(file));
        }
    }
}
