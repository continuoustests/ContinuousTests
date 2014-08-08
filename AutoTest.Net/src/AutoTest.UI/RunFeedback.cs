using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using AutoTest.Messages;
using System.IO;
using AutoTest.UI.TextFormatters;

namespace AutoTest.UI
{
    public partial class RunFeedback : UserControl
    {
        private object _selected = null;
        private FeedbackProvider _provider;

        public void SetFeedbackProvider(FeedbackProvider provider) {
            _provider = provider;

            _provider
                .OnGoToReference(
                    (file,line,column) => {
                        if (GoToReference != null)
                            GoToReference(this, new GoToReferenceArgs(new CodePosition(file, line, column)));
                    })
                .OnGoToType(
                    (assembly,typename) => {
                        var args = new GoToTypeArgs(assembly, typename);
                        if (GoToType != null)
                            GoToType(this, args);
                        return args.Handled;
                    })
                .OnDebugTest(
                    (test) => {
                        if (DebugTest != null)
                            DebugTest(this, new DebugTestArgs(test));
                    })
                .OnCancelRun(
                    () => { 
                        if (CancelRun != null)
                            CancelRun(this, new EventArgs());
                    })
                .OnPrepareForFocus(
                    () => {
                        if (listViewFeedback.Items.Count > 0 && listViewFeedback.SelectedItems.Count == 0)
                            listViewFeedback.Items[0].Selected = true;
                        listViewFeedback.Select();
                        _provider.ReOrganize();
                    })
                .OnClearList(
                    () => {
                        if (!isWindows())
                            listViewFeedback.SelectedItems.Clear();
                        listViewFeedback.Items.Clear();
                    })
                .OnClearBuilds(
                    (project) => {
                        if (!isWindows())
                            listViewFeedback.SelectedItems.Clear();
                        foreach (ListViewItem listItem in listViewFeedback.Items) {
                            if (listItem.Tag.GetType() == typeof(CacheBuildMessage)) {
                                var item = (CacheBuildMessage)listItem.Tag;
                                if (project == null || item.Project.Equals(project))
                                    listViewFeedback.Items.Remove(listItem);
                            }
                        }
                    })
                .OnIsInFocus(
                    () => {
                        if (Focused)
                            return true;
                        return Controls.Cast<Control>().Any(control => control.Focused);
                    })
                .OnUpdatePicture(
                    (picture, state, information) => {
                        if (pictureBoxWorking.ImageLocation != picture)
                        {
                            pictureBoxWorking.ImageLocation = picture;
                            pictureBoxWorking.Refresh();
                        }
                        pictureBoxRed.Visible = state == ImageStates.Red;
                        pictureMoose.Visible = state == ImageStates.Green;
                        pictureBoxGray.Visible = state == ImageStates.None;
                        pictureBoxWorking.Visible = state == ImageStates.Progress;
                        _toolTipProvider.SetToolTip(pictureBoxWorking, information);
                    })
                .OnPrintMessage(
                    (msg, color, normal) => {
                        if (normal)
                            label1.Font = new Font(label1.Font, FontStyle.Regular);
                        else
                            label1.Font = new Font(label1.Font, FontStyle.Bold);

                        label1.Text = msg;
                        label1.ForeColor = Color.FromName(color);
                        label1.Refresh();
                    })
                .OnStoreSelected(
                    () => {
                        _selected = null;
                        if (listViewFeedback.SelectedItems.Count == 1)
                            _selected = listViewFeedback.SelectedItems[0].Tag;
                        listViewFeedback.SuspendLayout();
                    })
                .OnRestoreSelected(
                    (check) => {
                        foreach (ListViewItem item in listViewFeedback.Items) {
                            if (check(item.Tag, _selected)) {
                                item.Selected = true;
                                break;
                            }
                        }
                        listViewFeedback.ResumeLayout();
                        listViewFeedback.Refresh();
                    })
                .OnAddItem(
                    (type, message, color, tag) => {
                        string[] placeBefore = null;
                        if (type == "Build warning")
                            placeBefore = new[] { "Build warning", "Test ignored" };
                        else if (type == "Test failed")
                            placeBefore = new[] { "Test failed", "Build warning", "Test ignored" };
                        else if (type == "Test ignored")
                            placeBefore = new[] { "Test ignored" };
                        var position = 0;
                        if (placeBefore != null)
                            position = getFirstItemPosition(placeBefore);
                        var item = listViewFeedback.Items.Insert(position, type);
                        item.SubItems.Add(message);
                        item.ForeColor = Color.FromName(color);
                        item.Tag = tag;
                        /*if (isWindows()) {
                            if (selected != null && tag.GetType() == selected.GetType() && tag.Equals(selected))
                                item.Selected = true;
                        }*/
                    })
                .OnRemoveBuildItem(
                    (check) => {
                        foreach (ListViewItem item in listViewFeedback.Items) {
                            if (item.Tag.GetType() == typeof(CacheBuildMessage)) {
                                if (check((CacheBuildMessage)item.Tag)) {
                                    item.Remove();
                                    break;
                                }
                            }
                        }
                    })
                .OnRemoveTest(
                    (check) => {
                        foreach (ListViewItem item in listViewFeedback.Items) {
                            if (item.Tag.GetType() == typeof(CacheTestMessage)) {
                                if (check((CacheTestMessage)item.Tag)) {
                                    item.Remove();
                                    break;
                                }
                            }
                        }
                    })
                .OnSetSummary(
                    (m) => {
                        _toolTipProvider.SetToolTip(label1, m);
                    })
                .OnExists(
                    (check) => {
                        foreach (ListViewItem item in listViewFeedback.Items) {
                            if (check(item.Tag))
                                return true;
                        }
                        return false;
                    })
                .OnGetSelectedItem(
                    () => {
                        if (listViewFeedback.SelectedItems.Count == 0)
                            return null;
                        return listViewFeedback.SelectedItems[0].Tag;
                    })
                .OnGetWidth(() => this.Width);

            _provider.CanGoToTypes = CanGoToTypes;
            _provider.ShowRunInformation = ShowRunInformation;
            _provider.CanDebug = CanDebug;
            _provider.Initialize();
            _provider.PrintMessage(new UI.RunMessages(UI.RunMessageType.Normal, "Collecting source and project information..."));			
        }

