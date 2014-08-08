using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Client.Config;
using AutoTest.VM.Messages.Configuration;
using System.IO;

namespace AutoTest.Client.UI
{
    public class ConfigurationFormWriter
    {
        private readonly IMSBuildLocator _locator;
        private readonly bool _isLocal;
        private readonly bool _atReadTimeIgnoreFileExisted;

        public Configuration Configuration { get; private set; }

        public ConfigurationFormWriter(bool isLocal, Configuration configuration, IMSBuildLocator locator)
        {
            _isLocal = isLocal;
            Configuration = configuration;
            _locator = locator;
            _atReadTimeIgnoreFileExisted = configuration.IgnoreFile.Exists && configuration.IgnoreFile.Item.File.Trim().Length > 0;
        }

        public void EnableMightyMoose()
        {
            var executable = _locator.GetBuildExecutable();
            if (executable != null)
            {
                Configuration.MSBuild.Clear();
                Configuration.MSBuild.Add(new ConfigItem<VersionConfig>()
                            {
                                Exists = true,
                                ShouldExclude = false,
                                ShouldMerge = false,
                                IsLocal = _isLocal,
                                Item = new VersionConfig() { Path = executable }
                            });
            }
            SetPausedAtStartup(false);
        }

        public void EnableAutoMoose()
        {
            Configuration.MSBuild.Clear();
            if (_isLocal)
                Configuration.MSBuild.Add(new ConfigItem<VersionConfig>() { Exists = true, ShouldExclude = true, Item = new VersionConfig() });
            SetPausedAtStartup(false);
        }

        public void EnableManualMoose()
        {
            SetPausedAtStartup(true);
        }

        public void SetPausedAtStartup(bool status)
        {
            Configuration.StartPaused = new ConfigItem<bool>()
                                                {
                                                    Exists = status,
                                                    IsLocal = _isLocal,
                                                    ShouldExclude = false,
                                                    ShouldMerge = false,
                                                    Item = status
                                                };
        }

        public void SetIgnoreFilePath(string ignoreFile)
        {
            if (ignoreFile.Trim().Length == 0)
            {
                Configuration.IgnoreFile.Exists = false;
                if (_atReadTimeIgnoreFileExisted)
                    Configuration.IgnoreFile.Item.WriteContent = false;
                return;
            }

            Configuration.IgnoreFile.Item.File = ignoreFile;
            Configuration.IgnoreFile.Exists = true;
            Configuration.IgnoreFile.IsLocal = _isLocal;
            Configuration.IgnoreFile.ShouldExclude = false;
            Configuration.IgnoreFile.ShouldMerge = false;
            if (ignoreFile.Trim().Length > 0)
            {
                Configuration.IgnoreFile.Item.WriteContent = true;
                Configuration.IgnoreFile.Item.ContentPath = Path.GetDirectoryName(Path.Combine(Path.GetDirectoryName(Configuration.ConfigurationFullpath), ignoreFile));
                Configuration.IgnoreFile.Item.ContentPath = Configuration.IgnoreFile.Item.ContentPath.Replace("\\..", "");
            }
        }

        public void SetIgnoreFilePatterns(string content)
        {
            if (content.Replace(Environment.NewLine, "").Trim().Length == 0)
                content = "";

            Configuration.IgnoreFile.Item.Content = content;
            Configuration.IgnoreFile.Item.WriteContent = true;
        }

        public void AddIgnoreAssembly(string assembly)
        {
            var item = new ConfigItem<string>();
            item.Exists = true;
            item.IsLocal = _isLocal;
            item.ShouldExclude = false;
            item.ShouldMerge = false;
            item.Item = assembly;
            Configuration.IgnoredAssemblies.Add(item);
        }

        public void RemoveIgnoreAssembly(string assembly)
        {
            Configuration.IgnoredAssemblies.RemoveAll(x => x.Item.Equals(assembly));
        }

        public void AddIgnoreCategory(string category)
        {
            var item = new ConfigItem<string>();
            item.Exists = true;
            item.IsLocal = _isLocal;
            item.ShouldExclude = false;
            item.ShouldMerge = false;
            item.Item = category;
            Configuration.IgnoredCategories.Add(item);
        }

        public void RemoveIgnoreCategory(string category)
        {
            Configuration.IgnoredCategories.RemoveAll(x => x.Item.Equals(category));
        }

        public void SetGrowlNotifyPath(string path)
        {
            if (path.Trim().Length == 0)
            {
                Configuration.GrowlNotifyPath.Exists = false;
                return;
            }

            Configuration.GrowlNotifyPath.Item = path;
            Configuration.GrowlNotifyPath.Exists = true;
            Configuration.GrowlNotifyPath.IsLocal = _isLocal;
            Configuration.GrowlNotifyPath.ShouldExclude = false;
            Configuration.GrowlNotifyPath.ShouldMerge = false;
        }

