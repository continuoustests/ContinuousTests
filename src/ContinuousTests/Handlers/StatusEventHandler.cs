using System;
using System.IO;
using System.Text;
using AutoTest.UI;
using AutoTest.Messages;

namespace ContinuousTests.Handlers
{
    class StatusEventHandler : Handler
    {
        private bool _isRunning = false; 

        public override void OnMessage(object message) {
            if (isType<RunStartedMessage>(message)) {
                _isRunning = true;
                var x = (RunStartedMessage)message;
                send(ImageStates.Progress, "processing changes...");
                printMessage(RunMessageType.Normal, x.ToString());
                generateSummary(null);
            } else if (isType<RunFinishedMessage>(message)) {
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
            } else if (isType<RunInformationMessage>(message)) {
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
            } else if (isType<LiveTestStatusMessage>(message)) {
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

    class SummaryBuilder
    {
        private readonly RunReport _report;

        public SummaryBuilder(RunReport report)
        {
            _report = report;
        }

        public string Build()
        {
            var builder = new StringBuilder();
            foreach (var action in _report.RunActions)
                addLinePrAction(action, builder);
            addTotalLine(_report.RealTimeSpent, builder);
            return builder.ToString();
        }

        private void addLinePrAction(RunAction action, StringBuilder builder)
        {
            var state = action.Succeeded ? "succeeded" : "failed";
            var project = Path.GetFileName(action.Project);
            var timeSpent = action.TimeSpent;
            addLineByType(action, timeSpent, builder, state, project);
        }

        private void addLineByType(RunAction action, TimeSpan timeSpent, StringBuilder builder, string state, string project)
        {
            switch (action.Type)
            {
                case InformationType.Build:
                    addBuildLine(timeSpent, builder, state, project);
                    break;
                case InformationType.TestRun:
                    addTestLine(action, timeSpent, builder, state, project);
                    break;
            }
        }

        private void addBuildLine(TimeSpan timeSpent, StringBuilder builder, string state, string project)
        {
            builder.AppendLine(string.Format("{1} build {0} ({2},{3} sec)",
                                             state,
                                             project,
                                             timeSpent.Seconds.ToString(),
                                             timeSpent.Milliseconds.ToString()));
        }

        private void addTestLine(RunAction action, TimeSpan timeSpent, StringBuilder builder, string state, string project)
        {
            var assembly = Path.GetFileName(action.Assembly);
            builder.AppendLine(string.Format("Test run for assembly {1} ({2}) {0} ({3},{4} sec)",
                                             state,
                                             assembly,
                                             project,
                                             timeSpent.Seconds.ToString(),
                                             timeSpent.Milliseconds.ToString()));
        }

        private void addTotalLine(TimeSpan total, StringBuilder builder)
        {
            builder.AppendLine(string.Format("Finished {0} steps in {1}:{2}:{3}.{4}",
                                             _report.RunActions.Length,
                                             getPaddedString(total.Hours),
                                             getPaddedString(total.Minutes),
                                             getPaddedString(total.Seconds),
                                             total.Milliseconds));
        }

        private string getPaddedString(int number)
        {
            return number.ToString().PadLeft(2, '0');
        } 
    }
}
