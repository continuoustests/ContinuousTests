using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using AutoTest.Client.Logging;

namespace AutoTest.Client.UI
{
    public partial class AboutForm : Form
    {
        private ATEClient _client;

        public AboutForm(ATEClient client)
        {
            InitializeComponent();
            try
            {
                _client = client;
                linkLabelRegister.Visible = !_client.IsRegistered();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            try
            {
                label1.BackColor = Color.Transparent;
                label2.BackColor = Color.Transparent;
                label3.BackColor = Color.Transparent;
                label4.BackColor = Color.Transparent;
                labelVersion.BackColor = Color.Transparent;
                linkLabelWWW.BackColor = Color.Transparent;
                linkLabelRegister.BackColor = Color.Transparent;
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                labelVersion.Text = string.Format("v{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

        }

        private void linkLabelRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Visible = false;
                _client.ShowLicenseRegistration();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            Close();
        }

        private void linkLabelWWW_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo(linkLabelWWW.Text);
            process.Start();
        }

        private void linkLabelLicense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void labelVersion_Click(object sender, EventArgs e)
        {

        }
    }
}
