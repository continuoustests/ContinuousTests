using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AutoTest.Client.Config;
using System.IO;
using AutoTest.VM.Messages.Configuration;
using AutoTest.VM.Messages;
using System.Diagnostics;
using AutoTest.Client.Logging;

namespace AutoTest.Client.UI
{
    enum MooseMode
    {
        Mighty,
        Auto,
        Manual
    }

    partial class ConfigurationForm : Form
    {
        private readonly string _watchToken = "";
        private readonly VM _vm;
        private ConfigurationFormWriter _writer;
        private readonly bool _isLocal;
        private bool _isInitializing;
        private MooseMode _originalWorkflow  = MooseMode.Mighty;
        private bool _originalMinimizerDebug;
        private string _originalMinimizer;
        private string _originalProfiler;
        private bool _showErrorsInFeedbackview;
        private bool _showWarningsInFeedbackview;
        private bool _showFailuresInFeedbackview;
        private bool _showIgnoresInFeedbackview;
        private bool _disableAllOriginalValue = false;
        private bool _anonFeedbackOriginalValue = false;
        private Action _doWhatever = null;

        public ConfigurationForm(string watchToken, VM vm, bool isLocal)
        {
            _isLocal = isLocal;
            InitializeComponent();
            if (_isLocal)
                _watchToken = watchToken;
            _vm = vm;
            readConfiguration();
            Text = _isLocal ? "Solution Configuration" : "Global Template Configuration";
        }

        public ConfigurationForm OnWhateverDo(Action action)
        {
            _doWhatever = action;
            return this;
        }

        private void readConfiguration()
        {
            _isInitializing = true;
            var parser = new ConfigParser(_watchToken);
            _writer = new ConfigurationFormWriter(_isLocal, parser.Parse(), new MSBuildLocator());
            setWorkflowSettings();
            setIgnoreFileSettings();
            setTestIgnoreSettings();
            setNotifySettings();
            setVariousSettings();
            setMinimizerSettings();
            _originalMinimizer = _writer.Configuration.MinimizerLevel.Exists ? _writer.Configuration.MinimizerLevel.Item : null;
            _originalProfiler = _writer.Configuration.ProfilerSetup.Exists ? _writer.Configuration.ProfilerSetup.Item : null;
            _disableAllOriginalValue = checkBoxDisableAll.Checked;
            _anonFeedbackOriginalValue = checkBoxAnalytics.Checked;
            _isInitializing = false;
        }

        private void setVariousSettings()
        {
            if (_writer.Configuration.GraphProvider.Exists)
                comboBoxGraphProvider.SelectedIndex = getGraphProviderIndex(_writer.Configuration.GraphProvider);
            else
                comboBoxGraphProvider.SelectedIndex = 0;
            checkBoxMinimizerLogging.Checked = _writer.Configuration.MinimizerDebug.Exists && _writer.Configuration.MinimizerDebug.Item;
            _originalMinimizerDebug = checkBoxMinimizerLogging.Checked;
            if (_writer.Configuration.BuildOutputPath.Exists)
                textBoxCustomOutput.Text = _writer.Configuration.BuildOutputPath.Item;
            if (_writer.Configuration.Debug.Exists)
                checkBoxDebug.Checked = _writer.Configuration.Debug.Item;

            if (_writer.Configuration.BuildErrorsInFeedbackWindow.Exists)
                checkBoxBuildErrors.Checked = _writer.Configuration.BuildErrorsInFeedbackWindow.Item;
            if (_writer.Configuration.BuildWarningsInFeedbackWindow.Exists)
                checkBoxWarnings.Checked = _writer.Configuration.BuildWarningsInFeedbackWindow.Item;
            if (_writer.Configuration.FailingTestsInFeedbackWindow.Exists)
                checkBoxFailing.Checked = _writer.Configuration.FailingTestsInFeedbackWindow.Item;
            if (_writer.Configuration.FailingTestsInFeedbackWindow.Exists)
                checkBoxIgnored.Checked = _writer.Configuration.IgnoredTestsInFeedbackWindow.Item;
            _showErrorsInFeedbackview = checkBoxBuildErrors.Checked;
            _showWarningsInFeedbackview = checkBoxWarnings.Checked;
            _showFailuresInFeedbackview = checkBoxFailing.Checked;
            _showIgnoresInFeedbackview = checkBoxIgnored.Checked;
        }

