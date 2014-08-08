using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using AutoTest.Client.Logging;
using AutoTest.Messages.FileStorage;

namespace AutoTest.Client.Config
{
    public class ConfigParser
    {
        private readonly string _configFile;
        private readonly string _localPath;
        private readonly bool _hasLocalConfig;
        private string _watchToken = null;

        private XmlDocument _xml;

        public bool OnlyGlobal { get; private set; }

        public ConfigParser(string solutionFile)
        {
            _watchToken = solutionFile;
            _configFile = FileLogger.GetConfigurationPath();
            OnlyGlobal = !File.Exists(solutionFile);
            if (OnlyGlobal)
                _localPath = "";
            else
                _localPath = Path.GetDirectoryName(solutionFile);
            _hasLocalConfig = File.Exists(getLocalConfig());
        }

        private string getLocalConfig()
        {
            try
            {
                if (_watchToken == null)
                    return "";
                if (!File.Exists(_watchToken) && !Directory.Exists(_watchToken))
                    return "";
                var config = new ConfigurationLocator().GetConfiguration(_watchToken);
                if (config == null)
                    return "";
                return config;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return "";
            }
        }

        public Configuration Parse()
        {
            var config = new Configuration();
            if (!File.Exists(_configFile))
            {
                config.ConfigurationFullpath = _configFile;
                return config;
            }
            if (!OnlyGlobal && !File.Exists(getLocalConfig()))
            {
                config.ConfigurationFullpath = getLocalConfig();
                config.SolutionPath = _localPath;
                return config;
            }
            if (OnlyGlobal)
                return getConfig(_configFile, false);
            else
                return getConfig(getLocalConfig(), false);
        }

        public Configuration ParseGlobal()
        {
            var config = new Configuration();
            if (!File.Exists(_configFile))
                return config;
            return getConfig(_configFile, false);
        }

        public Configuration ParseLocal()
        {
            var file = getLocalConfig();
            var config = new Configuration();
            config.ConfigurationFullpath = file;
            config.SolutionPath = _localPath;
            if (!_hasLocalConfig)
                return config;
            return getConfig(file, true);
        }

