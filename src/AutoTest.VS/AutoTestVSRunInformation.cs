using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using EnvDTE80;
using EnvDTE;
using AutoTest.Client.Logging;
using AutoTest.Client.Handlers;
using AutoTest.Client.UI;
using AutoTest.VS.Util;
using AutoTest.Client.Listeners;
using AutoTest.UI;
using AutoTest.VS.Util.Debugger;
using ContinuousTests.VS;
using AutoTest.VS.Util.Navigation;

namespace AutoTest.VS
{
    [ProgId("AutoTestVSRunInformationToolbox"), ClassInterface(ClassInterfaceType.AutoDual), Guid("67663444-f874-401c-9e55-053aa0b5bd0b")]
    public partial class AutoTestVSRunInformation : UserControl
    {
        private DTE2 _application;
        private IMessageListener _listener;
        private readonly FeedbackProvider _provider;

        public AutoTestVSRunInformation()
        {
            InitializeComponent();
            runFeedback.CanGoToTypes = true;

            _provider = new FeedbackProvider(
                new LabelItembehaviour(runFeedback.linkLabelCancelRun),
                new LabelItembehaviour(runFeedback.linkLabelDebugTest),
                new LabelItembehaviour(runFeedback.linkLabelTestDetails),
                new LabelItembehaviour(runFeedback.linkLabelErrorDescription));
            runFeedback.SetFeedbackProvider(_provider);

            _listener = new FeedbackListener(_provider);
            Connect.AddListener(_listener);
        }

        public void SetApplication(DTE2 application)
        {
            _application = application;
        }

        public void Connecting(int port, bool startPaused)
        {
            _listener.Connecting(port, startPaused);
        }

        public void Disconnecting(int port)
        {
            _listener.Disconnecting(port);
        }

        public void PrepareForFocus()
        {
            _provider.PrepareForFocus();
        }

        public void NotifyAboutUpcomingRecursiveRun()
        {
            _provider.PrintMessage(new RunMessages(RunMessageType.Normal, "To trigger the recursive run detection make a change and trigger a normal run"));
        }

        public void ClearList()
        {
            _provider.ClearList();
        }

        public void ClearBuilds()
        {
            _provider.ClearBuilds();
        }

        public void ClearBuilds(string project)
        {
            _provider.ClearBuilds(project);
        }

        public bool IsInFocus()
        {
            return _provider.IsInFocus();
        }

        public void IncomingMessage(object message)
        {
            _listener.IncomingMessage(message);
        }

        private void runFeedback_DebugTest(object sender, DebugTestArgs e)
        {
            Connect.Debug(_application, e.Test);   
        }

        private void runFeedback_GoToReference(object sender, GoToReferenceArgs e)
        {
            goToReference(e.Position.File, e.Position.LineNumber, e.Position.Column);
        }

        private void runFeedback_ShowSystemWindow(object sender, EventArgs e)
        {
            try
            {
                Connect.ShowMessageForm();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
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

        private void runFeedback_GoToType(object sender, GoToTypeArgs e)
        {
            try
            {
                new TypeNavigation().GoToType(_application, e.Assembly, e.TypeName);
            }
            catch (Exception ex)
            {
                e.Handled = false;
                Logger.Write(ex);
            }
        }

        private void AutoTestVSRunInformation_Resize(object sender, EventArgs e)
        {
            runFeedback.Resize();
        }

        private void runFeedback_CancelRun(object sender, EventArgs e)
        {
            Connect.AbortRun();
        }
    }
}