        private TestDetailsForm _testDetails;
        
        // Handle these and properties below 
        public event EventHandler<GoToReferenceArgs> GoToReference;
        public event EventHandler<GoToTypeArgs> GoToType;
        public event EventHandler<DebugTestArgs> DebugTest;
        public event EventHandler CancelRun;

        public bool CanGoToTypes { get; set; }
        public bool ShowRunInformation { get; set; }
        public bool CanDebug { get; set; }
        public int ListViewWidthOffset { get; set; }

        public bool ShowIcon {
            get { return pictureMoose.Visible; }
            set {
                pictureMoose.Visible = value;
                label1.Left = pictureMoose.Left + pictureMoose.Width + 5;

            }
        }

        public RunFeedback() {
            InitializeComponent();
            if (ListViewWidthOffset > 0)
                listViewFeedback.Width = Width - ListViewWidthOffset;
        }

        public new void Resize() {
            if (_provider != null)
                _provider.ReOrganize();
        }

        private int getFirstItemPosition(string[] placeBefore)
        {
            int position = 0;
            foreach (ListViewItem listItem in listViewFeedback.Items)
            {
                if ((from p in placeBefore where p.Equals(listItem.Text) select p).Any())
                    return position;
                position++;
            }
            return position;
        }

        private bool isWindows() {
            return 
                Environment.OSVersion.Platform == PlatformID.Win32S ||
                Environment.OSVersion.Platform == PlatformID.Win32NT ||
                Environment.OSVersion.Platform == PlatformID.Win32Windows ||
                Environment.OSVersion.Platform == PlatformID.WinCE ||
                Environment.OSVersion.Platform == PlatformID.Xbox;
        }

        private void listViewFeedback_SelectedIndexChanged(object sender, EventArgs e)
        {
            _provider.ReOrganize();
        }

        private void listViewFeedback_DoubleClick(object sender, EventArgs e)
        {
            if (listViewFeedback.SelectedItems.Count != 1)
                return;

            var item = listViewFeedback.SelectedItems[0].Tag;
            _provider.GoTo(item);
        }

        private void linkLabelDebugTest_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (DebugTest == null)
                return;

            if (listViewFeedback.SelectedItems.Count != 1)
                return;
            if (listViewFeedback.SelectedItems[0].Tag.GetType() != typeof(CacheTestMessage))
                return;
            
            var test = (CacheTestMessage)listViewFeedback.SelectedItems[0].Tag;
            DebugTest(this, new DebugTestArgs(test));
        }