        public void SetNotifyOnRunStarted(bool state)
        {
            Configuration.NotifyOnStartup.Item = state;
            Configuration.NotifyOnStartup.Exists = !state;
            Configuration.NotifyOnStartup.IsLocal = _isLocal;
            Configuration.NotifyOnStartup.ShouldExclude = false;
            Configuration.NotifyOnStartup.ShouldMerge = false;
        }

        public void SetNotifyOnRunFinished(bool state)
        {
            Configuration.NotifyOnFinish.Item = state;
            Configuration.NotifyOnFinish.Exists = !state;
            Configuration.NotifyOnFinish.IsLocal = _isLocal;
            Configuration.NotifyOnFinish.ShouldExclude = false;
            Configuration.NotifyOnFinish.ShouldMerge = false;
        }

        public void SetCustomOutputPath(string path)
        {
            if (path.Trim().Length == 0)
            {
                Configuration.BuildOutputPath.Exists = false;
                return;
            }

            Configuration.BuildOutputPath.Item = path;
            Configuration.BuildOutputPath.Exists = true;
            Configuration.BuildOutputPath.IsLocal = _isLocal;
            Configuration.BuildOutputPath.ShouldExclude = false;
            Configuration.BuildOutputPath.ShouldMerge = false;
        }

        public void SetDebugMode(bool state)
        {
            Configuration.Debug.Item = state;
            Configuration.Debug.Exists = true;
            Configuration.Debug.IsLocal = _isLocal;
            Configuration.Debug.ShouldExclude = false;
            Configuration.Debug.ShouldMerge = false;
        }

        public void SetMinimizerMode(bool state)
        {
            Configuration.MinimizerDebug.Item = state;
            Configuration.MinimizerDebug.Exists = true;
            Configuration.MinimizerDebug.IsLocal = _isLocal;
            Configuration.MinimizerDebug.ShouldExclude = false;
            Configuration.MinimizerDebug.ShouldMerge = false;
        }

        public void AddMinimizerAssembly(string assembly)
        {
            var item = new ConfigItem<string>();
            item.Exists = true;
            item.IsLocal = _isLocal;
            item.ShouldExclude = false;
            item.ShouldMerge = false;
            item.Item = assembly;
            Configuration.MinimizerAssemblies.Add(item);
        }

        public void AddProfilerNamespace(string ns)
        {
            var item = new ConfigItem<string>();
            item.Exists = true;
            item.IsLocal = _isLocal;
            item.ShouldExclude = false;
            item.ShouldMerge = false;
            item.Item = ns;
            Configuration.ProfilerNamespaces.Add(item);
        }

        public void RemoveMinimizerAssembly(string assembly)
        {
            Configuration.MinimizerAssemblies.RemoveAll(x => x.Item.Equals(assembly));
        }
        
        public void SetMinimizerLevel(string level)
        {
            Configuration.MinimizerLevel.Item = level;
            Configuration.MinimizerLevel.Exists = true;
            Configuration.MinimizerLevel.IsLocal = _isLocal;
            Configuration.MinimizerLevel.ShouldExclude = false;
            Configuration.MinimizerLevel.ShouldMerge = false;
        }

        public void SetProfilerSetup(ProfilerSettings setup)
        {
            Configuration.ProfilerSetup.Item = setup.ToString();
            Configuration.ProfilerSetup.Exists = true;
            Configuration.ProfilerSetup.IsLocal = _isLocal;
            Configuration.ProfilerSetup.ShouldExclude = false;
            Configuration.ProfilerSetup.ShouldMerge = false;
        }

        public void SetIgnoreWarmup(bool showWarmupDialog)
        {
            Configuration.IgnoreWarmup.Item = showWarmupDialog;
            Configuration.IgnoreWarmup.Exists = true;
            Configuration.IgnoreWarmup.IsLocal = _isLocal;
            Configuration.IgnoreWarmup.ShouldExclude = false;
            Configuration.IgnoreWarmup.ShouldMerge = false;
        }

        public void SetIgnoreThisUpgrade(string version)
        {
            Configuration.IgnoreThisUpgrade.Item = version;
            Configuration.IgnoreThisUpgrade.Exists = true;
            Configuration.IgnoreThisUpgrade.IsLocal = _isLocal;
            Configuration.IgnoreThisUpgrade.ShouldExclude = false;
            Configuration.IgnoreThisUpgrade.ShouldMerge = false;
        }

        public void ShowBuildErrorsInFeedbackWindow(bool state)
        {
            Configuration.BuildErrorsInFeedbackWindow.Item = state;
            Configuration.BuildErrorsInFeedbackWindow.Exists = true;
            Configuration.BuildErrorsInFeedbackWindow.IsLocal = _isLocal;
            Configuration.BuildErrorsInFeedbackWindow.ShouldExclude = false;
            Configuration.BuildErrorsInFeedbackWindow.ShouldMerge = false;
        }

