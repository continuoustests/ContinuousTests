using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;

namespace AutoTest.Core.Configuration
{
    class ConfigurationFailureArgument : EventArgs
    {
        public string Message { get; private set; }

        public ConfigurationFailureArgument(string message)
        {
            Message = message;
        }
    }

    public class CoreSection
    {
        private XmlDocument _xml = new XmlDocument();

        public ConfigItem<string> Providers { get; private set; }
        public ConfigItem<bool> StartPaused { get; private set; }
        public ConfigItem<string[]> WatchDirectories { get; private set; }
        public ConfigItem<KeyValuePair<string, string>[]> BuildExecutables { get; private set; }
        public ConfigItem<KeyValuePair<string, string>[]> NUnitTestRunner { get; private set; }
        public ConfigItem<KeyValuePair<string, string>[]> MSTestRunner { get; private set; }
        public ConfigItem<KeyValuePair<string, string>[]> XUnitTestRunner { get; private set; }
        public ConfigItem<KeyValuePair<string, string>[]> MSpecTestRunner { get; private set; }
        public ConfigItem<CodeEditor> CodeEditor { get; private set; }
        public ConfigItem<bool> DebuggingEnabled { get; private set; }
        public ConfigItem<string> MSBuildAdditionalParameters { get; private set; }
        public ConfigItem<int> MSBuildParallelBuildCount { get; private set; } 
		public ConfigItem<string> GrowlNotify { get; private set; }
		public ConfigItem<bool> NotifyOnRunStarted { get; private set; }
		public ConfigItem<bool> NotifyOnRunCompleted { get; private set; }
		public ConfigItem<string> WatchIgnoreFile { get; private set; }
		public ConfigItem<string[]> TestAssembliesToIgnore { get; private set; }
		public ConfigItem<string[]> TestCategoriesToIgnore { get; private set; }
		public ConfigItem<int> FileChangeBatchDelay { get; private set; }
		public ConfigItem<string> CustomOutputPath { get; private set; }
		public ConfigItem<bool> RerunFailedTestsFirst { get; private set; }
        public ConfigItem<bool> WhenWatchingSolutionBuildSolution { get; private set; }
        public ConfigItem<bool> UseAutoTestTestRunner { get; private set; }
        public ConfigItem<bool> UseLowestCommonDenominatorAsWatchPath { get; private set; }
        public ConfigItem<bool> WatchAllFiles { get; private set; }
        public ConfigItem<bool> RunAssembliesInParallel { get; private set; }
        public ConfigItem<bool> TestRunnerCompatibilityMode { get; private set; }
        public ConfigItem<long> LogRecycleSize { get; private set; }
        public ConfigItem<string[]> ProjectsToIgnore { get; private set; }

        public List<KeyValuePair<string, string>> Keys { get; private set; }

        public CoreSection()
        {
            Providers = new ConfigItem<string>(".NET");
            StartPaused = new ConfigItem<bool>(false);
			WatchDirectories = new ConfigItem<string[]>(new string[] {});
            BuildExecutables = new ConfigItem<KeyValuePair<string, string>[]>(new KeyValuePair<string, string>[] {});
            NUnitTestRunner = new ConfigItem<KeyValuePair<string, string>[]>(new KeyValuePair<string, string>[] {});
            MSTestRunner = new ConfigItem<KeyValuePair<string, string>[]>(new KeyValuePair<string, string>[] {});
            XUnitTestRunner = new ConfigItem<KeyValuePair<string, string>[]>(new KeyValuePair<string, string>[] {});
            MSpecTestRunner = new ConfigItem<KeyValuePair<string, string>[]>(new KeyValuePair<string, string>[] {});
            CodeEditor = new ConfigItem<CodeEditor>(new CodeEditor("", ""));
            DebuggingEnabled = new ConfigItem<bool>(false);
            MSBuildAdditionalParameters = new ConfigItem<string>("");
            MSBuildParallelBuildCount = new ConfigItem<int>(0);
			GrowlNotify = new ConfigItem<string>(null);
			NotifyOnRunStarted = new ConfigItem<bool>(true);
			NotifyOnRunCompleted = new ConfigItem<bool>(true);
			WatchIgnoreFile = new ConfigItem<string>("");
			TestAssembliesToIgnore = new ConfigItem<string[]>(new string[] {});
			TestCategoriesToIgnore = new ConfigItem<string[]>(new string[] {});
			FileChangeBatchDelay = new ConfigItem<int>(100);
			CustomOutputPath = new ConfigItem<string>("");
			RerunFailedTestsFirst = new ConfigItem<bool>(false);
            WhenWatchingSolutionBuildSolution = new ConfigItem<bool>(true);
            UseAutoTestTestRunner = new ConfigItem<bool>(true);
            UseLowestCommonDenominatorAsWatchPath = new ConfigItem<bool>(true);
            WatchAllFiles = new ConfigItem<bool>(false);
            RunAssembliesInParallel = new ConfigItem<bool>(false);
            TestRunnerCompatibilityMode = new ConfigItem<bool>(false);
            LogRecycleSize = new ConfigItem<long>(1024000);
            ProjectsToIgnore = new ConfigItem<string[]>(new string[] {});
            Keys = new List<KeyValuePair<string, string>>();
        }

