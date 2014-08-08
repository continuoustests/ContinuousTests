using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContinuousTests.ExtensionModel
{
    /// <summary>
    /// Test scope for running tests
    /// </summary>
    public class TestScope
    {
        internal bool RunAllTestsInProject { get; private set; }

        /// <summary>
        /// Fullpath to project file
        /// </summary>
        public string Project { get; private set; }

        /// <summary>
        /// Fullname to to test Namespace.TestClass.TestMethod
        /// </summary>
        public string[] Tests { get; private set; }

        /// <summary>
        /// Fullname to member Namespace.TestClass. Will run all tests contained by member
        /// </summary>
        public string[] Members { get; private set; }

        /// <summary>
        /// Will run all tests in namespace
        /// </summary>
        public string[] Namespaces { get; private set; }

        /// <summary>
        /// Project test run
        /// </summary>
        /// <param name="project">Fullpath to project that should be tested</param>
        public TestScope(string project)
        {
            RunAllTestsInProject = true;
            Project = project;
            Tests = new string[] {};
            Members = new string[] { };
            Namespaces = new string[] { };
        }

        /// <summary>
        /// Selected test run
        /// </summary>
        /// <param name="project">Fullpath to project that should be tested</param>
        /// <param name="tests">Tests that should be included in the test run</param>
        /// <param name="members">Members that should be included in the test run</param>
        /// <param name="namespaces">Namespaces that should be included in the test run</param>
        public TestScope(string project, string[] tests, string[] members, string[] namespaces)
        {
            RunAllTestsInProject = false;
            Project = project;
            Tests = tests;
            Members = members;
            Namespaces = namespaces;
        }

        internal AutoTest.Messages.OnDemandRun ToInternal()
        {
            if (RunAllTestsInProject)
                return new AutoTest.Messages.OnDemandRun(Project).ShouldRunAllTestsInProject();
            return new AutoTest.Messages.OnDemandRun(Project, Tests, Members, Namespaces);
        }
    }
}
