using System;
using System.Threading;
using System.Windows.Forms;
using AutoTest.Client.Logging;
using AutoTest.VM.Messages.License;
using AutoTest.Client.HTTP;

namespace AutoTest.Client.UI
{
    public partial class RegisterLicenseFrom : Form
    {
        private readonly Environment.SpecialFolder _appData;

        public RegisterLicenseFrom(Environment.SpecialFolder appData)
        {
            InitializeComponent();
            _appData = appData;
        }

        private void RegisterLicenseFrom_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
                return;
            Logger.Write("Registering licesnse");

            var name = textBoxName.Text;
            var email = textBoxEmail.Text;
            ThreadPool.QueueUserWorkItem((m) => {
                var validator = new RhinoValidator(_appData);
                validator.Register(name, email);

                if (validator.IsValid && passesTestQuestion(validator))
                    Casualties.ReportCasualty(string.Format("License registered{0}{1}{0}{2}", Environment.NewLine, name, email));
            }, null);
        }

        private bool passesTestQuestion(RhinoValidator validator)
        {
            return validator.Register("Lay it on me bro", null)
                .Equals("Honestly if you are so bad off that you have to hack this product to get it why didn't you just get in touch? " +
                        "We would probably have sponsored you with a license until you're back on track. " +
                        "Have some guts and be honest, don't just go off and steal everything you want");
        }

        private void linkLabelWWW_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Browse.To(linkLabelWWW.Text);
        }
    }
}
