using System;
using System.IO;
using AutoTest.UI;
using AutoTest.Messages;

namespace AutoTest.Server.Handlers
{
	class StatusHandler : IHandler, IInternalMessageHandler
	{
        private Action<string, object> _dispatch;
        private bool _isRunning = false;

        public void DispatchThrough(Action<string, object> dispatcher) {
            _dispatch = dispatcher;
        }

        public void OnInternalMessage(object message) {
            if (message.Is<RunStartedMessage>()) {
                _isRunning = true;
                var x = (RunStartedMessage)message;
                send(ImageStates.Progress, "processing changes...");
                printMessage(RunMessageType.Normal, x.ToString());
                generateSummary(null);
            } else if (message.Is<RunFinishedMessage>()) {
                _isRunning = false;
                var x = (RunFinishedMessage)message;
                if (x.Report.Aborted) {
                    var i = getRunFinishedInfo(x);
                    var runType = i.Succeeded ? RunMessageType.Succeeded : RunMessageType.Failed;
                    send(runType == RunMessageType.Succeeded ? ImageStates.Green : ImageStates.Red, "");
                    printMessage(runType, i.Text);
                    generateSummary(i.Report);
                } else {
                    var i = getRunFinishedInfo(x);
                    var runType = i.Succeeded ? RunMessageType.Succeeded : RunMessageType.Failed;
                    send(runType == RunMessageType.Succeeded ? ImageStates.Green : ImageStates.Red, "");
                    printMessage(runType, i.Text);
                    generateSummary(i.Report);
                }
            } else if (message.Is<RunInformationMessage>()) {
                var x = (RunInformationMessage)message;
                if (!_isRunning)
                    return;
                var text = "";
                switch (x.Type)
                {
                    case InformationType.Build:
                        text = string.Format("building {0}", Path.GetFileName(x.Project));
                        break;
                    case InformationType.TestRun:
                        text = "testing...";
                        break;
                    case InformationType.PreProcessing:
                        text = "locating affected tests";
                        break;
                }
                if (text != "") {
                    send(ImageStates.Progress, text.ToString());
                    printMessage(RunMessageType.Normal, text.ToString());
                }
            } else if (message.Is<LiveTestStatusMessage>()) {
                if (!_isRunning)
                    return;

                var liveStatus = (LiveTestStatusMessage)message;
                var ofCount = liveStatus.TotalNumberOfTests > 0 ? string.Format(" of {0}", liveStatus.TotalNumberOfTests) : "";
                var testName = liveStatus.CurrentTest;
                if (testName.Trim().Length > 0)
                    testName += " in ";
                printMessage(RunMessageType.Normal, string.Format("testing {3}{0} ({1}{2} tests completed)", Path.GetFileNameWithoutExtension(liveStatus.CurrentAssembly), liveStatus.TestsCompleted, ofCount, testName));
            }
        }

        private void send(ImageStates state, string information) {
            _dispatch("picture-update", new {
                state = state.ToString().ToLower(),
                information = information
            });
        }

        private RunFinishedInfo getRunFinishedInfo(RunFinishedMessage message) {
            var report = message.Report;
            var text = string.Format(
                        "Ran {0} build(s) ({1} succeeded, {2} failed) and {3} test(s) ({4} passed, {5} failed, {6} ignored)",
                        report.NumberOfProjectsBuilt,
                        report.NumberOfBuildsSucceeded,
                        report.NumberOfBuildsFailed,
                        report.NumberOfTestsRan,
                        report.NumberOfTestsPassed,
                        report.NumberOfTestsFailed,
                        report.NumberOfTestsIgnored);
            var succeeded = !(report.NumberOfBuildsFailed > 0 || report.NumberOfTestsFailed > 0);
            return new RunFinishedInfo(text, succeeded, report);
        }

        private void printMessage(RunMessageType type, string message) {
            var normal = true;
            var color = "Black";

            if (type == RunMessageType.Succeeded)
            {
                color = "Green";
                normal = false;
            }
            if (type == RunMessageType.Failed)
            {
                color = "Red";
                normal = false;
            }

            _dispatch("status-information", new {
                message = message,
                color = color,
                normal = normal
            });
        }

        private void generateSummary(RunReport report)
        {
            if (report == null) {
                _dispatch("run-summary", new { message = "" });
                return;
            }

            var builder = new SummaryBuilder(report);
            _dispatch("run-summary", new { message = builder.Build() });
        }
	}
}