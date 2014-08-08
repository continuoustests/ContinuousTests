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
    public class RunTestsForSolution : ICommandHandler
    {
        private readonly string _commandName;
        private readonly Func<bool> _isEnabled;
        private readonly Func<bool> _manualBuild;
        private readonly Action<List<OnDemandRun>> _runTests;
        private readonly DTE2 _applicationObject;
        private readonly IVSBuildRunner _buildRunner;
        private readonly Action<IEnumerable<OnDemandRun>> _peek;

        public RunTestsForSolution(string commandName, Func<bool> isEnabled, Func<bool> manualBuild, Action<List<OnDemandRun>> runTests, DTE2 applicationObject, IVSBuildRunner buildRunner, Action<IEnumerable<OnDemandRun>> peek)
        {
            _commandName = commandName;
            _isEnabled = isEnabled;
            _manualBuild = manualBuild;
            _runTests = runTests;
            _applicationObject = applicationObject;
            _buildRunner = buildRunner;
            _peek = peek;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            var runs = new List<OnDemandRun>();
            foreach (Project project in _applicationObject.Solution.Projects)
                addProjects(runs, project);
            _peek(runs);
            if (!_manualBuild() || _buildRunner.Build())
                _runTests(runs);
        }

        private void addProjects(List<OnDemandRun> runs, Project project)
        {
            if (project.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
            {
                foreach (ProjectItem item in project.ProjectItems)
                {
                    Project subProject = item.SubProject;
                    if (subProject == null)
                        continue;
                    addProjects(runs, subProject);
                }
                return;
            }
            add(runs, project);
        }

        private static void add(List<OnDemandRun> runs, Project project)
        {
            try
            {
                var run = new OnDemandRun(project.FullName);
                run.ShouldRunAllTestsInProject();
                runs.Add(run);
            }
            catch
            {
            }
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
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
