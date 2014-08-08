using System;
using System.Runtime.InteropServices;
using System.Threading;
using AutoTest.Client.Config;
using EnvDTE80;
using AutoTest.Messages;
using System.IO;
using EnvDTE;
using AutoTest.Client.UI;

namespace AutoTest.VS.Listeners
{
    class StatusBarListener : IMessageListener
    {
        private readonly DTE2 _application;
        private MMConfiguration _config;
        private bool _isRunning;
        private readonly SynchronizationContext _context;
        
        public StatusBarListener(DTE2 application, MMConfiguration config)
        {
            _application = application;
            _config = config;
            _context = SynchronizationContext.Current;
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
            {
                if (!_config.RealtimeFeedback)
                    _application.StatusBar.Progress(true, "Run started...", 10, 100);
                _isRunning = true;
            }
            if (message.GetType().Equals(typeof(RunInformationMessage)))
            {
                if (!_isRunning)
                    return;
                var text = "";
                var msg = (RunInformationMessage)message;
                if (!_config.RealtimeFeedback)
                {
                    switch (msg.Type)
                    {
                        case InformationType.Build:
                            _application.StatusBar.Progress(true, "Building projects...", 30, 100);
                            text = string.Format("building {0}", Path.GetFileName(msg.Project));
                            break;
                        case InformationType.TestRun:
                            _application.StatusBar.Progress(true, "Testing assemblies...", 70, 100);
                            text = string.Format("testing {0}", Path.GetFileName(msg.Assembly));
                            break;
                    }
                }
            }
            if (message.GetType().Equals(typeof(RunFinishedMessage)))
            {
                var msg = message as RunFinishedMessage;
                string image = null;
                if (!_config.RealtimeFeedback)
                {
                    _application.StatusBar.Progress(false, "meh", 100, 100);
                    _application.StatusBar.Text = "";
                }
                if (!msg.Report.Aborted)
                {
                    var rnd = new Random();
                    var num = rnd.Next(0, 10);
                    _application.StatusBar.Text = GetFinishedMessage((RunFinishedMessage)message);
                    if (_config.CatMode)
                    {
                        image = msg.Report.NumberOfTestsFailed > 0 ? "catFAIL" + num + ".jpg" : "catWIN" + num + ".jpg";
                    }
                    else if (_config.OverlayNotifications)
                    {
                        image = msg.Report.NumberOfTestsFailed > 0 ? "circle_red.png" : "circle_green.png";
                    }
                    if (msg.Report.NumberOfTestsRan > 0 && image != null)
                        _context.Post(showWindow, image);
                }
                _isRunning = false;
            }
            if (message.GetType().Equals(typeof(MMConfiguration)))
                _config = (MMConfiguration)message;
        }

        private static void showWindow(object state)
        {

            var ptr = GetForegroundWindow();
            var str = (string) state;
            var win = new TransparentImageWindow(str);
            win.Show();
            SetFocus(new HandleRef(null, ptr));
        }
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetFocus(HandleRef hWnd);
        private static string GetFinishedMessage(RunFinishedMessage message)
        {
            var report = message.Report;
            var failed = report.NumberOfBuildsFailed > 0 || report.NumberOfTestsFailed > 0;
            var text = "";
            if (failed)
            {
                text = string.Format(
                            "Ran {0} build(s) ({1} succeeded, {2} failed) and {3} test(s) ({4} passed, {5} failed, {6} ignored)",
                            report.NumberOfProjectsBuilt,
                            report.NumberOfBuildsSucceeded,
                            report.NumberOfBuildsFailed,
                            report.NumberOfTestsRan,
                            report.NumberOfTestsPassed,
                            report.NumberOfTestsFailed,
                            report.NumberOfTestsIgnored);
                
            }
            else
            {
                text = string.Format(
                            "Ran {0} build(s) and {1} test(s)",
                            report.NumberOfProjectsBuilt,
                            report.NumberOfTestsRan);
            }
            var statusText = failed ? "FAILED: " : "Succeeded: ";
            return statusText + text;
        }
    }
}