        private void listViewFeedback_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode.Equals(Keys.Enter))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    listViewFeedback_DoubleClick(this, new EventArgs());
                    return;
                }
                if (e.KeyCode.Equals(Keys.D))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    if (linkLabelDebugTest.Visible == true)
                        linkLabelDebugTest_LinkClicked(this, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link(0, 1)));
                    return;
                }

                if (e.KeyCode.Equals(Keys.I))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    if (linkLabelTestDetails.Visible)
                        linkLabelTestDetails_LinkClicked(this, new LinkLabelLinkClickedEventArgs(null));
                    else if (linkLabelErrorDescription.Visible)
                        linkLabelErrorDescription_LinkClicked(this, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link(0, 1)));
                    return;
                }

                if (e.KeyCode.Equals(Keys.A))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    if (linkLabelCancelRun.Visible)
                        linkLabelSystemMessages_LinkClicked(this, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link(0, 1)));
                    return;
                }
                if (e.KeyCode.Equals(Keys.K) || (e.Shift && e.KeyCode.Equals(Keys.F8)))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    SendKeys.Send("{UP}");
                    if (e.KeyCode.Equals(Keys.F8))
                        listViewFeedback_DoubleClick(this, new EventArgs());
                    return;
                }
                if (e.KeyCode.Equals(Keys.J) || (e.KeyCode.Equals(Keys.F8)))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    SendKeys.Send("{DOWN}");
                    if (e.KeyCode.Equals(Keys.F8))
                        listViewFeedback_DoubleClick(this, new EventArgs());
                }
            }
            catch
            {
                // Pft who logs!?
            }
        }

        private void linkLabelTestDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var item = (CacheTestMessage)listViewFeedback.SelectedItems[0].Tag;
                var builder = new DetailBuilder(item);
                var details = builder.Text;
                var links = builder.Links;
                if (CanGoToTypes)
                    links.Insert(0, new Link(details.IndexOf(item.Test.Name), details.IndexOf(item.Test.Name) + item.Test.Name.Length, item.Assembly, item.Test.Name));
                showDetailsWindow(details, "Test output", links, Screen.PrimaryScreen.WorkingArea.Width > 1024 ? Screen.PrimaryScreen.WorkingArea.Width - 500 : Screen.PrimaryScreen.WorkingArea.Width);
            }
            catch
            {
                // Pft who logs!?
            }
        }

        private void linkLabelErrorDescription_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var item = (CacheBuildMessage)listViewFeedback.SelectedItems[0].Tag;
                var builder = new DetailBuilder(item);
                var details = builder.Text;
                var links = builder.Links;
                showDetailsWindow(details, "Build output", links, Screen.PrimaryScreen.WorkingArea.Width > 1024 ? 1024 : Screen.PrimaryScreen.WorkingArea.Width);
            }
            catch
            {
                // Pft who logs!?
            }
        }

        private void showDetailsWindow(string message, string caption, List<Link> links, int maxWidth)
        {
            var hasTestDetails = _testDetails != null;
            var x = -1;
            var y = -1;
            if (hasTestDetails)
            {
                x = _testDetails.Left;
                y = _testDetails.Top;
            }

            _testDetails = new TestDetailsForm((file, line) => _provider.GoTo(file, line, 0), (assembly, type) => _provider.GoTo(assembly, type), x, y, message, caption, links, maxWidth);
            _testDetails.Show();
            _testDetails.BringToFront();
        }

        private void linkLabelSystemMessages_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (CancelRun != null)
                CancelRun(this, new EventArgs());
        }

        private void RunFeedback_Resize(object sender, EventArgs e)
        {
            _provider.ReOrganize();
        }

        private void listViewFeedback_Resize(object sender, EventArgs e)
        {
            listViewFeedback.Columns[1].Width = listViewFeedback.Width - (listViewFeedback.Columns[0].Width + 25);
        }
    }


    public class GoToReferenceArgs : EventArgs
    {
        public CodePosition Position { get; private set; }
        public GoToReferenceArgs(CodePosition position) { Position = position; }
    }

    public class GoToTypeArgs : EventArgs
    {
        public bool Handled = true;
        public string Assembly { get; private set; }
        public string TypeName { get; private set; }
        public GoToTypeArgs(string assembly, string typename) { Assembly = assembly; TypeName = typename; }
    }

    public class DebugTestArgs : EventArgs
    {
        public CacheTestMessage Test { get; private set; }
        public DebugTestArgs(CacheTestMessage test) { Test = test; }
    }
}
