using System;
using System.Configuration;
using System.IO;
using System.Linq;
using AutoTest.Core.Messaging;
using System.Collections.Generic;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.FileSystem;
using AutoTest.Core.DebugLog;
using AutoTest.Messages;
using System.Text;

namespace AutoTest.Core.Configuration
{
    public class Config : IConfiguration
    {
        private IMessageBus _bus;
		private ILocateWriteLocation _defaultConfigLocator;
		private string _ignoreFile;
        private List<KeyValuePair<string, string>> _keys = new List<KeyValuePair<string,string>>();
        private string _overriddenSolution = null;
		
        private string[] _watchDirectories;
        private List<KeyValuePair<string, string>> _buildExecutables = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> _nunitTestRunners = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> _msTestRunner = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> _xunitTestRunner = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> _mspecTestRunner = new List<KeyValuePair<string, string>>();
        private CodeEditor _codeEditor;
        private bool _debuggingEnabled;

        public bool StartPaused { get; private set; }
        public string WatchPath { get; private set; } // Adjusted watch path (use common denominator)
        public string WatchToken { get; private set; } // Original watch token
        public string SolutionToBuild
        {
            get
            {
                if (_overriddenSolution != null)
                    return _overriddenSolution;
                return WatchToken;
            }
        }

        public string Providers { get; private set; }
        public string MSBuildAdditionalParameters { get; private set; }
        public int MSBuildParallelBuildCount { get; private set; }
		public string GrowlNotify { get; private set; }
		public bool NotifyOnRunStarted { get; private set; }
		public bool NotifyOnRunCompleted { get; private set; }
		public string[] WatchIgnoreList { get; private set; }
		public bool ShouldUseBinaryChangeIgnoreLists { get { return _buildExecutables.Count == 0; } }
		public int FileChangeBatchDelay { get; private set; }
		public string[] TestAssembliesToIgnore { get; private set; }
		public string[] TestCategoriesToIgnore { get; private set; }
		public string CustomOutputPath { get; private set; }
		public bool RerunFailedTestsFirst { get; private set; }
        public bool WhenWatchingSolutionBuildSolution { get; private set; }
        public bool UseAutoTestTestRunner { get; private set; }
        public bool UseLowestCommonDenominatorAsWatchPath { get; private set; }
        public bool WatchAllFiles { get; private set; }
        public bool RunAssembliesInParallel { get; private set; }
        public bool TestRunnerCompatibilityMode { get; private set; }
		public long LogRecycleSize { get; private set; }
        public string[] ProjectsToIgnore { get; private set; }

        public string IgnoreFile { get { return _ignoreFile; } }

        public bool ShouldBuildSolution { get { return File.Exists(SolutionToBuild) && WhenWatchingSolutionBuildSolution; } }
		
        public Config(IMessageBus bus, ILocateWriteLocation defaultConfigLocator)
        {
            _bus = bus;
			_defaultConfigLocator = defaultConfigLocator;
			var core = getConfiguration();
            tryToConfigure(core);
            WatchIgnoreList = new string[] {};
        }

        public void Reload(string localConfiguration)
        {
            var core = getConfiguration();
            tryToConfigure(core);
            if (localConfiguration != null)
			{
	            if (File.Exists(localConfiguration))
	                Merge(localConfiguration);
			}
			BuildIgnoreList(WatchPath);
            SetBuildProvider();
            AnnounceTrackerType();
        }

        public void BuildIgnoreList(string path)
        {
            BuildIgnoreListFromPath(path);
            var list = getIgnorePatterns();
            if (list.Length > 0)
                _bus.Publish(new InformationMessage(string.Format("Ignoring \"{0}\"", list)));
        }

        private string getIgnorePatterns()
        {
            var list = "";
            foreach (var pattern in WatchIgnoreList)
                list += (list.Length == 0 ? "" : "|") + pattern;
            if (CustomOutputPath != null && CustomOutputPath.Length > 0)
                list += (list.Length == 0 ? "" : "|") + CustomOutputPath.Replace('\\', '/');
            return list;
        }