        public void ShowBuildWarningsInFeedbackWindow(bool state)
        {
            Configuration.BuildWarningsInFeedbackWindow.Item = state;
            Configuration.BuildWarningsInFeedbackWindow.Exists = true;
            Configuration.BuildWarningsInFeedbackWindow.IsLocal = _isLocal;
            Configuration.BuildWarningsInFeedbackWindow.ShouldExclude = false;
            Configuration.BuildWarningsInFeedbackWindow.ShouldMerge = false;
        }

        public void ShowFailingTestsInFeedbackWindow(bool state)
        {
            Configuration.FailingTestsInFeedbackWindow.Item = state;
            Configuration.FailingTestsInFeedbackWindow.Exists = true;
            Configuration.FailingTestsInFeedbackWindow.IsLocal = _isLocal;
            Configuration.FailingTestsInFeedbackWindow.ShouldExclude = false;
            Configuration.FailingTestsInFeedbackWindow.ShouldMerge = false;
        }

        public void ShowIgnoredTestsInFeedbackWindow(bool state)
        {
            Configuration.IgnoredTestsInFeedbackWindow.Item = state;
            Configuration.IgnoredTestsInFeedbackWindow.Exists = true;
            Configuration.IgnoredTestsInFeedbackWindow.IsLocal = _isLocal;
            Configuration.IgnoredTestsInFeedbackWindow.ShouldExclude = false;
            Configuration.IgnoredTestsInFeedbackWindow.ShouldMerge = false;
        }

        public void RemoveProfilerNamespace(string ns)
        {
            Configuration.ProfilerNamespaces.RemoveAll(x => x.Item.Equals(ns));
        }

        public void SetBuildSetup(bool buildSolution)
        {
            Configuration.BuildSolution.Item = buildSolution;
            Configuration.BuildSolution.Exists = true;
            Configuration.BuildSolution.IsLocal = _isLocal;
            Configuration.BuildSolution.ShouldExclude = false;
            Configuration.BuildSolution.ShouldMerge = false;
        }

        public void SetRealtimeFeedback(bool realtimeFeedback)
        {
            Configuration.RealtimeFeedback.Item = realtimeFeedback;
            Configuration.RealtimeFeedback.Exists = true;
            Configuration.RealtimeFeedback.IsLocal = _isLocal;
            Configuration.RealtimeFeedback.ShouldExclude = false;
            Configuration.RealtimeFeedback.ShouldMerge = false;
        }

        public void SetOverlayNotifications(bool enabled)
        {
            Configuration.OverlayNotifications.Item = enabled;
            Configuration.OverlayNotifications.Exists = true;
            Configuration.OverlayNotifications.IsLocal = _isLocal;
            Configuration.OverlayNotifications.ShouldExclude = false;
            Configuration.OverlayNotifications.ShouldMerge = false;
        }

        public void SetRunAssembliesInParallel(bool enabled)
        {
            Configuration.RunAssembliesInParallel.Item = enabled;
            Configuration.RunAssembliesInParallel.Exists = true;
            Configuration.RunAssembliesInParallel.IsLocal = _isLocal;
            Configuration.RunAssembliesInParallel.ShouldExclude = false;
            Configuration.RunAssembliesInParallel.ShouldMerge = false;
        }

        public void SetTestRunnerCompatibilityMode(bool enabled)
        {
            Configuration.RunTestsInCompatibilityMode.Item = enabled;
            Configuration.RunTestsInCompatibilityMode.Exists = true;
            Configuration.RunTestsInCompatibilityMode.IsLocal = _isLocal;
            Configuration.RunTestsInCompatibilityMode.ShouldExclude = false;
            Configuration.RunTestsInCompatibilityMode.ShouldMerge = false;
        }

        public void SetRiscEnabled(bool isEnabled)
        {
            Configuration.RiscEnabled.Item = isEnabled;
            Configuration.RiscEnabled.Exists = true;
            Configuration.RiscEnabled.IsLocal = _isLocal;
            Configuration.RiscEnabled.ShouldExclude = false;
            Configuration.RiscEnabled.ShouldMerge = false;
        }

        public void SetMSBuildParallelBuildCount(int parallelCount)
        {
            Configuration.MSBuildParallelBuildCount.Item = parallelCount;
            Configuration.MSBuildParallelBuildCount.Exists = true;
            Configuration.MSBuildParallelBuildCount.IsLocal = _isLocal;
            Configuration.MSBuildParallelBuildCount.ShouldExclude = false;
            Configuration.MSBuildParallelBuildCount.ShouldMerge = false;
        }

        internal void SetAnonFeedbackEnabled(bool isEnabled)
        {
            Configuration.AnonFeedback.Item = isEnabled;
            Configuration.AnonFeedback.Exists = true;
            Configuration.AnonFeedback.IsLocal = _isLocal;
            Configuration.AnonFeedback.ShouldExclude = false;
            Configuration.AnonFeedback.ShouldMerge = false;
        }
    }
}
