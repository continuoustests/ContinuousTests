using System;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public enum UnitTestOutcome
    {
        Failed = 0,
        Inconclusive = 1,
        Passed = 2,
        InProgress = 3,
        Error = 4,
        Timeout = 5,
        Aborted = 6,
        Unknown = 7,
    }
}