        public string AllSettings(string key)
        {
            var setting = _keys.Where(x => x.Key.Equals(key)).Select(x => x.Value).FirstOrDefault();
            if (setting == null)
                return "";
            return setting;
        }
		
		public void SetBuildProvider()
		{
			if (_buildExecutables == null)
				return;
			
			if (_buildExecutables.Count == 0)
			{
				FileChangeBatchDelay = 1000;
				_bus.SetBuildProvider("NoBuild");
                CustomOutputPath = Path.Combine("bin", "Debug");
			}
		}
		
		public void AnnounceTrackerType()
		{
			var trackerType = "file change tracking";
			if (_buildExecutables.Count == 0)
				trackerType = "assembly tracking";
			_bus.Publish(new InformationMessage(string.Format("Tracker type: {0}", trackerType)));
		}

        public void SetCustomOutputPath(string path)
        {
            CustomOutputPath = path;
        }

        public void OverrideSolution(string solution)
        {
            _overriddenSolution = solution;
        }

        public void ResetSolution()
        {
            _overriddenSolution = null;
        }
		
		public void Merge(string configuratoinFile)
		{
			var core = getConfiguration(configuratoinFile);
            if (core.Providers.WasReadFromConfig)
                Providers = core.Providers.Value;
            if (core.StartPaused.WasReadFromConfig)
                StartPaused = core.StartPaused.Value;
			mergeVersionedItem(_buildExecutables, core.BuildExecutables);
			mergeVersionedItem(_nunitTestRunners, core.NUnitTestRunner);
			mergeVersionedItem(_msTestRunner, core.MSTestRunner);
			mergeVersionedItem(_xunitTestRunner, core.XUnitTestRunner);
			mergeVersionedItem(_mspecTestRunner, core.MSpecTestRunner);
            mergeCodeEditor(core.CodeEditor);
			if (core.DebuggingEnabled.WasReadFromConfig)
				_debuggingEnabled = core.DebuggingEnabled.Value;
            if (core.MSBuildAdditionalParameters.WasReadFromConfig)
                MSBuildAdditionalParameters = mergeValueItem(core.MSBuildAdditionalParameters, "");
            if (core.MSBuildParallelBuildCount.WasReadFromConfig)
                MSBuildParallelBuildCount = mergeValueItem(core.MSBuildParallelBuildCount, 0);
			if (core.GrowlNotify.WasReadFromConfig)
				GrowlNotify = mergeValueItem(core.GrowlNotify, null);
			if (core.NotifyOnRunStarted.WasReadFromConfig)
				NotifyOnRunStarted = core.NotifyOnRunStarted.Value;
			if (core.NotifyOnRunCompleted.WasReadFromConfig)
				NotifyOnRunCompleted = core.NotifyOnRunCompleted.Value;
			if (core.TestAssembliesToIgnore.WasReadFromConfig)
				TestAssembliesToIgnore = mergeValues(TestAssembliesToIgnore, core.TestAssembliesToIgnore);
			if (core.TestCategoriesToIgnore.WasReadFromConfig)
				TestCategoriesToIgnore = mergeValues(TestCategoriesToIgnore, core.TestCategoriesToIgnore);
			if (core.WatchIgnoreFile.WasReadFromConfig)
				_ignoreFile = mergeValueItem(core.WatchIgnoreFile, "");
			if (core.FileChangeBatchDelay.WasReadFromConfig)
				FileChangeBatchDelay = core.FileChangeBatchDelay.Value;
			if (core.CustomOutputPath.WasReadFromConfig)
				CustomOutputPath = core.CustomOutputPath.Value;
			if (core.RerunFailedTestsFirst.WasReadFromConfig)
				RerunFailedTestsFirst = core.RerunFailedTestsFirst.Value;
            if (core.WhenWatchingSolutionBuildSolution.WasReadFromConfig)
                WhenWatchingSolutionBuildSolution = core.WhenWatchingSolutionBuildSolution.Value;
            if (core.UseAutoTestTestRunner.WasReadFromConfig)
                UseAutoTestTestRunner = core.UseAutoTestTestRunner.Value;
            if (core.UseLowestCommonDenominatorAsWatchPath.WasReadFromConfig)
                UseLowestCommonDenominatorAsWatchPath = core.UseLowestCommonDenominatorAsWatchPath.Value;
            if (core.WatchAllFiles.WasReadFromConfig)
                WatchAllFiles = core.WatchAllFiles.Value;
            if (core.RunAssembliesInParallel.WasReadFromConfig)
                RunAssembliesInParallel = core.RunAssembliesInParallel.Value;
            if (core.TestRunnerCompatibilityMode.WasReadFromConfig)
                TestRunnerCompatibilityMode = core.TestRunnerCompatibilityMode.Value;
			if (core.LogRecycleSize.WasReadFromConfig)
                LogRecycleSize = core.LogRecycleSize.Value;
            if (core.ProjectsToIgnore.WasReadFromConfig)
                ProjectsToIgnore = mergeValues(ProjectsToIgnore, core.ProjectsToIgnore);
            core.Keys.ForEach(x => mergeKey(x));
		}