        private void setMinimizerSettings()
        {
            if (_writer.Configuration.MinimizerLevel.Exists)
            {
                if (_writer.Configuration.MinimizerLevel.Item == "off")
                    comboBoxMinimizer.SelectedIndex = 2;
                else if (_writer.Configuration.MinimizerLevel.Item == "notrun")
                    comboBoxMinimizer.SelectedIndex = 1;
                else
                    comboBoxMinimizer.SelectedIndex = 0;
                    
            }
            else
                comboBoxMinimizer.SelectedIndex = 0;

            if (_writer.Configuration.ProfilerSetup.Exists && _writer.Configuration.ProfilerSetup.Item.ToUpper().Equals(ProfilerSettings.RUN.ToString()))
                comboBoxProfiler.SelectedIndex = 0;
            else if (_writer.Configuration.ProfilerSetup.Exists && _writer.Configuration.ProfilerSetup.Item.ToUpper().Equals(ProfilerSettings.DONTRUN.ToString()))
                comboBoxProfiler.SelectedIndex = 1;
            else if (_writer.Configuration.ProfilerSetup.Exists && _writer.Configuration.ProfilerSetup.Item.ToUpper().Equals(ProfilerSettings.RUNANDAUTODETECT.ToString()))
                comboBoxProfiler.SelectedIndex = 2;
            else
                comboBoxProfiler.SelectedIndex = 2;

            if (_writer.Configuration.RunAssembliesInParallel.Exists)
                checkBoxRunAssembliesInParallel.Checked = _writer.Configuration.RunAssembliesInParallel.Item;

            if (_writer.Configuration.RunTestsInCompatibilityMode.Exists)
                checkBoxCompatibilityMode.Checked = _writer.Configuration.RunTestsInCompatibilityMode.Item;

            if (_writer.Configuration.RiscEnabled.Exists)
                checkBoxUseMargins.Checked = _writer.Configuration.RiscEnabled.Item;
            else
                checkBoxUseMargins.Checked = true;

            foreach (var assembly in _writer.Configuration.MinimizerAssemblies)
                listViewMinimizerAssemblies.Items.Add(assembly.Item);
            foreach (var ns in _writer.Configuration.ProfilerNamespaces)
                listViewProfilerNamespaces.Items.Add(ns.Item);
        }

        private int getGraphProviderIndex(ConfigItem<string> configItem)
        {
            if (configItem.Item.ToUpper().Equals(GraphProviderConverter.FromEnum(GraphProvider.BUILTINDARK)))
                return 0;
            if (configItem.Item.ToUpper().Equals(GraphProviderConverter.FromEnum(GraphProvider.BUILTINLIGHT)))
                return 1;
            if (configItem.Item.ToUpper().Equals(GraphProviderConverter.FromEnum(GraphProvider.GRAPHVIZ)))
                return 2;
            if (configItem.Item.ToUpper().Equals(GraphProviderConverter.FromEnum(GraphProvider.DGML)))
                return 3;
            if (configItem.Item.ToUpper().Equals(GraphProviderConverter.FromEnum(GraphProvider.WINDOW)))
                return 4;

            return 0;
        }

        private void setNotifySettings()
        {
            if (_writer.Configuration.GrowlNotifyPath.Exists)
                textBoxGrowlPath.Text = _writer.Configuration.GrowlNotifyPath.Item;
            if (_writer.Configuration.NotifyOnStartup.Exists)
                checkBoxNotifyOnRunStart.Checked = _writer.Configuration.NotifyOnStartup.Item;
            if (_writer.Configuration.NotifyOnFinish.Exists)
                checkBoxNotifyOnRunFinished.Checked = _writer.Configuration.NotifyOnFinish.Item;
            if (_writer.Configuration.OverlayNotifications.Exists)
                checkBoxOverlayNotififations.Checked = _writer.Configuration.OverlayNotifications.Item;
        }

        private void setTestIgnoreSettings()
        {
            foreach (var assembly in _writer.Configuration.IgnoredAssemblies)
                listViewIgnoreAssembly.Items.Add(assembly.Item);
            foreach (var category in _writer.Configuration.IgnoredCategories)
                listViewIgnoreCategory.Items.Add(category.Item);
        }

