using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Core.Configuration
{
    public interface IConfiguration
    {
        string WatchPath { get; }
        string WatchToken { get; }
        string SolutionToBuild { get; }

        void OverrideSolution(string solution);
        void ResetSolution();

        string Providers { get; }
        bool StartPaused { get; }
        string[] WatchDirectores { get; }
        string BuildExecutable(ProjectDocument project);
        string NunitTestRunner(string version);
		string GetSpesificNunitTestRunner(string version);
        string MSTestRunner(string version);
        string XunitTestRunner(string version);
        string MSpecTestRunner(string version);
        CodeEditor CodeEditor { get; }
        bool DebuggingEnabled { get; }
		bool NotifyOnRunStarted { get; }
		bool NotifyOnRunCompleted { get; }
        string MSBuildAdditionalParameters { get; }
        int MSBuildParallelBuildCount { get; }
		string GrowlNotify { get; }
		string[] WatchIgnoreList { get; }
		bool ShouldUseBinaryChangeIgnoreLists { get; }
		int FileChangeBatchDelay { get; }
		string[] TestAssembliesToIgnore { get; }
		string[] TestCategoriesToIgnore { get; }
		string CustomOutputPath { get; }
		bool RerunFailedTestsFirst { get; }
        bool WhenWatchingSolutionBuildSolution { get; }
        bool UseAutoTestTestRunner { get; }
        bool UseLowestCommonDenominatorAsWatchPath { get; }
        bool WatchAllFiles { get; }
        bool RunAssembliesInParallel { get; }
        bool TestRunnerCompatibilityMode { get; }
		long LogRecycleSize { get; }
        string[] ProjectsToIgnore { get; }

        string IgnoreFile { get; }

        bool ShouldBuildSolution { get; }

        void ValidateSettings();
		void BuildIgnoreListFromPath(string watchPath);
		void SetBuildProvider();
        string AllSettings(string key);
		void AnnounceTrackerType();
		void Merge(string configuratoinFile);
        void BuildIgnoreList(string path);
        void SetCustomOutputPath(string path);

        void Reload(string localConfiguration);

        void SetWatchPath(string watchFolder);
        void SetWatchToken(string watchToken);

        void SetLoggerStateAction(Action<bool> setupLogging);
        void EnableLogging();
        void DisableLogging();
    }
}
