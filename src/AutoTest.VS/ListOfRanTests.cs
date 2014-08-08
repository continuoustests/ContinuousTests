using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AutoTest.Client.UI;
using AutoTest.Messages;
using AutoTest.VS.ClientHandlers;
using System.Threading;
using AutoTest.Client.Logging;
using AutoTest.VS.Util;
using EnvDTE;
using EnvDTE80;
using AutoTest.UI;
using ContinuousTests.VS;
using AutoTest.VS.Util.Debugger;
using AutoTest.UI.TextFormatters;
using AutoTest.VS.Resources;
using AutoTest.VS.Util.Navigation;

namespace AutoTest.VS
{
    [ProgId("ContinuousTests_ListOfRanTests"), ClassInterface(ClassInterfaceType.AutoDual), Guid("67663444-f874-401c-9e55-053bb0b5bd0b")]
    public partial class ContinuousTests_ListOfRanTests : UserControl, IMessageListener
    {
        private SynchronizationContext _syncContext;
        private TestDetailsForm _testDetails;
        private bool _newRunInitialized = false;
        private object _padLock = new object();
        private DTE2 _application;
        private int _testCount = 0;

        public ContinuousTests_ListOfRanTests()
        {
            _syncContext = SynchronizationContext.Current;
            InitializeComponent();
            imageList.Images.Add(ResourceReader.GetImage("circleWIN.png"));
            imageList.Images.Add(ResourceReader.GetImage("circleWAIL.png"));
            imageList.Images.Add(ResourceReader.GetImage("circleFAIL.png"));
            StartupHandler.AddListener(this);
        }

        public void SetApplication(DTE2 application)
        {
            _application = application;
        }

        public void Connecting(int port, bool startPaused)
        {
        }

        public void Disconnecting(int port)
        {
        }

        public void IncomingMessage(object message)
        {
            if (message.GetType().Equals(typeof(RunStartedMessage)))
                _newRunInitialized = true;
            if (message.GetType().Equals(typeof(TestRunMessage)))
                updateView((TestRunMessage)message);
        }

        private void updateView(TestRunMessage message)
        {
            _syncContext.Post((s) =>
                {
                    lock (_padLock)
                    {
                        if (_newRunInitialized == true)
                        {
                            treeViewLastRun.Nodes.Clear();
                            _testCount = 0;
                        }

                        message.Results.All
                            .Select(x => new CacheTestMessage(message.Results.Assembly, x))
                            .GroupBy(x => getFixture(x)).ToList()
                            .ForEach(x =>
                                {
                                    var node = treeViewLastRun.Nodes.Add(string.Format("{0} ({1}) - {2} ms", x.Key, x.Count(), x.Sum(y => y.Test.TimeSpent.TotalMilliseconds)));
                                    x.ToList()
                                        .ForEach(y =>
                                            {
                                                var child = node.Nodes.Add(string.Format("{0} ({1}) - {2} ms", getTestName(y.Test.DisplayName), y.Test.Runner, y.Test.TimeSpent.TotalMilliseconds));
                                                child.ImageIndex = getImageIndex(y.Test.Status);
                                                child.SelectedImageIndex = child.ImageIndex;
                                                child.Tag = y;
                                            });
                                    node.ImageIndex = x.Max(y => getImageIndex(y.Test.Status));
                                    node.SelectedImageIndex = node.ImageIndex;
                                    if (radioButtonExpanded.Checked)
                                        node.ExpandAll();
                                    _testCount += x.Count();
                                });
                        labelTestCount.Text = _testCount.ToString();
                        _newRunInitialized = false;
                    }
                }, null);
        }

        private string getTestName(string displayName)
        {
            var end = displayName.LastIndexOf('.');
            if (end == -1)
                return displayName;
            return displayName.Substring(end + 1, displayName.Length - (end + 1));
        }

        private int getImageIndex(TestRunStatus testRunStatus)
        {
            switch (testRunStatus)
            {
                case TestRunStatus.Passed:
                    return 0;
                case TestRunStatus.Ignored:
                    return 1;
                case TestRunStatus.Failed:
                    return 2;
            }
            return 2;
        }

        private string getFixture(CacheTestMessage x)
        {
            if (x.Test.Runner == TestRunner.MSpec)
                return x.Test.Name;
            var end = x.Test.Name.LastIndexOf('.');
            if (end == -1)
                return x.Test.Name;
            return x.Test.Name.Substring(0, end);
        }