        public void Read(string configFile)
        {
            if (!tryLoadXml(configFile))
				return;
            Providers = getValueItem("configuration/Provider", ".NET");
            StartPaused = getBoolItem("configuration/StartPaused", false);
			WatchDirectories = getValues("configuration/DirectoryToWatch", false);
            BuildExecutables = getVersionedSetting("configuration/BuildExecutable");
            NUnitTestRunner = getVersionedSetting("configuration/NUnitTestRunner");
            MSTestRunner = getVersionedSetting("configuration/MSTestRunner");
            XUnitTestRunner = getVersionedSetting("configuration/XUnitTestRunner");
            MSpecTestRunner = getVersionedSetting("configuration/MachineSpecificationsTestRunner");
            CodeEditor = getCodeEditor();
            DebuggingEnabled = getBoolItem("configuration/Debugging", false);
            MSBuildAdditionalParameters = getValueItem("configuration/MSBuildAdditionalParameters", "");
            MSBuildParallelBuildCount = getIntItem("configuration/MSBuildParallelBuildCount", 0);
			GrowlNotify = getValueItem("configuration/growlnotify", null);
			NotifyOnRunStarted = getBoolItem("configuration/notify_on_run_started", true);
			NotifyOnRunCompleted = getBoolItem("configuration/notify_on_run_completed", true);
			WatchIgnoreFile = getValueItem("configuration/IgnoreFile", "");
			TestAssembliesToIgnore = getValues("configuration/ShouldIgnoreTestAssembly/Assembly", true);
			TestCategoriesToIgnore = getValues("configuration/ShouldIgnoreTestCategories/Category", true);
			FileChangeBatchDelay = getIntItem("configuration/changedetectiondelay", 200);
			CustomOutputPath = getValueItem("configuration/CustomOutput", "");
			RerunFailedTestsFirst = getBoolItem("configuration/RerunFailedTestsFirst", false);
            WhenWatchingSolutionBuildSolution = getBoolItem("configuration/WhenWatchingSolutionBuildSolution", true);
            UseAutoTestTestRunner = getBoolItem("configuration/UseAutoTestTestRunner", true);
            UseLowestCommonDenominatorAsWatchPath = getBoolItem("configuration/UseLowestCommonDenominatorAsWatchPath", true);
            WatchAllFiles = getBoolItem("configuration/WatchAllFiles", false);
            RunAssembliesInParallel = getBoolItem("configuration/RunAssembliesInParallel", false);
            TestRunnerCompatibilityMode = getBoolItem("configuration/TestRunnerCompatibilityMode", false);
            LogRecycleSize = getLongItem("configuration/LogRecycleSize", 1024000);
            ProjectsToIgnore = getValues("configuration/ShouldIgnoreProject/Project", true);
            Keys = getAllKeys("configuration/*");
        }
		
		private bool tryLoadXml(string configFile)
		{
			try
			{
				_xml.Load(configFile);
				return true;
			}
			catch
			{
				return false;
			}
		}
		
        private ConfigItem<KeyValuePair<string, string>[]> getVersionedSetting(string xpath)
        {
			var item = new ConfigItem<KeyValuePair<string, string>[]>(new KeyValuePair<string,string>[] {});
            var nodes = _xml.SelectNodes(xpath);
			if (nodes.Count == 0)
				return item;
			item.SetValue(readVersionedNodes(nodes));
			if (shouldMerge(xpath))
				item.SetShouldMerge();
			if (shouldExcludeFromConfig(xpath))
				item.Exclude();
            return item;
        }
		
