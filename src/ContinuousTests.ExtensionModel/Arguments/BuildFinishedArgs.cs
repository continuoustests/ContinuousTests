using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContinuousTests.ExtensionModel.Arguments
{
    public class BuildFinishedArgs : EventArgs
    {
        public int ErrorCount { get; internal set; }
        public BuildResult[] Errors { get; internal set; }
        public string Project { get; internal set; }
        public TimeSpan TimeSpent { get; internal set; }
        public int WarningCount { get; internal set; }
        public BuildResult[] Warnings { get; internal set; }

        internal static BuildFinishedArgs FromInternal(AutoTest.Messages.BuildRunMessage msg)
        {
            return new BuildFinishedArgs()
                        {
                            Project = msg.Results.Project,
                            ErrorCount = msg.Results.ErrorCount,
                            WarningCount = msg.Results.WarningCount,
                            Errors = msg.Results.Errors.Select(x => getBuildResult(x)).ToArray(),
                            Warnings = msg.Results.Warnings.Select(x => getBuildResult(x)).ToArray(),
                            TimeSpent = msg.Results.TimeSpent
                        };
        }

        private static BuildResult getBuildResult(AutoTest.Messages.BuildMessage x)
        {
            return new BuildResult()
                            {
                                ErrorMessage = x.ErrorMessage,
                                File = x.File,
                                LineNumber = x.LineNumber,
                                LinePosition = x.LinePosition
                            };
        }
    }

    public class BuildResult
    {
        public string ErrorMessage { get; internal set; }
        public string File { get; internal set; }
        public int LineNumber { get; internal set; }
        public int LinePosition { get; internal set; }
    }
}
