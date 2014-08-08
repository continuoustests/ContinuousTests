using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoTest.Client.UI
{
    public partial class SystemMessageForm : Form
    {
        private int _rightSpacing = 0;
        private int _listBottomSpacing = 0;
        private int _infoBottomSpacing = 0;

        public SystemMessageForm()
        {
            InitializeComponent();
        }

        public void AddInformation(string message)
        {
            addToList(message, Color.Black);
        }

        public void AddWarning(string message)
        {
            addToList(message, Color.DarkOrange);
        }

        public void AddError(string message)
        {
            addToList(message, Color.Red);
        }

        private void ATInternalMessagesForm_Resize(object sender, EventArgs e)
        {
            linkLabelInfo.MaximumSize = new Size(Width - (linkLabelInfo.Left + _rightSpacing), 0);
            linkLabelInfo.Top = Height - (linkLabelInfo.Height + _infoBottomSpacing);
            informationList.Height = linkLabelInfo.Top - (informationList.Top + _listBottomSpacing);
            informationList.Width = Width - (informationList.Left + _rightSpacing);
        }

        private void addToList(string text, Color color)
        {
            if (text.Length > 150)
                text = string.Format("{0}...", text.Substring(0, 150));
            var item = informationList.Items.Add(text);
            item.ForeColor = color;
            item.Tag = text;
        }

        private void informationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (informationList.SelectedItems.Count != 1)
            {
                setInfoText("");
                return;
            }
            setInfoText(informationList.Items[informationList.SelectedItems[0].Index].Tag.ToString());
        }

        private void setInfoText(string text)
        {
            int previousHeight = linkLabelInfo.Height;
            linkLabelInfo.Text = text;
            linkLabelInfo.LinkArea = new LinkArea(0, 0);
            var difference = linkLabelInfo.Height - previousHeight;
            Height = Height + difference;
        }

        private void ATInternalMessagesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }

        private void ATInternalMessagesForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Visible = false;
        }
    }
}