		private KeyValuePair<string, string>[] readVersionedNodes(XmlNodeList nodes)
		{
			var executables = new List<KeyValuePair<string, string>>();
            foreach (XmlNode node in nodes)
            {
                var executable = node.InnerText;
                var version = "";
                var attribute = node.SelectSingleNode("@framework");
                if (attribute != null)
                    version = attribute.InnerText;
                executables.Add(new KeyValuePair<string, string>(version, executable));
            }
			return executables.ToArray();
		}
		
		private ConfigItem<bool> getBoolItem(string path, bool defaultValue)
        {
			var item = new ConfigItem<bool>(defaultValue);
			var val = getBool(path, null);
			if (val == null)
				return item;
			item.SetValue((bool) val);
			if (shouldExcludeFromConfig(path))
				item.Exclude();
			return item;
        }
		
		private ConfigItem<int> getIntItem(string path, int defaultValue)
		{
			var item = new ConfigItem<int>(defaultValue);
			var val = getInt(path, null);
			if (val == null)
				return item;
			item.SetValue((int) val);
			if (shouldExcludeFromConfig(path))
				item.Exclude();
			return item;
		}
		
		private ConfigItem<long> getLongItem(string path, long defaultValue)
		{
			var item = new ConfigItem<long>(defaultValue);
			var val = getLong(path, null);
			if (val == null)
				return item;
			item.SetValue((long) val);
			if (shouldExcludeFromConfig(path))
				item.Exclude();
			return item;
		}
		
		private bool shouldExcludeFromConfig(string path)
		{
			return checkOverrideAttribute(path, "exclude");
		}
		
		private bool shouldMerge(string path)
		{
			return checkOverrideAttribute(path, "merge");
		}
		
		private bool checkOverrideAttribute(string path, string content)
		{
			var node = _xml.SelectSingleNode(path);
            var attrib = node.Attributes["override"];
			if (attrib == null)
				return false;
            return attrib.InnerText.ToLower().Equals(content);
		}
		
		private bool? getBool(string nodeName, bool? defaultValue)
		{
			bool state;
            var value = getValue(nodeName, "");
            if (bool.TryParse(value, out state))
                return state;
            return defaultValue;
		}
		
		private int? getInt(string nodeName, int? defaultValue)
		{
			int state;
            var value = getValue(nodeName, "");
            if (int.TryParse(value, out state))
                return state;
            return defaultValue;
		}
		
		private long? getLong(string nodeName, long? defaultValue)
		{
			long state;
            var value = getValue(nodeName, "");
            if (long.TryParse(value, out state))
                return state;
            return defaultValue;
		}

        private ConfigItem<CodeEditor> getCodeEditor()
        {
			var item = new ConfigItem<CodeEditor>(new CodeEditor("", ""));
            var executable = getValue("configuration/CodeEditor/Executable", null);
			if (executable == null)
				return item;
            var arguments = getValue("configuration/CodeEditor/Arguments", "");
			if (shouldExcludeFromConfig("configuration/CodeEditor"))
				item.Exclude();
            return item.SetValue(new CodeEditor(executable, arguments));
        }
		
		private ConfigItem<string> getValueItem(string nodeName, string defaultValue)
        {
			var item = new ConfigItem<string>(defaultValue);
            var str = getValue(nodeName, null);
			if (str == null)
				return item;
			item.SetValue(str);
			if (shouldExcludeFromConfig(nodeName))
				item.Exclude();
			return item;
        }
		
        private string getValue(string nodeName, string defaultValue)
        {
            var node = _xml.SelectSingleNode(nodeName);
            if (node == null)
                return defaultValue;
            return node.InnerText;
        }

		private string getParentNode(string path)
		{
			return path.Substring(0, path.LastIndexOf('/'));
		}
		
        private ConfigItem<string[]> getValues(string nodeName, bool hasParent)
        {
			var item = new ConfigItem<string[]>(new string[] {});
            var values = new List<string>();
            var nodes = _xml.SelectNodes(nodeName);
			if (nodes.Count == 0)
				return item;
            foreach (XmlNode node in nodes)
                values.Add(node.InnerText);
			item.SetValue(values.ToArray());
			var mainNode = nodeName;
			if (hasParent)
				mainNode = getParentNode(nodeName);
			if (shouldMerge(mainNode))
				item.SetShouldMerge();
			if (shouldExcludeFromConfig(mainNode))
				item.Exclude();
            return item;
        }

        private List<KeyValuePair<string, string>> getAllKeys(string p)
        {
            var list = new List<KeyValuePair<string, string>>();
            foreach (XmlNode node in _xml.SelectNodes(p))
                list.Add(new KeyValuePair<string, string>(node.Name, node.InnerXml));
            return list;
        }
    }
}
