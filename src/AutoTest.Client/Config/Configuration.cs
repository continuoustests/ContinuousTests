using System;
using System.Collections.Generic;

namespace AutoTest.Client.Config
{
    public class Configuration
    {
        public ConfigItem<string> GraphProvider { get; set; }
        public ConfigItem<bool> MinimizerDebug { get; set; }
        public List<ConfigItem<string>> MinimizerAssemblies { get; set; }
        public ConfigItem<string> MinimizerLevel { get; set; }
        public ConfigItem<string> ProfilerSetup { get; set; }
        public List<ConfigItem<string>> ProfilerNamespaces { get; set; }
        public ConfigItem<bool> IgnoreWarmup { get; set; }
        public ConfigItem<string> IgnoreThisUpgrade { get; set; }
        public ConfigItem<bool> RealtimeFeedback { get; set; }
        public ConfigItem<bool> AllDisabled { get; set; }

        public ConfigItem<bool> BuildErrorsInFeedbackWindow { get; set; }
        public ConfigItem<bool> BuildWarningsInFeedbackWindow { get; set; }
        public ConfigItem<bool> FailingTestsInFeedbackWindow { get; set; }
        public ConfigItem<bool> IgnoredTestsInFeedbackWindow { get; set; }
        public ConfigItem<bool> StartPaused { get; set; }

        public string ConfigurationFullpath { get; set; }
        public string SolutionPath { get; set; }

        public List<ConfigItem<VersionConfig>> MSBuild { get; set; }
        public ConfigItem<bool> BuildSolution { get; set; }

        public List<ConfigItem<VersionConfig>> NUnit { get; set; }
        public List<ConfigItem<VersionConfig>> MSTest { get; set; }
        public List<ConfigItem<VersionConfig>> XUnit { get; set; }

        public ConfigItem<IgnoreFile> IgnoreFile { get; set; }

        public List<ConfigItem<string>> IgnoredAssemblies { get; set; }
        public List<ConfigItem<string>> IgnoredCategories { get; set; }

        public ConfigItem<string> ChangeDelay { get; set; }

        public ConfigItem<string> BuildOutputPath { get; set; }
        public ConfigItem<string> GrowlNotifyPath { get; set; }

        public ConfigItem<bool> NotifyOnStartup { get; set; }
        public ConfigItem<bool> NotifyOnFinish { get; set; }

        public ConfigItem<bool> Debug { get; set; }

        public ConfigItem<bool> OverlayNotifications { get ; set ; }
        public ConfigItem<bool> CatMode { get; set; }
        public ConfigItem<bool> RiscEnabled { get; set; }
        public ConfigItem<bool> AnonFeedback { get; set; }

        public ConfigItem<bool> RunAssembliesInParallel { get; set; }
        public ConfigItem<bool> RunTestsInCompatibilityMode { get; set; }
        public ConfigItem<int> MSBuildParallelBuildCount { get; set; }

        public Configuration()
        {
            GraphProvider = new ConfigItem<string>();
            MinimizerDebug = new ConfigItem<bool>();
            MinimizerAssemblies = new List<ConfigItem<string>>();
            MinimizerLevel = new ConfigItem<string>();
            ProfilerSetup = new ConfigItem<string>();
            ProfilerNamespaces = new List<ConfigItem<string>>();
            IgnoreWarmup = new ConfigItem<bool>();
            IgnoreThisUpgrade = new ConfigItem<string>();
            RealtimeFeedback = new ConfigItem<bool>();
            AllDisabled = new ConfigItem<bool>();
            AnonFeedback = new ConfigItem<bool>();

            BuildErrorsInFeedbackWindow = new ConfigItem<bool>();
            BuildWarningsInFeedbackWindow = new ConfigItem<bool>();
            FailingTestsInFeedbackWindow = new ConfigItem<bool>();
            IgnoredTestsInFeedbackWindow = new ConfigItem<bool>();

            StartPaused = new ConfigItem<bool>();

            MSBuild = new List<ConfigItem<VersionConfig>>();
            BuildSolution = new ConfigItem<bool>();

            NUnit = new List<ConfigItem<VersionConfig>>();
            MSTest = new List<ConfigItem<VersionConfig>>();
            XUnit = new List<ConfigItem<VersionConfig>>();

            IgnoreFile = new ConfigItem<IgnoreFile> { Item = new IgnoreFile() };

            IgnoredAssemblies = new List<ConfigItem<string>>();
            IgnoredCategories = new List<ConfigItem<string>>();

            ChangeDelay = new ConfigItem<string>();

            BuildOutputPath = new ConfigItem<string>();
            GrowlNotifyPath = new ConfigItem<string>();

            NotifyOnStartup = new ConfigItem<bool>();
            NotifyOnFinish = new ConfigItem<bool>();
            Debug = new ConfigItem<bool>();
            CatMode = new ConfigItem<bool>();
            OverlayNotifications = new ConfigItem<bool>();
            RunAssembliesInParallel = new ConfigItem<bool>();
            RunTestsInCompatibilityMode = new ConfigItem<bool>();
            RiscEnabled = new ConfigItem<bool>();
            MSBuildParallelBuildCount = new ConfigItem<int>();
        }
    }
}
