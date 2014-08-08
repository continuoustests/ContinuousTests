using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoTest.Client.Config;
using AutoTest.VM.Messages.Configuration;
using System.IO;
using AutoTest.VM.Messages;

namespace AutoTest.Client.UI
{
    partial class WarmupForm : Form
    {
        private ConfigurationFormWriter _global;
        private ConfigurationFormWriter _local;
        private Timer _beefTimer = new Timer();
        private MooseMode _originalMode;
        private VM _vm;

        public WarmupForm(string watchToken, VM vm)
        {
            InitializeComponent();
            _beefTimer.Interval = 1400;
            _beefTimer.Tick += new EventHandler(_beefTimer_Tick);
            _beefTimer.Start();
            _global = new ConfigurationFormWriter(false, new ConfigParser("").Parse(), new MSBuildLocator());
            _local = new ConfigurationFormWriter(true, new ConfigParser(watchToken).Parse(), new MSBuildLocator());
            _vm = vm;
            setGlobalMooseMode();
        }

        void _beefTimer_Tick(object sender, EventArgs e)
        {
            SuspendLayout();
            if (pictureBoxFlex.Visible)
            {
                pictureBoxRelax.Visible = true;
                pictureBoxFlex.Visible = false;
            }
            else
            {
                pictureBoxFlex.Visible = true;
                pictureBoxRelax.Visible = false;
            }
            ResumeLayout();
        }

        private void setGlobalMooseMode()
        {
            if (_global.Configuration.StartPaused.Item)
            {
                setMooseModeControls(MooseMode.Manual);
                return;
            }
            if (_global.Configuration.MSBuild.Where(x => x.Exists && File.Exists(x.Item.Path)).Count() > 0)
                setMooseModeControls(MooseMode.Mighty);
            else
                setMooseModeControls(MooseMode.Auto);
        }

        private void setMooseModeControls(MooseMode state)
        {
            radioButtonMighty.Checked = state == MooseMode.Mighty;
            radioButtonAuto.Checked = state == MooseMode.Auto;
            radioButtonManual.Checked = state == MooseMode.Manual;
            _originalMode = state;
        }

        private MooseMode getMooseMode()
        {
            if (radioButtonMighty.Checked)
                return MooseMode.Mighty;
            if (radioButtonAuto.Checked)
                return MooseMode.Auto;
            if (radioButtonManual.Checked || _global.Configuration.StartPaused.Item)
                return MooseMode.Manual;
            return MooseMode.Mighty;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            save();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            save();
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void save()
        {
            if (checkBoxNeverShow.Checked)
            {
                _global.SetIgnoreWarmup(true);
                save(_global, false);
            }
            var newMode = getMooseMode();
            if (_originalMode != newMode)
            {
                if (newMode == MooseMode.Mighty)
                    _local.EnableMightyMoose();
                else if (newMode == MooseMode.Auto)
                    _local.EnableAutoMoose();
                else if (newMode == MooseMode.Manual)
                    _local.EnableManualMoose();
            }

            if (checkBoxLowMemMode.Checked)
                _local.SetMinimizerLevel("off");

            if (_originalMode != newMode || checkBoxLowMemMode.Checked)
                save(_local, true);
        }

        private void save(ConfigurationFormWriter formWriter, bool isLocal)
        {
            var writer = new ConfigWriter();
            var file = formWriter.Configuration.ConfigurationFullpath;
            var content = writer.Transform(formWriter.Configuration);
            saveConfiguration(file, content);
            notifyVM(file, content, isLocal);
        }

        private void notifyVM(string file, string content, bool isLocal)
        {
            var requiresRestart = _originalMode != getMooseMode();
            ConfigurationUpdateMessage message;
            if (isLocal)
                message = new ConfigurationUpdateMessage(requiresRestart, "", "", file, content);
            else
                message = new ConfigurationUpdateMessage(requiresRestart, file, content, "", "");
            _vm.SendConfigurationUpdate(message);
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
    }
}