        private void setIgnoreFileSettings()
        {
            var parser = new ConfigParser("");
            var globalConfig = parser.Parse();
            if (_writer.Configuration.IgnoreFile.Exists)
                textBoxIgnoreFile.Text = _writer.Configuration.IgnoreFile.Item.File;
            else
            {
                if (globalConfig.IgnoreFile.Exists)
                    textBoxIgnoreFile.Text = globalConfig.IgnoreFile.Item.File;
            }
            if (_isLocal)
            {
                var fileName = "";
                if (_writer.Configuration.IgnoreFile.Exists && _writer.Configuration.IgnoreFile.Item.File.Trim().Length > 0)
                {
                    if (Path.IsPathRooted(_writer.Configuration.IgnoreFile.Item.File.Trim()))
                        fileName = _writer.Configuration.IgnoreFile.Item.File.Trim();
                    else
                        fileName = Path.Combine(_writer.Configuration.IgnoreFile.Item.ContentPath, _writer.Configuration.IgnoreFile.Item.File.Trim());
                }
                else
                {
                    if (globalConfig.IgnoreFile.Exists && globalConfig.IgnoreFile.Item.File.Trim().Length > 0)
                    {
                        if (Path.IsPathRooted(globalConfig.IgnoreFile.Item.File.Trim()))
                            fileName = globalConfig.IgnoreFile.Item.File.Trim();
                        else
                            fileName = Path.Combine(_writer.Configuration.IgnoreFile.Item.ContentPath, globalConfig.IgnoreFile.Item.File.Trim());
                        _writer.Configuration.IgnoreFile.Item.File = globalConfig.IgnoreFile.Item.File.Trim();
                    }
                }
                if (File.Exists(fileName))
                    textBoxIgnoreFilePatterns.Text = File.ReadAllText(fileName);
            }
            textBoxIgnoreFilePatterns.Enabled = _isLocal;
        }

        private void setWorkflowSettings()
        {
            if (_isLocal)
                label1.Text = label1.Text.Replace("This is the global configuration for Mighty-Moose that will act as the base configuration for all instances. You can set up local configurations to override/merge with this configuration.", "This is the local configuration overriding the global configuration for this solution.");
            setMooseMode();
            if (_writer.Configuration.StartPaused.Exists && _writer.Configuration.StartPaused.Item)
                checkBoxStartPaused.Checked = true;
            _originalWorkflow = getMooseMode();

            if (_writer.Configuration.BuildSolution.Exists)
            {
                if (_writer.Configuration.BuildSolution.Item)
                    comboBoxBuildSetup.SelectedIndex = 0;
                else
                    comboBoxBuildSetup.SelectedIndex = 1;
            }
            else
                comboBoxBuildSetup.SelectedIndex = 0;

            if (_writer.Configuration.RealtimeFeedback.Exists)
            {
                checkBoxRealtimeFeedback.Checked = _writer.Configuration.RealtimeFeedback.Item;
            }
            var global = _isLocal ? new ConfigParser("").ParseGlobal() : _writer.Configuration;
            if (global.AllDisabled.Exists)
                checkBoxDisableAll.Checked = global.AllDisabled.Item;
            if (_writer.Configuration.MSBuildParallelBuildCount.Exists)
                textBoxParallelMSBuild.Text = _writer.Configuration.MSBuildParallelBuildCount.Item.ToString();
            else
                textBoxParallelMSBuild.Text = "1";
            setParallelBuildVisibility();
            if (_writer.Configuration.AnonFeedback.Exists)
                checkBoxAnalytics.Checked = _writer.Configuration.AnonFeedback.Item;
        }

        private MooseMode getMooseMode()
        {
            if (radioButtonMighty.Checked)
                return MooseMode.Mighty;
            if (radioButtonAuto.Checked)
                return MooseMode.Auto;
            if (radioButtonManual.Checked || checkBoxStartPaused.Checked)
                return MooseMode.Manual;
            return MooseMode.Mighty;
        }

        private void setMooseMode()
        {
            if (_isLocal)
                setLocalMooseMode();
            else
                setGlobalMooseMode(_writer.Configuration.MSBuild);
        }

