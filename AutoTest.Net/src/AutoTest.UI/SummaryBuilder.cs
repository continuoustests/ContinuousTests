using System;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.UI
{
    public class SummaryBuilder
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
