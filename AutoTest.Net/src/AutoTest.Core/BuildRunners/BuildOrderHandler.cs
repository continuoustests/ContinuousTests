using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging.MessageConsumers;

namespace AutoTest.Core.BuildRunners
{
    public interface IGenerateOrderedBuildLists
    {
        IEnumerable<string> Generate(IEnumerable<string> projects);
    }

    public class BuildOrderHandler : IGenerateOrderedBuildLists
    {
        private IGenerateBuildList _listGenerator;
        private IOptimizeBuildConfiguration _buildOptimizer;

        public BuildOrderHandler(IGenerateBuildList listGenerator, IOptimizeBuildConfiguration buildOptimizer)
        {
            _listGenerator = listGenerator;
            _buildOptimizer = buildOptimizer;
        }

        public IEnumerable<string> Generate(IEnumerable<string> projects)
        {
            var list = _listGenerator.Generate(projects.ToArray());
            var runInfos = _buildOptimizer.AssembleBuildConfiguration(list);
            return runInfos.Where(x => x.ShouldBeBuilt).Select(x => getProject(x));
        }

        private string getProject(RunInfo x)
        {
            if (x.TemporaryBuildProject != null)
                return x.TemporaryBuildProject;
            return x.Project.Key;
        }
    }
}
