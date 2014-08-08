using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoTest.Client.Config;

namespace AutoTest.Client.UI
{
    public enum LocalConfigOption
    {
        IsNotLocal,
        Override,
        Merge,
        Exclude
    }

    public class ItemAddedEventArgs : EventArgs
    {
        public string Path { get; private set; }
        public string Framework { get; private set; }

        public ItemAddedEventArgs(string path, string framework)
        {
            Path = path;
            Framework = framework;
        }
    }

    public class ItemRemovedEventArgs : EventArgs
    {
        public ConfigItem<VersionConfig> Item { get; private set; }

        public ItemRemovedEventArgs(ConfigItem<VersionConfig> item)
        {
            Item = item;
        }
    }

    public class LocalOverrideEventArgs : EventArgs
    {
        public LocalConfigOption Option { get; private set; }

        public LocalOverrideEventArgs(LocalConfigOption option)
        {
            Option = option;
        }
    }

    public partial class VersionedConfigOption : UserControl
    {
        private string _description = "";
        private string _additionalDescription = "";

        public event EventHandler<ItemAddedEventArgs> ItemAdded;
        public event EventHandler<ItemRemovedEventArgs> ItemRemoved;
        public event EventHandler<LocalOverrideEventArgs> LocalOverrideChanged;

        public void SetLocalConfigOption(LocalConfigOption option)
        {
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

        public string Description
        {
            get { return _description; }
            set { _description = value; updateLabel(); }
        }

        public string AdditionalDescription
        {
            get { return _additionalDescription; }
            set { _additionalDescription = value; updateLabel(); }
        }

        private void updateLabel()
        {
            var additional = _additionalDescription.Length > 0 ? " - " + _additionalDescription : "";
            labelDescription.Text = _description + additional;
        }

        public string[] Versions
        {
            get
            {
                var versions = new List<string>();
                foreach (var item in comboBoxVersions.Items)
                    versions.Add(item.ToString());
                return versions.ToArray();
            }
            set
            {
                comboBoxVersions.Items.Clear();
                foreach (var item in value)
                    comboBoxVersions.Items.Add(item);
                comboBoxVersions.Text = "";
            }
        }

        public VersionedConfigOption()
        {
            InitializeComponent();
        }

        public void Add(ConfigItem<VersionConfig> configItem, string additionalText)
        {
            var item = listViewItems.Items.Add(additionalText + configItem.Item.Path);
            item.SubItems.Add(configItem.Item.Framework);
            item.Tag = configItem;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
                textBoxPath.Text = dlg.FileName;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBoxPath.Text.Trim().Length == 0)
                return;
            if (ItemAdded != null)
                ItemAdded(this, new ItemAddedEventArgs(textBoxPath.Text, comboBoxVersions.Text));
        }

        private void listViewItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (listViewItems.SelectedItems.Count == 1 && e.KeyCode == Keys.Delete)
            {
                if (ItemRemoved != null)
                    ItemRemoved(this, new ItemRemovedEventArgs((ConfigItem<VersionConfig>) listViewItems.SelectedItems[0].Tag));
            }
        }

        public void Clear()
        {
            listViewItems.Items.Clear();
        }

        void radioButtonExclude_CheckedChanged(object sender, System.EventArgs e)
        {
            if (LocalOverrideChanged == null)
                return;

            if (radioButtonOverride.Checked)
                LocalOverrideChanged(this, new LocalOverrideEventArgs(LocalConfigOption.Override));
            if (radioButtonMerge.Checked)
                LocalOverrideChanged(this, new LocalOverrideEventArgs(LocalConfigOption.Merge));
            if (radioButtonExclude.Checked)
                LocalOverrideChanged(this, new LocalOverrideEventArgs(LocalConfigOption.Exclude));
        }
    }
}