        private void setLocalMooseMode()
        {
            if (_writer.Configuration.StartPaused.Exists && _writer.Configuration.StartPaused.Item)
            {
                setMooseModeControls(MooseMode.Manual);
                return;
            }
            if (_writer.Configuration.MSBuild.Where(x => x.Exists && x.ShouldExclude).Count() > 0)
            {
                setMooseModeControls(MooseMode.Auto);
                return;
            }
            if (_writer.Configuration.MSBuild.Where(x => x.Exists && File.Exists(x.Item.Path)).Count() > 0)
            {
                setMooseModeControls(MooseMode.Mighty);
                return;
            }

            var parser = new ConfigParser("");
            var globalConfig = parser.Parse();
            setGlobalMooseMode(globalConfig.MSBuild);
        }

        private void setMooseModeControls(MooseMode state)
        {
            radioButtonMighty.Checked = state == MooseMode.Mighty;
            radioButtonAuto.Checked = state == MooseMode.Auto;
            radioButtonManual.Checked = state == MooseMode.Manual;
        }

        private void setGlobalMooseMode(IEnumerable<ConfigItem<VersionConfig>> msbuild)
        {
            if (_writer.Configuration.StartPaused.Item)
            {
                setMooseModeControls(MooseMode.Manual);
                return;
            }
            if (msbuild.Where(x => x.Exists && File.Exists(x.Item.Path)).Count() > 0)
                setMooseModeControls(MooseMode.Mighty);
            else
                setMooseModeControls(MooseMode.Auto);
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
                textBoxIgnoreFile.Text = dialog.FileName;
        }

        private void radioButtonMighty_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            if (radioButtonMighty.Checked)
            {
                _writer.EnableMightyMoose();
                checkBoxRealtimeFeedback.Enabled = true;
                checkBoxStartPaused.Checked = false;
            }
            else
            {
                checkBoxRealtimeFeedback.Checked = false;
                checkBoxRealtimeFeedback.Enabled = false;
            }
        }