        private void mergeKey(KeyValuePair<string, string> x)
        {
            _keys.RemoveAll(k => k.Key.Equals(x.Key));
            _keys.Add(x);
        }
		
		private void tryToConfigure(CoreSection core)
        {
            try
            {
                Providers = core.Providers.Value;
                StartPaused = core.StartPaused.Value;
                _watchDirectories = core.WatchDirectories.Value;
                _buildExecutables.AddRange(core.BuildExecutables.Value);
                _nunitTestRunners.AddRange(core.NUnitTestRunner.Value);
                _msTestRunner.AddRange(core.MSTestRunner.Value);
                _xunitTestRunner.AddRange(core.XUnitTestRunner.Value);
                _mspecTestRunner.AddRange(core.MSpecTestRunner.Value);
                _codeEditor = core.CodeEditor.Value;
                _debuggingEnabled = core.DebuggingEnabled.Value;
                MSBuildAdditionalParameters = core.MSBuildAdditionalParameters.Value;
                MSBuildParallelBuildCount = core.MSBuildParallelBuildCount.Value;
				GrowlNotify = core.GrowlNotify.Value;
				NotifyOnRunStarted = core.NotifyOnRunStarted.Value;
				NotifyOnRunCompleted = core.NotifyOnRunCompleted.Value;
				TestAssembliesToIgnore = core.TestAssembliesToIgnore.Value;
				TestCategoriesToIgnore = core.TestCategoriesToIgnore.Value;
				_ignoreFile = core.WatchIgnoreFile.Value;
				FileChangeBatchDelay = core.FileChangeBatchDelay.Value;
				CustomOutputPath = core.CustomOutputPath.Value;
				RerunFailedTestsFirst = core.RerunFailedTestsFirst.Value;
                WhenWatchingSolutionBuildSolution = core.WhenWatchingSolutionBuildSolution.Value;
                UseAutoTestTestRunner = core.UseAutoTestTestRunner.Value;
                UseLowestCommonDenominatorAsWatchPath = core.UseLowestCommonDenominatorAsWatchPath.Value;
                WatchAllFiles = core.WatchAllFiles.Value;
                RunAssembliesInParallel = core.RunAssembliesInParallel.Value;
                TestRunnerCompatibilityMode = core.TestRunnerCompatibilityMode.Value;
                LogRecycleSize = core.LogRecycleSize.Value;
                ProjectsToIgnore = core.ProjectsToIgnore.Value;
                _keys = core.Keys;
            }
            catch (Exception ex)
            {
                DebugLog.Debug.FailedToConfigure(ex);
                throw;
            }
        }
		
