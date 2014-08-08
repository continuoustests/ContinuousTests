using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners.Shared.Targeting
{
    public class TargetedRunAssembler
    {
        private RunOptions _options;
        private IAssemblyPropertyReader _locator;
        private bool _runAssembliesInParallel;
        private List<TargetedRun> _runs;

        public TargetedRunAssembler(RunOptions options, IAssemblyPropertyReader locator, bool runAssembliesInParallel)
        {
            _options = options;
            _locator = locator;
            _runAssembliesInParallel = runAssembliesInParallel;
        }

        public IEnumerable<TargetedRun> Assemble()
        {
            _runs = new List<TargetedRun>();
            foreach (var run in _options.TestRuns)
            {
                foreach (var assembly in run.Assemblies)
                    addAssembly(run.ID, run.Categories, assembly);
            }
            return _runs;
        }

        private void addAssembly(string runnerID, IEnumerable<string> categories, AssemblyOptions assemblyOptions)
        {
            var platform = getPlatform(assemblyOptions.Assembly);
            var targetFramework = getTargetFramework(assemblyOptions.Assembly);
            var targeted = _runs.Where(x => x.TargetFramework.Equals(targetFramework) && x.Platform.Equals(platform)).FirstOrDefault();
            if (targeted == null || _runAssembliesInParallel)
                targeted = addTarget(platform, targetFramework);
            targeted.AddRunner(runnerID, categories, assemblyOptions);
        }

        private TargetedRun addTarget(Platform platform, Version targetFramework)
        {
            _runs.Add(new TargetedRun(platform, targetFramework));
            return _runs[_runs.Count -1];
        }

        private Version getTargetFramework(string assembly)
        {
            return _locator.GetTargetFramework(assembly);
        }

        private Platform getPlatform(string assembly)
        {
            return _locator.GetPlatform(assembly);
        }
    }
}
