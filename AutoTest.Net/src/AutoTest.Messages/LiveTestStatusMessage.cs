using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTest.Messages
{
    public class LiveTestStatusMessage : IMessage, ICustomBinarySerializable
    {
        private LiveTestStatus[] _failedTest;
        private LiveTestStatus[] _failedButNowPassing;

        public string CurrentAssembly { get; private set; }
        public string CurrentTest { get; private set; }
        public int TestsCompleted { get; private set; }
        public int TotalNumberOfTests { get; private set; }
        public LiveTestStatus[] FailedTests { get { return _failedTest; } }
        public LiveTestStatus[] FailedButNowPassingTests { get { return _failedButNowPassing; } }

        public LiveTestStatusMessage(string assembly, string currentTest, int totalNumberOfTests, int completedTests, LiveTestStatus[] failedTests, LiveTestStatus[] failedButNowPassing)
        {
            CurrentAssembly = assembly;
            CurrentTest = currentTest;
            TotalNumberOfTests = totalNumberOfTests;
            TestsCompleted = completedTests;
            _failedTest = failedTests;
            _failedButNowPassing = failedButNowPassing;
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(CurrentAssembly);
            if (CurrentTest == null)
                writer.Write("");
            else
                writer.Write(CurrentTest);
            writer.Write(TotalNumberOfTests);
            writer.Write(TestsCompleted);
            writer.Write(_failedTest.Length);
            foreach (var failedTest in _failedTest)
                failedTest.WriteDataTo(writer);
            writer.Write(_failedButNowPassing.Length);
            foreach (var passing in _failedButNowPassing)
                passing.WriteDataTo(writer);
        }

        public void SetDataFrom(BinaryReader reader)
        {
            var tests = new List<LiveTestStatus>();
            CurrentAssembly = reader.ReadString();
            CurrentTest = reader.ReadString();
            TotalNumberOfTests = reader.ReadInt32();
            TestsCompleted = reader.ReadInt32();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var test = new LiveTestStatus("", null);
                test.SetDataFrom(reader);
                tests.Add(test);
            }
            _failedTest = tests.ToArray();

            tests = new List<LiveTestStatus>();
            count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var test = new LiveTestStatus("", null);
                test.SetDataFrom(reader);
                tests.Add(test);
            }
            _failedButNowPassing = tests.ToArray();
        }
    }

    public class LiveTestStatus : ICustomBinarySerializable
    {
        public string Assembly { get; private set; }
        public TestResult Test { get; private set; }

        public LiveTestStatus(string assembly, TestResult test)
        {
            Assembly = assembly;
            Test = test;
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(Assembly);
            Test.WriteDataTo(writer);
        }

        public void SetDataFrom(BinaryReader reader)
        {
            Assembly = reader.ReadString();
            Test = new TestResult(TestRunner.Any, TestRunStatus.Failed, "");
            Test.SetDataFrom(reader);
        }
    }
}
