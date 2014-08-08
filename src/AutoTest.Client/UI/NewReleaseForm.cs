using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoTest.Client.VersionCheck;
using System.Reflection;
using System.Diagnostics;
using AutoTest.Client.Config;
using AutoTest.VM.Messages.Configuration;
using System.IO;

namespace AutoTest.Client.UI
{
    public partial class NewReleaseForm : Form
    {
        private string _newVersion;

        public NewReleaseForm(DidWeReleaseYet releaseInfo)
        {
            InitializeComponent();
            _newVersion = releaseInfo.Version;
            if (_newVersion == null)
                _newVersion = "";
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            labelTitle.Text = string.Format("Version {0} of ContinuousTests is Now Available (Installed {1}.{2}.{3})", releaseInfo.Version, version.Major, version.Minor, version.Build);
            textBox1.Text = releaseInfo.ReleaseNotes;
            textBox1.SelectionStart = 0;
            textBox1.SelectionLength = 0;
            button1.Left = -1000;
            button1.Select();
            comboBoxCheckFrequency.SelectedIndex = 0;
        }

        private void linkLabelDownloadlink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo("http://www.continuoustests.com/download.html");
            process.Start();
            Close();
        }

        private void comboBoxCheckFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCheckFrequency.SelectedIndex == 0)
                write("");
            else if (comboBoxCheckFrequency.SelectedIndex == 1)
                write(_newVersion);
            else if (comboBoxCheckFrequency.SelectedIndex == 2)
                write("all");
        }

        private void write(string text)
        {
            var config = new ConfigurationFormWriter(false, new ConfigParser("").Parse(), new MSBuildLocator());
            config.SetIgnoreThisUpgrade(text);

            var writer = new ConfigWriter();
            var file = config.Configuration.ConfigurationFullpath;
            var content = writer.Transform(config.Configuration);
            saveConfiguration(file, content);
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