        public bool IsInFocus()
        {
            if (Focused)
                return true;
            foreach (Control control in Controls)
            {
                if (control.Focused)
                    return true;
            }
            return false;
        }

        private void treeViewLastRun_DoubleClick(object sender, EventArgs e)
        {
            if (treeViewLastRun.SelectedNode == null)
                return;

            var item = treeViewLastRun.SelectedNode.Tag;
            if (item == null)
                return;

            if (item.GetType().Equals(typeof(CacheTestMessage)))
            {
                var test = (CacheTestMessage)item;
                goToType(test.Assembly, test.Test.Name);
            }
        }

        private bool goToType(string assembly, string typeName)
        {
            try
            {
                new TypeNavigation().GoToType(_application, assembly, typeName);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            return false;
        }

        private void goToReference(string file, int lineNumber, int column)
        {
            try
            {
                var window = _application.OpenFile(EnvDTE.Constants.vsViewKindCode, file);
                window.Activate();
                var selection = (TextSelection)_application.ActiveDocument.Selection;
                selection.MoveToDisplayColumn(lineNumber, column, false);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private void treeViewLastRun_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode.Equals(Keys.Enter))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    treeViewLastRun_DoubleClick(this, new EventArgs());
                    return;
                }
                if (e.KeyCode.Equals(Keys.D))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    if (isFailedOrIgnoedTest())
                        debugTest();
                    return;
                }

                if (e.KeyCode.Equals(Keys.I))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    if (isFailedOrIgnoedTest())
                        runInformationForm();
                    return;
                }

                if (e.KeyCode.Equals(Keys.K) || (e.Shift && e.KeyCode.Equals(Keys.F8)))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    System.Windows.Forms.SendKeys.Send("{UP}");
                    if (e.KeyCode.Equals(Keys.F8))
                        treeViewLastRun_DoubleClick(this, new EventArgs());
                    return;
                }
                if (e.KeyCode.Equals(Keys.J) || e.KeyCode.Equals(Keys.F8))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    System.Windows.Forms.SendKeys.Send("{DOWN}");
                    if (e.KeyCode.Equals(Keys.F8))
                        treeViewLastRun_DoubleClick(this, new EventArgs());
                    return;
                }

                if (e.KeyCode.Equals(Keys.H))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    System.Windows.Forms.SendKeys.Send("{LEFT}");
                    return;
                }

                if (e.KeyCode.Equals(Keys.L))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    System.Windows.Forms.SendKeys.Send("{RIGHT}");
                    return;
                }
            }
            catch
            {
                // Pft who logs!?
            }

        }

        private void debugTest()
        {
            var test = (CacheTestMessage)treeViewLastRun.SelectedNode.Tag;
            if (test == null)
                return;
            Connect.Debug(_application, test);
        }

        private void runInformationForm()
        {
            try
            {
                var item = (CacheTestMessage)treeViewLastRun.SelectedNode.Tag;
                var builder = new DetailBuilder(item);
                var details = builder.Text;
                var links = builder.Links;
                links.Insert(0, new AutoTest.UI.Link(details.IndexOf(item.Test.Name), details.IndexOf(item.Test.Name) + item.Test.Name.Length, item.Assembly, item.Test.Name));
                showDetailsWindow(details, "Test output", links, Screen.PrimaryScreen.WorkingArea.Width > 1024 ? Screen.PrimaryScreen.WorkingArea.Width - 500 : Screen.PrimaryScreen.WorkingArea.Width);
            }
            catch
            {
            }
        }

        private void showDetailsWindow(string message, string caption, List<AutoTest.UI.Link> links, int maxWidth)
        {
            var hasTestDetails = _testDetails != null;
            var x = -1;
            var y = -1;
            if (hasTestDetails)
            {
                x = _testDetails.Left;
                y = _testDetails.Top;
            }

            _testDetails = new TestDetailsForm((file, line) => goToReference(file, line, 0), goToType, x, y, message, caption, links, maxWidth);
            _testDetails.Show();
            _testDetails.BringToFront();
        }

        private bool isFailedOrIgnoedTest()
        {
            if (treeViewLastRun.SelectedNode == null)
                return false;
            return true;
        }

        private void radioButtonExpanded_CheckedChanged(object sender, EventArgs e)
        {
            treeViewLastRun.ExpandAll();
        }

        private void radioButtonCollapsed_CheckedChanged(object sender, EventArgs e)
        {
            treeViewLastRun.CollapseAll();
        }
    }
}
