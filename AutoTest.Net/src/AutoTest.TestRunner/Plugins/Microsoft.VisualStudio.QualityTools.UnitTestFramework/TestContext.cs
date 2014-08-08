using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public abstract class TestContext
    {
        public const string AspNetDevelopmentServerPrefix = "AspNetDevelopmentServer.";
        public virtual UnitTestOutcome CurrentTestOutcome { get; private set; }
        public abstract DbConnection DataConnection { get; }
        public abstract DataRow DataRow { get; }
        public virtual string DeploymentDirectory { get; private set; }
        public virtual string FullyQualifiedTestClassName { get; private set; }
        public abstract IDictionary Properties { get; }
        public virtual object RequestedPage { get; private set; }
        public virtual string ResultsDirectory { get; private set; }
        public virtual string TestDeploymentDir { get; private set; }
        public virtual string TestDir { get; private set; }
        public virtual string TestLogsDir { get; private set; }
        public virtual string TestName { get; private set; }
        public virtual string TestResultsDirectory { get; private set; }
        public virtual string TestRunDirectory { get; private set; }
        public virtual string TestRunResultsDirectory { get; set; }
        public abstract void AddResultFile(string fileName);
        public abstract void BeginTimer(string timerName);
        public abstract void EndTimer(string timerName);
        public abstract void WriteLine(string format, params object[] args);
    }
}
