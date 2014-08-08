using System;
using System.ComponentModel;
using System.Windows.Forms;
using AutoTest.Client.Handlers;
using AutoTest.VM.Messages;
using AutoTest.Client.UI;
using AutoTest.Messages;
using System.Threading;
using AutoTest.Client.Listeners;
using AutoTest.UI;

namespace ContinuousTests
{
    public partial class RunFeedbackForm : Form, IStartupHandler
    {
        private readonly SynchronizationContext _syncContext;
        private readonly IMessageListener _listener;
        private readonly FeedbackProvider _provider;

        public RunFeedbackForm(string path)
        {
            InitializeComponent();
            Text += " - " + path;
            runFeedback1.ShowIcon = false;

            _provider = new FeedbackProvider(
                new LabelItembehaviour(runFeedback1.linkLabelCancelRun),
                new LabelItembehaviour(runFeedback1.linkLabelDebugTest),
                new LabelItembehaviour(runFeedback1.linkLabelTestDetails),
                new LabelItembehaviour(runFeedback1.linkLabelErrorDescription));
            runFeedback1.SetFeedbackProvider(_provider);

            _listener = new FeedbackListener(_provider);
            _syncContext = AsyncOperationManager.SynchronizationContext;
            RunFeedbackFormResize(this, new EventArgs());
            UpdateMenu(false);
        }

        public void VMStarted(VMSpawnedMessage message)
        {
            UpdateMenu(true);
        }

        public void Connecting(int port, bool startPaused)
        {
            _listener.Connecting(port, startPaused);
            UpdateMenu();
        }

        public void Disconnecting(int port)
        {
            _listener.Disconnecting(port);
            UpdateMenu(false);
        }

        public void Consume(object message)
        {
            _listener.IncomingMessage(message);
            if (message.GetType().Equals(typeof(ExternalCommandMessage)))
            {
                var commandMessage = (ExternalCommandMessage)message;
                if (commandMessage.Sender == "EditorEngine")
                {
                    var msg = EditorEngineMessage.New(commandMessage.Sender + " " + commandMessage.Command);
                    if (msg.Arguments.Count == 1 &&
                        msg.Arguments[0].ToLower() == "shutdown")
                    {
                        Close();
                    }
                    if (msg.Arguments.Count == 2 &&
                        msg.Arguments[0].ToLower() == "autotest.net" &&
                        msg.Arguments[1].ToLower() == "setfocus")
                    {
                        Activate();
                        _provider.PrepareForFocus();
                    }
                }
            }
        }

        private void runFeedback1_GoToReference(object sender, GoToReferenceArgs e)
        {
            Program.Client.GoTo(e.Position.File, e.Position.LineNumber, e.Position.Column);
        }

        private void RunFeedbackFormResize(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                runFeedback1.Width = Width - (runFeedback1.Left * 2) - 10;
                runFeedback1.Height = Height - (runFeedback1.Top * 2) - 30;
            }
            runFeedback1.Resize();
        }

        private void RunFeedbackFormActivated(object sender, EventArgs e)
        {
            _provider.PrepareForFocus();
        }

        private void ResumeEngineToolStripMenuItemClick(object sender, EventArgs e)
        {
            Program.Client.ResumeEngine();
            UpdateMenu(true);
        }

        private void PauseEngineToolStripMenuItemClick(object sender, EventArgs e)
        {
            Program.Client.PauseEngine();
            UpdateMenu(false);
        }

        private void BuildAndTestAllToolStripMenuItem1Click(object sender, EventArgs e)
        {
            Program.Client.RunAll();
        }

        private void DetectRecursiveBuildsToolStripMenuItemClick(object sender, EventArgs e)
        {
            Program.Client.RunRecursiveRunDetection();
        }

        private void GlobalToolStripMenuItemClick(object sender, EventArgs e)
        {
            Program.Client.ShowConfiguration(false);
        }

        private void LocalToolStripMenuItemClick(object sender, EventArgs e)
        {
            Program.Client.ShowConfiguration(true);
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            Program.Client.ShowAboutBox();
        }

        private void UpdateMenu()
        {
            if (Program.Client == null)
            {
                UpdateMenu(false);
                return;
            }
            UpdateMenu(Program.Client.IsRunning);
        }

        private void UpdateMenu(object running)
        {
            _syncContext.Post((x) =>
                {
                    var isRunning = (bool)x;
                    resumeEngineToolStripMenuItem.Enabled = !isRunning;
                    pauseEngineToolStripMenuItem.Enabled = isRunning;
                    buildAndTestAllToolStripMenuItem1.Enabled = isRunning;
                    toolStripMenuItemClearList.Enabled = isRunning;
                    detectRecursiveBuildsToolStripMenuItem.Enabled = isRunning;
                    globalToolStripMenuItem.Enabled = true;
                    localToolStripMenuItem.Enabled = true;
                    aboutToolStripMenuItem.Enabled = true;
                }, running);
        }

        private void toolStripMenuItemClearList_Click(object sender, EventArgs e)
        {
            PauseEngineToolStripMenuItemClick(this, new EventArgs());
            Thread.Sleep(500);
            ResumeEngineToolStripMenuItemClick(this, new EventArgs());
        }

        private void runFeedback1_CancelRun(object sender, EventArgs e)
        {
            Program.Client.AbortRun();
        }
    }
}
