using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;
using AutoTest.VS.Util.Builds;
using AutoTest.Messages;

namespace AutoTest.VS.Util.CommandHandling
{
    public class RunTestsForCodeModel : ICommandHandler
    {
        private readonly string _commandName;
        private readonly Func<bool> _isEnabled;
        private readonly Func<bool> _manualBuild;
        private readonly Action<List<OnDemandRun>> _runTests;
        private readonly Func<IEnumerable<OnDemandRun>, IEnumerable<string>> _getProjectsForBuild;
        private readonly DTE2 _applicationObject;
        private readonly IVSBuildRunner _buildRunner;
        private readonly Action<IEnumerable<OnDemandRun>> _peek;

        public RunTestsForCodeModel(string commandName, Func<bool> isEnabled, Func<bool> manualBuild, Action<List<OnDemandRun>> runTests, DTE2 applicationObject, IVSBuildRunner buildRunner, Func<IEnumerable<OnDemandRun>, IEnumerable<string>> getProjectsForBuild, Action<IEnumerable<OnDemandRun>> peek)
        {
            _commandName = commandName;
            _isEnabled = isEnabled;
            _manualBuild = manualBuild;
            _runTests = runTests;
            _applicationObject = applicationObject;
            _buildRunner = buildRunner;
            _getProjectsForBuild = getProjectsForBuild;
            _peek = peek;
        }

        public void Exec(EnvDTE.vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            var runs = new List<OnDemandRun>();
            foreach (SelectedItem selectedItem in _applicationObject.SelectedItems)
            {
                try
                {
                    if (selectedItem.Project != null)
                        addItemsFor(runs, selectedItem.Project);
                    if (selectedItem.ProjectItem != null)
                        addItemFor(runs, selectedItem.ProjectItem);
                }
                catch
                {
                }
            }
            try
            {
                _peek(runs);
                if (!_manualBuild() || _buildRunner.Build(_getProjectsForBuild(runs)))
                    _runTests(runs);
            }
            catch (Exception ex)
            {
            }
        }

        private void addItemFor(List<OnDemandRun> runs, ProjectItem projectItem)
        {
            if (projectItem.FileCodeModel != null)
            {
                addItemFor(runs, projectItem.FileCodeModel.CodeElements, projectItem.ContainingProject.FullName);
                return;
            }
            foreach (ProjectItem item in projectItem.ProjectItems)
                addItemFor(runs, item);
        }

        private void addItemFor(List<OnDemandRun> runs, CodeElements elements, string projectName)
        {
            var classes = new List<string>();
            foreach (CodeElement element in elements)
            {
                if (element.Kind == vsCMElement.vsCMElementNamespace)
                    addItemFor(runs, element.Children, projectName);
                if (element.Kind == vsCMElement.vsCMElementClass)
                    classes.Add(element.FullName);
            }
            if (classes.Count > 0)
                runs.Add(new OnDemandRun(projectName, new string[] { }, classes.ToArray(), new string[] { }));
        }

        private void addItemsFor(List<OnDemandRun> runs, Project project)
        {
            if (project.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
                addItemsForSolutionFolder(runs, project);
            else
                addItemFor(runs, project);
        }

        private void addItemFor(List<OnDemandRun> runs, Project project)
        {
            var run = new OnDemandRun(project.FullName);
            run.ShouldRunAllTestsInProject();
            runs.Add(run);
        }

        private void addItemsForSolutionFolder(List<OnDemandRun> runs, Project project)
        {
            foreach (ProjectItem item in project.ProjectItems)
                addItemsFor(runs, item.SubProject);
        }

        public void QueryStatus(EnvDTE.vsCommandStatusTextWanted NeededText, ref EnvDTE.vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = _isEnabled()
                               ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled
                               : vsCommandStatus.vsCommandStatusSupported;
        }

        public string Name
        {
            get { return _commandName; }
        }
    }
}
