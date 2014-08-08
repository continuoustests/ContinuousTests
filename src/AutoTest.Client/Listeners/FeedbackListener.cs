using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoTest.Client.UI;
using AutoTest.UI;
using AutoTest.Client.Logging;
using AutoTest.VM.Messages;
using AutoTest.Messages;
using System.IO;
using System.Reflection;
using AutoTest.Client.HTTP;
namespace AutoTest.Client.Listeners
{
    public class LabelItembehaviour : IListItemBehaviour
    {
        private Label _label;

        public LabelItembehaviour(Label label) {
            _label = label;
        }

        public int Left { get { return _label.Left; } set { _label.Left = value; } }
        public int Width { get { return _label.Width; } set { _label.Width = value; } }
        public string Name { get { return _label.Name; } }
        public bool Visible { get { return _label.Visible; } set { _label.Visible = value; } }
    }

    public class FeedbackListener : IMessageListener, IMessageForwarder
    {
        enum RunStatus
        {
            Green,
            Red,
            Aborted
        }
        private FeedbackProvider _view;
        private RunStatus _lastTestRunStatus = RunStatus.Aborted;

        public FeedbackListener(FeedbackProvider view)
        {
            _view = view;
            /*_view
                .OnGoToReference(
                    file, line, column) => {
                        
                }*/
        }

        public void Connecting(int port, bool startPaused)
        {
            Analytics.SendEvent("EngineStarted");
            var connectPort = port;
            if (startPaused)
            {
                _view.PrintMessage(new RunMessages(RunMessageType.Normal, string.Format("Engine paused. Changes will not be detected automatically.", connectPort, Environment.NewLine)));
            }
            else
            {
                _view.PrintMessage(new RunMessages(RunMessageType.Normal, string.Format("Engine started and waiting for changes.", connectPort, Environment.NewLine)));
            }
        }

        public void Disconnecting(int port)
        {
            var connectPort = port;
            _view.PrintMessage(new RunMessages(RunMessageType.Normal, string.Format("Engine paused. Changes will not be detected automatically.", connectPort, Environment.NewLine)));
        }

        public void IncomingMessage(object message)
        {
            Logger.Write(string.Format("Handling {0}", message.GetType()));
            if (message.GetType().Equals(typeof(InvalidLicenseMessage)))
                _view.PrintMessage(new RunMessages(RunMessageType.Normal, "To start using ContinuousTests go to ContinuousTests->About and register the application."));
            else if (message.GetType().Equals(typeof(ValidLicenseMessage)))
                _view.PrintMessage(new RunMessages(RunMessageType.Normal, "Engine started and waiting for changes."));
            else if (message.GetType().Equals(typeof(AutoTest.Client.Config.MMConfiguration)))
            {
                var config = (AutoTest.Client.Config.MMConfiguration)message;
                _view.SetVisibilityConfiguration(
                    config.BuildErrorsInFeedbackWindow,
                    config.BuildWarningsInFeedbackWindow,
                    config.FailingTestsInFeedbackWindow,
                    config.BuildWarningsInFeedbackWindow);
                _view.ShowRunInformation = !config.RealtimeFeedback;
            }
            else if (message.GetType().Equals(typeof(ProfiledTestRunStarted)))
                _view.SetProgress(true, "Loading profiler information", Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "progress-light.gif"));
            else if (message.GetType().Equals(typeof(ProfilerCompletedMessage)))
                setProgressFinished();
            else if (message.GetType().Equals(typeof(RunStartedMessage)))
            {
                Analytics.SendEvent("RunStarted");
                _view.ConsumeMessage(message);
            }
            else if (message.GetType().Equals(typeof(RunFinishedMessage)))
            {
                sendAnalytics((RunFinishedMessage)message);
                _lastTestRunStatus = setRunStatus((RunFinishedMessage)message);
                _view.ConsumeMessage(message);
            }
            else
                _view.ConsumeMessage(message);
        }

        private void setProgressFinished()
        {
            if (_lastTestRunStatus == RunStatus.Green)
                _view.SetProgress(false, "", ImageStates.Green);
            else if (_lastTestRunStatus == RunStatus.Red)
                _view.SetProgress(false, "", ImageStates.Red);
            else if (_lastTestRunStatus == RunStatus.Aborted)
                _view.SetProgress(false, "", ImageStates.None);
        }

        private RunStatus setRunStatus(RunFinishedMessage message)
        {
            if (message.Report.Aborted)
            {
                return RunStatus.Aborted;
            }
            if (message.Report.NumberOfBuildsFailed == 0 && message.Report.NumberOfTestsFailed == 0)
            {
                return RunStatus.Green;
            }
            if (message.Report.NumberOfBuildsFailed == 0)
            {
                return RunStatus.Red;
            }
            return RunStatus.Red;
        }

        private void sendAnalytics(RunFinishedMessage message)
        {
            if (message.Report.Aborted)
            {
                Analytics.SendEvent("RunAborted");
            }
            else if (message.Report.NumberOfBuildsFailed == 0 && message.Report.NumberOfTestsFailed == 0)
            {
                Analytics.SendEvent("RunPassed");
            }
            else if (message.Report.NumberOfBuildsFailed > 0)
            {
                Analytics.SendEvent("BuildFailed");
            }
            else
            {
                Analytics.SendEvent("TestsFailed");
            }
        }

        public void Forward(object message)
        {
            IncomingMessage(message);
        }
    }
}