		private string[] mergeValues(string[] setting, ConfigItem<string[]> settingToMerge)
		{
			if (settingToMerge.ShouldExclude)
				return new string[] {};
			if (settingToMerge.ShouldMerge)
			{
				var list = new List<string>();
				list.AddRange(setting);
				list.AddRange(settingToMerge.Value);
				return list.ToArray();
			}
			return settingToMerge.Value;
		}
		
		private string mergeValueItem(ConfigItem<string> settingToMerge, string defaultValue)
		{
			if (settingToMerge.ShouldExclude)
				return defaultValue;
			return settingToMerge.Value;
		}

        private int mergeValueItem(ConfigItem<int> settingToMerge, int defaultValue)
        {
            if (settingToMerge.ShouldExclude)
                return defaultValue;
            return settingToMerge.Value;
        }
		
		private void mergeCodeEditor(ConfigItem<CodeEditor> settingToMerge)
		{
			if (!settingToMerge.WasReadFromConfig)
				return;
			if (settingToMerge.ShouldExclude)
			{
				_codeEditor = new CodeEditor("", "");
				return;
			}
			_codeEditor = settingToMerge.Value;
		}
		
		private void mergeVersionedItem(List<KeyValuePair<string, string>> setting, ConfigItem<KeyValuePair<string, string>[]> settingToMerge)
		{
			if (!settingToMerge.WasReadFromConfig)
				return;
			if (settingToMerge.ShouldExclude)
			{
				setting.Clear();
				return;
			}
			if (settingToMerge.ShouldMerge)
			{
				foreach (var mergedItem in settingToMerge.Value)
				{
					setting.RemoveAll(x => x.Key.Equals(mergedItem.Key));
					setting.Add(mergedItem);
				}
				return;
			}
			setting.Clear();
			setting.AddRange(settingToMerge.Value);
		}
		
		private CoreSection getConfiguration()
		{
			return getConfiguration(_defaultConfigLocator.GetConfigurationFile());
		}
		
		private CoreSection getConfiguration(string configFile)
		{
			var core = new CoreSection();
			if (!File.Exists(configFile))
			{
				Debug.ConfigurationFileMissing();
				return core;
			}
			core.Read(configFile);
			return core;
		}

        public void ValidateSettings()
        {
            if (noneExists(_buildExecutables))
                _bus.Publish(new WarningMessage("Invalid build executable specified in the configuration file. Builds will not be run."));
            if (noneExists(_nunitTestRunners))
                _bus.Publish(new WarningMessage("NUnit test runner not specified. NUnit tests will not be run."));
            if (noneExists(_msTestRunner))
                _bus.Publish(new WarningMessage("MSTest test runner not specified. MSTest tests will not be run."));
			if (noneExists(_xunitTestRunner))
                _bus.Publish(new WarningMessage("XUnit test runner not specified. XUnit tests will not be run."));
            if (noneExists(_mspecTestRunner))
                _bus.Publish(new WarningMessage("Machine.Specifications test runner not specified. Machine.Specifications tests will not be run."));
            if (_codeEditor == null || !File.Exists(_codeEditor.Executable))
                _bus.Publish(new WarningMessage("Code editor not specified"));
            _bus.Publish(new InformationMessage(string.Format("MbUnit tests: The MbUnit runner needs a reference to your gallio bin directory to be able to run tests. Edit TestRunners{0}MbUnit{0}mbunit.config to point to the correct path.", Path.DirectorySeparatorChar)));
            _bus.Publish(new InformationMessage(getIgnoreList()));
        }

        private string getIgnoreList()
        {
            var sb = new StringBuilder();
            sb.Append("Ignore patterns: ");
            foreach (var item in WatchIgnoreList)
                sb.Append(item + "|");
            return sb.ToString();
        }
		
		public void BuildIgnoreListFromPath(string watchPath)
		{
            var file = new PathParser(_ignoreFile).ToAbsolute(watchPath);
			
			Debug.WriteDebug("Using ignore file {0}", file);
			if (File.Exists(file))
				WatchIgnoreList = getLineArrayFromFile(file, watchPath);
			else
				WatchIgnoreList = new string[] { };
		}

