using AutoTest.Messages;

namespace AutoTest.VM
{
    internal class TestFailure
    {
        public string Assembly { get; set; }
        public string ClassName { get; set; }
        public string TestName { get; set; }
        public TestRunner Runner { get; set; }

        public TestFailure(string assembly, string className, string testName, TestRunner runner)
        {
            Assembly = assembly;
            ClassName = className;
            TestName = testName;
            Runner = runner;
        }
    }
}