        private Configuration getConfig(string path, bool isLocal)
        {
            var config = new Configuration();
            try
            {
                config.ConfigurationFullpath = path;
                config.SolutionPath = _localPath;
                _xml = new XmlDocument();
                _xml.Load(path);
                config.GraphProvider = getValue("configuration/mm-GraphProvider", isLocal);
                config.MinimizerDebug = getBool("configuration/mm-MinimizerDebug", isLocal);
                config.MinimizerAssemblies.AddRange(getListOfStrings("configuration/mm-MinimizerAssemblies/Assembly", isLocal));
                config.MinimizerLevel = getValue("configuration/mm-MinimizerLevel", isLocal);
                config.ProfilerSetup = getValue("configuration/mm-ProfilerSetup", isLocal);
                config.ProfilerNamespaces.AddRange(getListOfStrings("configuration/mm-ProfilerNamespaces/Namespace", isLocal));
                config.IgnoreWarmup = getBool("configuration/mm-IgnoreWarmup", isLocal);
                config.IgnoreThisUpgrade = getValue("configuration/mm-IgnoreThisUpgrade", isLocal);
                config.RealtimeFeedback = getBool("configuration/mm-RealtimeFeedback", isLocal);
                config.OverlayNotifications = getBool("configuration/mm-OverlayNotifications", isLocal, false);
                config.AllDisabled = getBool("configuration/mm-AllDisabled", isLocal, false);
                config.AnonFeedback = getBool("configuration/mm-AnonFeedback", isLocal, true);

                config.BuildErrorsInFeedbackWindow = getBool("configuration/mm-BuildErrorsInFeedbackWindow", isLocal, true);
                config.BuildWarningsInFeedbackWindow = getBool("configuration/mm-BuildWarningsInFeedbackWindow", isLocal, true);
                config.FailingTestsInFeedbackWindow = getBool("configuration/mm-FailingTestsInFeedbackWindow", isLocal, true);
                config.IgnoredTestsInFeedbackWindow = getBool("configuration/mm-IgnoredTestsInFeedbackWindow", isLocal, true);

                config.StartPaused = getBool("configuration/StartPaused", isLocal);
                config.MSBuild.AddRange(GetVersionedSetting("configuration/BuildExecutable", isLocal));
                config.BuildSolution = getBool("configuration/WhenWatchingSolutionBuildSolution", isLocal);
                config.NUnit.AddRange(GetVersionedSetting("configuration/NUnitTestRunner", isLocal));
                config.MSTest.AddRange(GetVersionedSetting("configuration/MSTestRunner", isLocal));
                config.XUnit.AddRange(GetVersionedSetting("configuration/XUnitTestRunner", isLocal));
                config.IgnoreFile = getIgnoreFile(isLocal);
                config.IgnoredAssemblies.AddRange(getListOfStrings("configuration/ShouldIgnoreTestAssembly/Assembly", isLocal));
                config.IgnoredCategories.AddRange(getListOfStrings("configuration/ShouldIgnoreTestCategories/Category", isLocal));
                config.ChangeDelay = getValue("configuration/changedetectiondelay", isLocal);
                config.BuildOutputPath = getValue("configuration/CustomOutput", isLocal);
                config.GrowlNotifyPath = getValue("configuration/growlnotify", isLocal);
                config.GrowlNotifyPath = getValue("configuration/growlnotify", isLocal);
                config.NotifyOnStartup = getBool("configuration/notify_on_run_started", isLocal);
                config.NotifyOnFinish = getBool("configuration/notify_on_run_completed", isLocal);
                config.Debug = getBool("configuration/Debugging", isLocal);
                config.CatMode = getBool("configuration/MMFTW", isLocal, false);
                config.RunAssembliesInParallel = getBool("configuration/RunAssembliesInParallel", isLocal, false);
                config.RunTestsInCompatibilityMode = getBool("configuration/TestRunnerCompatibilityMode", isLocal, false);
                config.RiscEnabled = getBool("configuration/mm-RiscEnabled", isLocal, true);
                config.MSBuildParallelBuildCount = getInt("configuration/MSBuildParallelBuildCount", isLocal, 0);
                return config;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            config = new Configuration {ConfigurationFullpath = path, SolutionPath = _localPath};
            return config;
        }

        private ConfigItem<bool> getBool(string xpath, bool isLocal)
        {
            return getBool(xpath, isLocal, false);
        }

        private ConfigItem<bool> getBool(string xpath, bool isLocal, bool defaultValue)
        {
            var node = _xml.SelectSingleNode(xpath);
            if (node == null)
                return new ConfigItem<bool>() { Item = defaultValue, IsLocal = isLocal, Exists = false };
            return new ConfigItem<bool>() { Item = node.InnerText == "true", IsLocal = isLocal, Exists = true };
        }

        private ConfigItem<int> getInt(string xpath, bool isLocal, int defaultValue)
        {
            var node = _xml.SelectSingleNode(xpath);
            if (node == null)
                return new ConfigItem<int>() { Item = defaultValue, IsLocal = isLocal, Exists = false };
            int val;
            if (int.TryParse(node.InnerText, out val))
                return new ConfigItem<int>() { Item = val, IsLocal = isLocal, Exists = true };
            return new ConfigItem<int>() { Item = defaultValue, IsLocal = isLocal, Exists = false };
        }

        private ConfigItem<string> getValue(string xpath, bool isLocal)
        {
            var node = _xml.SelectSingleNode(xpath);
            if (node == null)
                return new ConfigItem<string>() { Item = "", IsLocal = isLocal, Exists = false };

            var item = new ConfigItem<string>() { Item = node.InnerText, IsLocal = isLocal, Exists = true };
            var overrideIfno = "";
            if (node.Attributes.GetNamedItem("override") != null)
                overrideIfno = node.Attributes.GetNamedItem("override").InnerText;
            if (overrideIfno == "exclude")
                item.ShouldExclude = true;
            if (overrideIfno == "merge")
                item.ShouldMerge = true;

            return item;
        }

        private IEnumerable<ConfigItem<string>> getListOfStrings(string xpath, bool isLocal)
        {
            var list = new List<ConfigItem<string>>();
            var nodes = _xml.SelectNodes(xpath);
            foreach (XmlNode node in nodes)
            {
                var item = new ConfigItem<string>() { Item = node.InnerText, IsLocal = isLocal, Exists = true };
                var overrideIfno = "";
                if (node.ParentNode.Attributes.GetNamedItem("override") != null)
                    overrideIfno = node.ParentNode.Attributes.GetNamedItem("override").InnerText;
                if (overrideIfno == "exclude")
                    item.ShouldExclude = true;
                if (overrideIfno == "merge")
                    item.ShouldMerge = true;
                list.Add(item);
            }
            return list.ToArray();
        }

        private ConfigItem<IgnoreFile> getIgnoreFile(bool isLocal)
        {
            var item = new ConfigItem<IgnoreFile>() { Item = new IgnoreFile(), IsLocal = isLocal, Exists = false };
            item.Item.ContentPath = _localPath;
            var node = _xml.SelectSingleNode("configuration/IgnoreFile");
            if (node == null)
                return item;
            item.Item.File = node.InnerText;
            item.Exists = true;

            var overrideIfno = "";
            if (node.Attributes.GetNamedItem("override") != null)
                overrideIfno = node.Attributes.GetNamedItem("override").InnerText;
            if (overrideIfno == "exclude")
                item.ShouldExclude = true;
            if (overrideIfno == "merge")
                item.ShouldMerge = true;


            if (item.Item.File.Trim().Length == 0)
                return item;
            if (!File.Exists(Path.Combine(_localPath, item.Item.File)))
                return item;
            var file = Path.Combine(_localPath, node.InnerText);
            item.Item.Content = File.ReadAllText(file);
            return item;
        }

        private IEnumerable<ConfigItem<VersionConfig>> GetVersionedSetting(string xpath, bool isLocal)
        {
            var list = new List<ConfigItem<VersionConfig>>();
            var nodes = _xml.SelectNodes(xpath);
            foreach (XmlNode node in nodes)
            {
                var file = node.InnerText;
                var framework = "";
                var overrideIfno = "";
                if (node.Attributes.GetNamedItem("framework") != null)
                    framework = node.Attributes.GetNamedItem("framework").InnerText;
                if (node.Attributes.GetNamedItem("override") != null)
                    overrideIfno = node.Attributes.GetNamedItem("override").InnerText;
                var item = new ConfigItem<VersionConfig>() { Item = new VersionConfig() { Path = file, Framework = framework }, IsLocal = isLocal, Exists = true };
                SetOverrideInfo(item, overrideIfno);
                list.Add(item);
            }
            return list.ToArray();
        }

        private static void SetOverrideInfo(ConfigItem<VersionConfig> item, string property)
        {
            if (property == "exclude")
                item.ShouldExclude = true;
            if (property == "merge")
                item.ShouldMerge = true;
        }
    }
}
