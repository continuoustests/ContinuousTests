using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoTest.Client.UI
{
    public partial class ConfigOverrideControl : UserControl
    {
        private bool _isReading = false;

        public event EventHandler<LocalOverrideEventArgs> LocalOverrideChanged;

        public ConfigOverrideControl()
        {
            InitializeComponent();
        }

        public LocalConfigOption GetLocalConfigOption()
        {
            if (radioButtonOverride.Checked)
                return LocalConfigOption.Override;
            if (radioButtonMerge.Checked)
                return LocalConfigOption.Merge;
            if (radioButtonExclude.Checked)
                return LocalConfigOption.Exclude;
            return LocalConfigOption.IsNotLocal;
        }

        public void SetLocalConfigOption(LocalConfigOption option)
        {
            _isReading = true;
            radioButtonOverride.Visible = true;
            radioButtonMerge.Visible = true;
            radioButtonExclude.Visible = true;
            switch (option)
            {
                case LocalConfigOption.IsNotLocal:
                    radioButtonOverride.Visible = false;
                    radioButtonMerge.Visible = false;
                    radioButtonExclude.Visible = false;
                    break;
                case LocalConfigOption.Override:
                    radioButtonOverride.Checked = true;
                    break;
                case LocalConfigOption.Merge:
                    radioButtonMerge.Checked = true;
                    break;
                case LocalConfigOption.Exclude:
                    radioButtonExclude.Checked = true;
                    break;
            }
            _isReading = false;
        }

        private void radioButtonOverride_CheckedChanged(object sender, EventArgs e)
        {
            if (_isReading)
                return;
            if (LocalOverrideChanged != null)
                LocalOverrideChanged(this, new LocalOverrideEventArgs(LocalConfigOption.Override));
        }

        private void radioButtonMerge_CheckedChanged(object sender, EventArgs e)
        {
            if (_isReading)
                return;
            if (LocalOverrideChanged != null)
                LocalOverrideChanged(this, new LocalOverrideEventArgs(LocalConfigOption.Merge));
        }

        private void radioButtonExclude_CheckedChanged(object sender, EventArgs e)
        {
            if (_isReading)
                return;
            if (LocalOverrideChanged != null)
                LocalOverrideChanged(this, new LocalOverrideEventArgs(LocalConfigOption.Exclude));
        }
    }
}
