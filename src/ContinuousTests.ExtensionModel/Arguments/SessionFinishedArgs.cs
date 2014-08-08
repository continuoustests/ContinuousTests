using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.UI;

namespace ContinuousTests.ExtensionModel.Arguments
{
    public class SessionFinishedArgs : EventArgs
    {
        public string Text { get; private set; }
        public bool Succeeded { get; private set; }
        public Run Run { get; private set; }

        public SessionFinishedArgs(string text, bool succeeded, Run run)
        {
            Text = text;
            Succeeded = succeeded;
            Run = run;
        }

        internal static SessionFinishedArgs FromInternal(RunFinishedInfo info)
        {
            return new SessionFinishedArgs(info.Text, info.Succeeded, getRun(info.Report));
        }

        private static Run getRun(AutoTest.Messages.RunReport runReport)
        {
            return new Run()
                {
                    RunDuration = runReport.RealTimeSpent,
                    NumberOfBuildsSucceeded = runReport.NumberOfBuildsSucceeded,
                    NumberOfBuildsFailed = runReport.NumberOfBuildsFailed,
                    NumberOfProjectsBuilt = runReport.NumberOfProjectsBuilt,
                    NumberOfTestsPassed = runReport.NumberOfTestsPassed,
                    NumberOfTestsFailed = runReport.NumberOfTestsFailed,
                    NumberOfTestsIgnored = runReport.NumberOfTestsIgnored,
                    NumberOfTestsRan = runReport.NumberOfTestsRan,
                    RunActions = runReport.RunActions.Select(x => new RunAction()
                                                                {
                                                                    Type = convertType(x.Type),
                                                                    Project = x.Project,
                                                                    Assembly = x.Assembly,
                                                                    TimeSpent = x.TimeSpent,
                                                                    Succeeded = x.Succeeded
                                                                }).ToArray()
                };
        }

        private static RunActionType convertType(AutoTest.Messages.InformationType informationType)
        {
            if (informationType == AutoTest.Messages.InformationType.Build)
                return RunActionType.Build;
            else if (informationType == AutoTest.Messages.InformationType.TestRun)
                return RunActionType.TestRun;
            return RunActionType.PreProcessing;
        }
    }

    public class Run
    {
        public TimeSpan RunDuration { get; internal set; }
        public int NumberOfBuildsSucceeded { get; internal set; }
        public int NumberOfBuildsFailed { get; internal set; }
        public int NumberOfProjectsBuilt { get; internal set; }
        public int NumberOfTestsPassed { get; internal set; }
        public int NumberOfTestsFailed { get; internal set; }
        public int NumberOfTestsIgnored { get; internal set; }
        public int NumberOfTestsRan { get; internal set; }
        public RunAction[] RunActions { get; internal set; }
    }

    public class RunAction
    {
        public RunActionType Type { get; internal set; }
        public string Project { get; internal set; }
        public string Assembly { get; internal set; }
        public TimeSpan TimeSpent { get; internal set; }
        public bool Succeeded { get; internal set; }
    }

    public enum RunActionType
    {
        Build = 1,
        TestRun = 2,
        PreProcessing = 3
    }
}
