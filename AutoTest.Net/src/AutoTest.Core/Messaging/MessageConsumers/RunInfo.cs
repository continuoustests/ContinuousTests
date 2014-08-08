using System;
using System.Linq;
using AutoTest.Core.Caching.Projects;
using System.Collections.Generic;
using AutoTest.Messages;
namespace AutoTest.Core.Messaging.MessageConsumers
{
    public class TestToRun
    {
        public TestRunner Runner { get; private set; }
        public string Test { get; private set; }

        public TestToRun(TestRunner runner, string test)
        {
            Runner = runner;
            Test = test;
        }
    }

	public class RunInfo : TestRunInfo
	{
        public string TemporaryBuildProject { get; private set; }
		public bool ShouldBeBuilt { get; private set; }
		public bool IsChanged { get; private set; }
		
		public RunInfo(Project project)
            : base(project, null)
		{
            TemporaryBuildProject = null;
			ShouldBeBuilt = false;
		}

        public void BuildTemporaryProject(string project)
        {
            TemporaryBuildProject = project;
        }

		public void ShouldBuild()
		{
			ShouldBeBuilt = true;
		}

        public void ShouldNotBuild()
        {
            ShouldBeBuilt = false;
        }

		public void Changed()
		{
			IsChanged = true;
		}

        public RunInfo Clone()
        {
            var runInfo = new RunInfo(Project);
            runInfo.SetAssembly(Assembly);
            BuildTemporaryProject(TemporaryBuildProject);
            if (ShouldBeBuilt)
                runInfo.ShouldBuild();
            if (IsChanged)
                runInfo.Changed();
            runInfo.AddTestsToRun(GetTests());
            runInfo.AddMembersToRun(GetMembers());
            runInfo.AddNamespacesToRun(GetNamespaces());
            foreach (var runner in _onlyRunTestsFor)
                runInfo.ShouldOnlyRunSpcifiedTestsFor(runner);
            foreach (var runner in _rerunAllWhenFinishedFor)
                runInfo.ShouldRerunAllTestWhenFinishedFor(runner);
            return runInfo;
        }

        public TestRunInfo CloneToTestRunInfo()
        {
            return this.Clone();
        }
	}
}