        private void radioButtonAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            if (radioButtonAuto.Checked)
            {
                _writer.EnableAutoMoose();
                checkBoxStartPaused.Checked = false;
            }
        }

        private void radioButtonManual_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            if (radioButtonManual.Checked)
            {
                _writer.EnableManualMoose();
                checkBoxStartPaused.Checked = true;
            }
        }

        private void checkBoxStartPaused_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.SetPausedAtStartup(checkBoxStartPaused.Checked);
        }

        private void textBoxIgnoreFile_TextChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            var file = textBoxIgnoreFile.Text.Trim();
            if (!_isLocal)
                _writer.SetIgnoreFilePath(file);
            else
                setLocalIgnoreFileSetting(file);
        }

        private void setLocalIgnoreFileSetting(string file)
        {
            var parser = new ConfigParser("");
            var globalConfig = parser.Parse();
            if (globalConfig.IgnoreFile.Exists && globalConfig.IgnoreFile.Item.File == file)
                _writer.SetIgnoreFilePath("");
            else
                _writer.SetIgnoreFilePath(file);

            if (_writer.Configuration.IgnoreFile.Exists && File.Exists(Path.Combine(_writer.Configuration.IgnoreFile.Item.ContentPath, file)))
                readPatterns(file);
        }

        private void readPatterns(string file)
        {
            try
            {
                textBoxIgnoreFilePatterns.Text = File.ReadAllText(Path.Combine(_writer.Configuration.IgnoreFile.Item.ContentPath, file));
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private void textBoxIgnoreFilePatterns_TextChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            if (textBoxIgnoreFile.Text.Trim() == "")
                textBoxIgnoreFile.Text = ".gitignore";
            _writer.SetIgnoreFilePatterns(textBoxIgnoreFilePatterns.Text);
        }

        private void buttonAddIgnoreAssembly_Click(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            if (textBoxIgnoreAssembly.Text.Trim().Length == 0)
                return;
            _writer.AddIgnoreAssembly(textBoxIgnoreAssembly.Text.Trim());
            listViewIgnoreAssembly.Items.Add(textBoxIgnoreAssembly.Text.Trim());
        }

        private void listViewIgnoreAssembly_KeyDown(object sender, KeyEventArgs e)
        {
            if (_isInitializing)
                return;
            if (e.KeyCode != Keys.Delete)
                return;
            if (listViewIgnoreAssembly.SelectedItems.Count != 1)
                return;
            _writer.RemoveIgnoreAssembly(listViewIgnoreAssembly.SelectedItems[0].Text);
            listViewIgnoreAssembly.SelectedItems[0].Remove();
        }

        private void buttonIgnoreCategory_Click(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            if (textBoxIgnoreCategory.Text.Trim().Length == 0)
                return;
            _writer.AddIgnoreCategory(textBoxIgnoreCategory.Text.Trim());
            listViewIgnoreCategory.Items.Add(textBoxIgnoreCategory.Text.Trim());
        }

        private void listViewIgnoreCategory_KeyDown(object sender, KeyEventArgs e)
        {
            if (_isInitializing)
                return;
            if (e.KeyCode != Keys.Delete)
                return;
            if (listViewIgnoreCategory.SelectedItems.Count != 1)
                return;
            _writer.RemoveIgnoreCategory(listViewIgnoreCategory.SelectedItems[0].Text);
            listViewIgnoreCategory.SelectedItems[0].Remove();
        }

        private void textBoxGrowlPath_TextChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.SetGrowlNotifyPath(textBoxGrowlPath.Text.Trim());
        }

        private void checkBoxNotifyOnRunStart_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.SetNotifyOnRunStarted(checkBoxNotifyOnRunStart.Checked);
        }

        private void checkBoxNotifyOnRunFinished_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.SetNotifyOnRunFinished(checkBoxNotifyOnRunFinished.Checked);
        }

        private void textBoxCustomOutput_TextChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.SetCustomOutputPath(textBoxCustomOutput.Text.Trim());
        }

        private void checkBoxMinimizerLogging_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.SetMinimizerMode(checkBoxMinimizerLogging.Checked);
        }

        private void checkBoxDebug_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.SetDebugMode(checkBoxDebug.Checked);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            saveDisableAll();
            saveDisableAnonFeedback();
            var writer = new ConfigWriter();
            var file = _writer.Configuration.ConfigurationFullpath;
            var content = writer.Transform(_writer.Configuration);
            saveConfiguration(file, content);
            notifyVM(file, content);
            Close();
        }

        private void saveDisableAll()
        {
            if (_disableAllOriginalValue == checkBoxDisableAll.Checked)
                return;
            if (!_isLocal)
            {
                _writer.Configuration.AllDisabled.Exists = true;
                _writer.Configuration.AllDisabled.Item = checkBoxDisableAll.Checked;
                return;
            }
            var config = new ConfigParser("").ParseGlobal();
            config.AllDisabled.Exists = true;
            config.AllDisabled.Item = checkBoxDisableAll.Checked;
            var content = new ConfigWriter().Transform(config);
            saveConfiguration(config.ConfigurationFullpath, content);
        }

        private void saveDisableAnonFeedback()
        {
            if (_anonFeedbackOriginalValue == checkBoxAnalytics.Checked)
                return;
            if (!_isLocal)
            {
                _writer.Configuration.AnonFeedback.Exists = true;
                _writer.Configuration.AnonFeedback.Item = checkBoxAnalytics.Checked;
                return;
            }
            var config = new ConfigParser("").ParseGlobal();
            config.AnonFeedback.Exists = true;
            config.AnonFeedback.Item = checkBoxDisableAll.Checked;
            var content = new ConfigWriter().Transform(config);
            saveConfiguration(config.ConfigurationFullpath, content);
        }

        private void notifyVM(string file, string content)
        {
            var requiresRestart = _originalWorkflow != getMooseMode() || feedbackViewSettingsChanged();
            if (needsVSRestart())
                MessageBox.Show("For the changes to take effect you need to restart Visual Studio", "Restart Visual Studio", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ConfigurationUpdateMessage message;
            if (_isLocal)
                message = new ConfigurationUpdateMessage(requiresRestart, "", "", file, content);
            else
                message = new ConfigurationUpdateMessage(requiresRestart, file, content, "", "");
            _vm.SendConfigurationUpdate(message);
        }

        private bool needsVSRestart()
        {
            if (_disableAllOriginalValue != checkBoxDisableAll.Checked)
                return true;
            var minimiser = _writer.Configuration.MinimizerLevel.Exists ? _writer.Configuration.MinimizerLevel.Item : null;
            if ((_originalMinimizer == "off" || minimiser == "off") && _originalMinimizer != minimiser)
                return true;
            var profiler = _writer.Configuration.ProfilerSetup.Exists ? _writer.Configuration.ProfilerSetup.Item : null;
            if (_originalProfiler != profiler)
                return true;
            if (_originalMinimizerDebug != checkBoxMinimizerLogging.Checked)
                return true;
            return false;
        }

        private bool feedbackViewSettingsChanged()
        {
            return checkBoxBuildErrors.Checked != _showErrorsInFeedbackview ||
                   checkBoxWarnings.Checked != _showWarningsInFeedbackview ||
                   checkBoxFailing.Checked != _showFailuresInFeedbackview ||
                   checkBoxIgnored.Checked != _showIgnoresInFeedbackview;
        }

        private static void saveConfiguration(string file, string content)
        {
            if (file.Length > 0)
            {
                if (File.Exists(file))
                {
                    var oldFile = file + ".old";
                    File.Delete(oldFile);
                    File.Move(file, oldFile);
                }
                File.WriteAllText(file, content);
            }
        }

        private void linkLabelOpenConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!File.Exists(_writer.Configuration.ConfigurationFullpath))
                File.WriteAllText(_writer.Configuration.ConfigurationFullpath, "");
            var process = new Process();
            process.StartInfo.FileName = _writer.Configuration.ConfigurationFullpath;
            process.Start();
            Close();
        }

        private void comboBoxGraphProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            _writer.Configuration.GraphProvider.Exists = true;
            _writer.Configuration.GraphProvider.IsLocal = _isLocal;
            _writer.Configuration.GraphProvider.ShouldExclude = false;
            _writer.Configuration.GraphProvider.ShouldMerge = false;
            if (comboBoxGraphProvider.SelectedIndex == 0)
                _writer.Configuration.GraphProvider.Item = GraphProviderConverter.FromEnum(GraphProvider.BUILTINDARK);
            if (comboBoxGraphProvider.SelectedIndex == 1)
                _writer.Configuration.GraphProvider.Item = GraphProviderConverter.FromEnum(GraphProvider.BUILTINLIGHT);
            if (comboBoxGraphProvider.SelectedIndex == 2)
                _writer.Configuration.GraphProvider.Item = GraphProviderConverter.FromEnum(GraphProvider.GRAPHVIZ);
            if (comboBoxGraphProvider.SelectedIndex == 3)
                _writer.Configuration.GraphProvider.Item = GraphProviderConverter.FromEnum(GraphProvider.DGML);
            if (comboBoxGraphProvider.SelectedIndex == 4)
                _writer.Configuration.GraphProvider.Item = GraphProviderConverter.FromEnum(GraphProvider.WINDOW);
        }

        private void checkBoxStartPaused_CheckedChanged_1(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.SetPausedAtStartup(checkBoxStartPaused.Checked);
        }

        private void checkBoxBuildErrors_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.ShowBuildErrorsInFeedbackWindow(checkBoxBuildErrors.Checked);
        }

        private void checkBoxWarnings_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.ShowBuildWarningsInFeedbackWindow(checkBoxWarnings.Checked);
        }

        private void checkBoxFailing_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.ShowFailingTestsInFeedbackWindow(checkBoxFailing.Checked);
        }

        private void checkBoxIgnored_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.ShowIgnoredTestsInFeedbackWindow(checkBoxIgnored.Checked);
        }

        private void buttonMAsmBrowse_Click(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            if (textBoxMinimizerAssembly.Text.Trim().Length == 0)
                return;
            _writer.AddMinimizerAssembly(textBoxMinimizerAssembly.Text.Trim());
            listViewMinimizerAssemblies.Items.Add(textBoxMinimizerAssembly.Text.Trim());
            textBoxMinimizerAssembly.Text = "";
        }

        private void listViewMinimizerAssemblies_KeyDown(object sender, KeyEventArgs e)
        {
            if (_isInitializing)
                return;
            if (e.KeyCode != Keys.Delete)
                return;
            if (listViewMinimizerAssemblies.SelectedItems.Count != 1)
                return;
            _writer.RemoveMinimizerAssembly(listViewMinimizerAssemblies.SelectedItems[0].Text);
            listViewMinimizerAssemblies.SelectedItems[0].Remove();
        }

        private void buttonMinAsmBrowse_Click(object sender, EventArgs e)
        {
            var open = new OpenFileDialog();
            open.Filter = "*(*.exe;*.dll)|*.exe;*.dll";
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBoxMinimizerAssembly.Text = open.FileName;
        }

        private void comboBoxProfiler_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            var setup = ProfilerSettings.RUNANDAUTODETECT;
            if (comboBoxProfiler.SelectedIndex == 0)
                setup = ProfilerSettings.RUN;
            if (comboBoxProfiler.SelectedIndex == 1)
                setup = ProfilerSettings.DONTRUN;
            if (comboBoxProfiler.SelectedIndex == 2)
                setup = ProfilerSettings.RUNANDAUTODETECT;
            _writer.SetProfilerSetup(setup);
        }

        private void buttonAddProfilerNamespace_Click(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            if (textBoxProfilerNamespace.Text.Trim().Length == 0)
                return;
            _writer.AddProfilerNamespace(textBoxProfilerNamespace.Text.Trim());
            listViewProfilerNamespaces.Items.Add(textBoxProfilerNamespace.Text.Trim());
            textBoxProfilerNamespace.Text = "";
        }

        private void listViewProfilerNamespaces_KeyDown(object sender, KeyEventArgs e)
        {
            if (_isInitializing)
                return;
            if (e.KeyCode != Keys.Delete)
                return;
            if (listViewProfilerNamespaces.SelectedItems.Count != 1)
                return;
            _writer.RemoveProfilerNamespace(listViewProfilerNamespaces.SelectedItems[0].Text);
            listViewProfilerNamespaces.SelectedItems[0].Remove();
        }

        private void comboBoxBuildSetup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            var solutionBuild = comboBoxBuildSetup.SelectedIndex == 0;
            _writer.SetBuildSetup(solutionBuild);
            setParallelBuildVisibility();
        }

        private void setParallelBuildVisibility()
        {
            var solutionBuild = comboBoxBuildSetup.SelectedIndex == 0;
            lblMSBuildParallelCount.Visible = solutionBuild;
            textBoxParallelMSBuild.Visible = solutionBuild;
        }

        private void checkBoxRealtimeFeedback_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.SetRealtimeFeedback(checkBoxRealtimeFeedback.Checked);
            if (checkBoxRealtimeFeedback.Checked && checkBoxNotifyOnRunStart.Checked)
                checkBoxNotifyOnRunStart.Checked = false;
        }

        private void comboBoxMinimizer_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBoxUseMargins.Visible = comboBoxMinimizer.SelectedIndex != 2;
            if (_isInitializing)
                return;
            var setup = "run";
            if (comboBoxMinimizer.SelectedIndex == 0)
                setup = "run";
            if (comboBoxMinimizer.SelectedIndex == 1)
                setup = "notrun";
            if (comboBoxMinimizer.SelectedIndex == 2)
                setup = "off";
            _writer.SetMinimizerLevel(setup);
        }

        private void checkBoxOverlayNotififations_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.SetOverlayNotifications(checkBoxOverlayNotififations.Checked);
        }

        private void checkBoxRunAssembliesInParallel_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.SetRunAssembliesInParallel(checkBoxRunAssembliesInParallel.Checked);
        }

        private void checkBoxCompatibilityMode_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;
            _writer.SetTestRunnerCompatibilityMode(checkBoxCompatibilityMode.Checked);
        }

        private void checkBoxUseMargins_CheckedChanged(object sender, EventArgs e)
        {
                if (_isInitializing)
                return;
            _writer.SetRiscEnabled(checkBoxUseMargins.Checked);
        }

        private void labelGlobalConfig_Click(object sender, EventArgs e)
        {
            if (_doWhatever != null)
                _doWhatever();
        }

        private void textBoxParallelMSBuild_TextChanged(object sender, EventArgs e)
        {
            int parallelRuns;
            if (int.TryParse(textBoxParallelMSBuild.Text, out parallelRuns))
                _writer.SetMSBuildParallelBuildCount(parallelRuns);
            else
                parallelRuns = 0;
            if (textBoxParallelMSBuild.Text != parallelRuns.ToString())
                textBoxParallelMSBuild.Text = parallelRuns.ToString();
        }
    }
}