		private string[] getLineArrayFromFile(string file, string watchPath)
		{
			var lines = new List<string>();
			using (var reader = new StreamReader(file))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					var trimmedLine = line.Trim();
					if (trimmedLine.Length == 0)
						continue;
					if (trimmedLine.StartsWith("!"))
						continue;
					if (trimmedLine.StartsWith("#"))
						continue;
                    trimmedLine = trimmedLine.Replace('\\', '/');
                    trimmedLine = getRelativePath(trimmedLine, watchPath);
					lines.Add(trimmedLine);
				}
			}
			return lines.ToArray();
		}

        private string getRelativePath(string path, string relativeTo)
        {
            var truePath = relativeTo.Replace('\\', '/');
            if (path.StartsWith(truePath))
                return path.Substring(truePath.Length, path.Length - truePath.Length);
            return path;
        }
		
        private bool noneExists(List<KeyValuePair<string, string>> files)
        {
			if (files == null)
				return true;
			
            foreach (var file in files)
            {
                if (File.Exists(file.Value))
                    return false;
            }
            return true;
        }

        public string[] WatchDirectores
        {
            get { return _watchDirectories; }
            set { _watchDirectories = value; }
        }

        public string NunitTestRunner(string version)
        {
            return getVersionedSetting(version, _nunitTestRunners);
        }
		
		public string GetSpesificNunitTestRunner(string version)
		{
			if (_nunitTestRunners.Count == 0)
                return null;
            int index;
            if ((index = _nunitTestRunners.FindIndex(0, b => b.Key.Equals(version))) >= 0)
                return _nunitTestRunners[index].Value;
			return null;
		}

        public string MSTestRunner(string version)
        {
            return getVersionedSetting(version, _msTestRunner);
        }

        public string XunitTestRunner(string version)
        {
            return getVersionedSetting(version, _xunitTestRunner);
        }

        public string MSpecTestRunner(string version)
        {
            return getVersionedSetting(version, _mspecTestRunner);
        }

        public CodeEditor CodeEditor
        {
            get { return _codeEditor; }
            set { _codeEditor = value; }
        }

        public bool DebuggingEnabled
        {
            get { return _debuggingEnabled; }
            set { _debuggingEnabled = value; }
        }

        public string BuildExecutable(ProjectDocument project)
        {
            if (_buildExecutables.Count == 0)
                return "";
            int index;
            if ((index = _buildExecutables.FindIndex(0, b => b.Key.Equals(project.ProductVersion))) >= 0)
                return _buildExecutables[index].Value;
            if ((index = _buildExecutables.FindIndex(0, b => b.Key.Equals(project.Framework))) >= 0)
                return _buildExecutables[index].Value;
            if ((index = _buildExecutables.FindIndex(0, b => b.Key.Equals(""))) >= 0)
                return _buildExecutables[index].Value;
            return _buildExecutables[0].Value;
        }

        public void SetWatchPath(string watchFolder)
        {
            WatchPath = watchFolder;
        }

        public void SetWatchToken(string watchToken)
        {
            WatchToken = watchToken;
        }

        private Action<bool> _logging = null;
        public void SetLoggerStateAction(Action<bool> setupLogging)
        {
            _logging = setupLogging;
        }

        public void EnableLogging()
        {
            if (_logging == null) return;
            _logging(true);
        }

        public void DisableLogging()
        {
            if (_logging == null) return;
            _logging(false);
        }

        private string getVersionedSetting(string version, List<KeyValuePair<string, string>> setting)
        {
            if (setting.Count == 0)
                return "";
            int index;
            if ((index = setting.FindIndex(0, b => b.Key.Equals(version))) >= 0)
                return setting[index].Value;
            if ((index = setting.FindIndex(0, b => b.Key.Equals(""))) >= 0)
                return setting[index].Value;
            return setting[0].Value;
        }
    }
}
