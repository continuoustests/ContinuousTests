using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContinuousTests.ExtensionModel.Arguments;

namespace ContinuousTests.ExtensionModel
{
    /// <summary>
    /// ContinuousTests engine
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Occures when connecting to ContinuousTests engine
        /// </summary>
        event EventHandler EngineConnecting;

        /// <summary>
        /// Occures when disconnecting from ContinuousTests engine
        /// </summary>
        event EventHandler EngineDisconnecting;

        /// <summary>
        /// Occures when a build/test session starts
        /// </summary>
        event EventHandler SessionStarted;

        /// <summary>
        /// Occures when a build finishes
        /// </summary>
        event EventHandler<BuildFinishedArgs> BuildFinished;

        /// <summary>
        /// Occures when a set of tests finish
        /// </summary>
        event EventHandler<TestsFinishedArgs> TestsFinished;

        /// <summary>
        /// Occures whenever a test failes or a test that has formerly failed passes. If non of the above it will report test progress about every second
        /// </summary>
        event EventHandler<ImmediateTestFeedbackArgs> TestProgress;

        /// <summary>
        /// Occures when a build/test session completes
        /// </summary>
        event EventHandler<SessionFinishedArgs> SessionFinished;

        /// <summary>
        /// If engine is up and running and ready for use this property will return true
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Open a connection to the ContinuousTests engine
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnect from the ContinuousTests engine
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Will wait until engine
        /// </summary>
        void WaitForConnection();

        /// <summary>
        /// Build all projects and run all tests
        /// </summary>
        void RunAll();

        /// <summary>
        /// Run build and affected tests for a project
        /// </summary>
        /// <param name="project"></param>
        void RunPartial(string project);

        /// <summary>
        /// Run build and affected tests for a set of projects
        /// </summary>
        /// <param name="projects"></param>
        void RunPartial(IEnumerable<string> projects);

        /// <summary>
        /// Run a set of tests for a project
        /// </summary>
        /// <param name="scope"></param>
        void RunTests(TestScope scope);
        
        /// <summary>
        /// Run a set of tests or a set of projects
        /// </summary>
        /// <param name="scopes"></param>
        void RunTests(IEnumerable<TestScope> scopes);
    }
}
