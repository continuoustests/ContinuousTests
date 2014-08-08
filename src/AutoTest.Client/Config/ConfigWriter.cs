using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace AutoTest.Client.Config
{
    public class ConfigWriter
    {
        private XmlDocument _document;

        public string Transform(Configuration config)
        {
            if (config.ConfigurationFullpath == null)
                return "";
            _document = new XmlDocument();
            LoadXml(config);
            WriteStringSetting(config.GraphProvider, "mm-GraphProvider");
            WriteBoolSetting(config.MinimizerDebug, "mm-MinimizerDebug");
            writeStringListSetting(config.MinimizerAssemblies, "mm-MinimizerAssemblies", "Assembly");
            WriteStringSetting(config.MinimizerLevel, "mm-MinimizerLevel");
            WriteStringSetting(config.ProfilerSetup, "mm-ProfilerSetup");
            writeStringListSetting(config.ProfilerNamespaces, "mm-ProfilerNamespaces", "Namespace");
            WriteBoolSetting(config.IgnoreWarmup, "mm-IgnoreWarmup");
            WriteStringSetting(config.IgnoreThisUpgrade, "mm-IgnoreThisUpgrade");
            WriteBoolSetting(config.RealtimeFeedback, "mm-RealtimeFeedback");
            WriteBoolSetting(config.OverlayNotifications, "mm-OverlayNotifications");
            WriteBoolSetting(config.RiscEnabled, "mm-RiscEnabled");
            WriteBoolSetting(config.AllDisabled, "mm-AllDisabled");
            WriteBoolSetting(config.AnonFeedback, "mm-AnonFeedback");

            WriteBoolSetting(config.BuildErrorsInFeedbackWindow, "mm-BuildErrorsInFeedbackWindow");
            WriteBoolSetting(config.BuildWarningsInFeedbackWindow, "mm-BuildWarningsInFeedbackWindow");
            WriteBoolSetting(config.FailingTestsInFeedbackWindow, "mm-FailingTestsInFeedbackWindow");
            WriteBoolSetting(config.IgnoredTestsInFeedbackWindow, "mm-IgnoredTestsInFeedbackWindow");

            WriteBoolSetting(config.StartPaused, "StartPaused");
            writeVersionedSetting(config.MSBuild, "BuildExecutable");
            WriteBoolSetting(config.BuildSolution, "WhenWatchingSolutionBuildSolution");
            writeVersionedSetting(config.NUnit, "NUnitTestRunner");
            writeVersionedSetting(config.MSTest, "MSTestRunner");
            writeVersionedSetting(config.XUnit, "XUnitTestRunner");
            writeIgnoreFileSetting(config.IgnoreFile);
            writeStringListSetting(config.IgnoredAssemblies, "ShouldIgnoreTestAssembly", "Assembly");
            writeStringListSetting(config.IgnoredCategories, "ShouldIgnoreTestCategories", "Category");
            WriteStringSetting(config.ChangeDelay, "changedetectiondelay");
            WriteStringSetting(config.BuildOutputPath, "CustomOutput");
            WriteStringSetting(config.GrowlNotifyPath, "growlnotify");
            WriteBoolSetting(config.NotifyOnStartup, "notify_on_run_started");
            WriteBoolSetting(config.NotifyOnFinish, "notify_on_run_completed");
            WriteBoolSetting(config.Debug, "Debugging");
            WriteBoolSetting(config.RunAssembliesInParallel, "RunAssembliesInParallel");
            WriteBoolSetting(config.RunTestsInCompatibilityMode, "TestRunnerCompatibilityMode");
            WriteIntSetting(config.MSBuildParallelBuildCount, "MSBuildParallelBuildCount");
            return WriteXml();
        }

        private void writeIgnoreFileSetting(ConfigItem<IgnoreFile> item)
        {
            RemoveNode("IgnoreFile");
            if (item.Exists)
                writeSimpleSetting(item.Item.File, true, item.ShouldExclude, "IgnoreFile");
            try
            {
                if (item.Item.WriteContent)
                {
                    if (Directory.Exists(Path.GetDirectoryName(item.Item.File)) && Path.IsPathRooted(item.Item.File))
                        File.WriteAllText(item.Item.File, item.Item.Content);
                    else if (Directory.Exists(item.Item.ContentPath))
                        File.WriteAllText(Path.Combine(item.Item.ContentPath, item.Item.File), item.Item.Content);
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.Write(ex);
            }
        }

        private void writeStringListSetting(IEnumerable<ConfigItem<string>> list, string nodeName, string itemNodeName)
        {
            var mainNode = _document.SelectSingleNode("configuration/" + nodeName);
            if (mainNode != null)
                mainNode.ParentNode.RemoveChild(mainNode);

            if (list.Where(x => x.Exists).Count() == 0)
                return;
            
            mainNode = createNode(nodeName, mainNode);
            var @override = getOverrideAttribute(mainNode);
            SetExclude(list.Where(x => x.ShouldExclude).Count() > 0, mainNode, @override);
            SetMerge(list.Where(x => x.ShouldMerge).Count() > 0, mainNode, @override);
            foreach (var item in list)
            {
                if (!item.Exists)
                    continue;
                var node = _document.SelectSingleNode("configuration/" + nodeName + "/" + itemNodeName + "[.  ='" + item.Item + "']");
                if (node != null)
                    continue;
                
                node = _document.CreateElement(itemNodeName);
                node.InnerText = item.Item;
                mainNode.AppendChild(node);
            }
        }

        private void writeVersionedSetting(IEnumerable<ConfigItem<VersionConfig>> list, string nodeName)
        {
            foreach (XmlNode node in _document.SelectNodes("configuration/" + nodeName))
                node.ParentNode.RemoveChild(node);

            if (list.Where(x => x.Exists).Count() == 0)
                return;

            foreach (var item in list)
            {
                if (!item.Exists)
                    continue;
                var path = "configuration/" + nodeName;
                if (IsValidFramework(item.Item.Framework))
                    path += "[@framework='" + item.Item.Framework.Trim() + "']";
                var node = _document.SelectSingleNode(path);
                if (node == null)
                    node = createNode(nodeName, node);

                var @override = getOverrideAttribute(node);
                SetExclude(item.ShouldExclude, node, @override);
                SetMerge(item.ShouldMerge, node, @override);

                if (IsValidFramework(item.Item.Framework))
                {
                    var framework = _document.CreateAttribute("framework");
                    framework.InnerText = item.Item.Framework.Trim();
                    node.Attributes.Append(framework);
                }
                node.InnerText = item.Item.Path.Trim();
            }
        }

        private static bool IsValidFramework(string framework)
        {
            return framework != null && framework.Trim().Length > 0;
        }

        private void WriteStringSetting(ConfigItem<string> configItem, string nodeName)
        {
            RemoveNode(nodeName);
            if (!configItem.Exists)
                return;
            writeSimpleSetting(configItem.Item.Trim(), configItem.Exists, configItem.ShouldExclude, nodeName);
        }

        private void WriteBoolSetting(ConfigItem<bool> configItem, string nodeName)
        {
            RemoveNode(nodeName);
            if (!configItem.Exists)
                return;
            writeSimpleSetting(configItem.Item.ToString().ToLower(), configItem.Exists, configItem.ShouldExclude, nodeName);
        }

        private void WriteIntSetting(ConfigItem<int> configItem, string nodeName)
        {
            RemoveNode(nodeName);
            if (!configItem.Exists)
                return;
            writeSimpleSetting(configItem.Item.ToString().ToLower(), configItem.Exists, configItem.ShouldExclude, nodeName);
        }

        private void RemoveNode(string nodeName)
        {
            var node = _document.SelectSingleNode("configuration/" + nodeName);
            if (node != null)
                node.ParentNode.RemoveChild(node);
        }

        private void writeSimpleSetting(string value, bool exists, bool exclude, string nodeName)
        {
            var node = _document.SelectSingleNode("configuration/" + nodeName);
            if (node == null)
                node = createNode(nodeName, node);
            var @override = getOverrideAttribute(node);
            SetExclude(exclude, node, @override);
            node.InnerText = value;
        }

        private void SetMerge(bool shouldSet, XmlNode node, XmlAttribute @override)
        {
            SetOverrideAttribute("merge", shouldSet, node, @override);

        }

        private void SetExclude(bool shouldSet, XmlNode node, XmlAttribute @override)
        {
            SetOverrideAttribute("exclude", shouldSet, node, @override);
        }

        private void SetOverrideAttribute(string value, bool shouldExclude, XmlNode node, XmlAttribute @override)
        {
            if (shouldExclude == true)
            {
                if (@override == null)
                {
                    @override = _document.CreateAttribute("override");
                    node.Attributes.Append(@override);
                }
                @override.InnerText = value;
            }
            else
            {
                if (@override != null)
                    node.Attributes.Remove(@override);
            }
        }

        private static XmlAttribute getOverrideAttribute(XmlNode node)
        {
            var @override = node.Attributes["override"];
            return @override;
        }

        private XmlNode createNode(string nodeName, XmlNode node)
        {
            node = _document.CreateElement(nodeName);
            var element = _document.DocumentElement;
            element.AppendChild(node);
            return node;
        }

        private string WriteXml()
        {
            var settings = new XmlWriterSettings
                               {
                                   Indent = true,
                                   NewLineChars = Environment.NewLine,
                                   IndentChars = "\t"
                               };
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, settings))
            {
                _document.Save(writer);
            }
            // TODO: do this properly.. hack for now
            return sb.ToString().Replace("utf-16", "utf-8");
        }

        private void LoadXml(Configuration config)
        {
            try
            {
                _document.Load(config.ConfigurationFullpath);
            }
            catch
            {
                _document = new XmlDocument();
                var element = _document.CreateElement("configuration");
                _document.AppendChild(element);
            }
        }
    }
}
