using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Targeting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProfilerHost.Profiler;

namespace MMProfiler.Tests
{
    public class ProfilerTestBase
    {
        class RunnerFeedback : ITestRunProcessFeedback
        {
            public void ProcessStart(string commandline)
            {
                Console.WriteLine(commandline);
            }

            public void TestFinished(AutoTest.TestRunners.Shared.Results.TestResult result)
            {
                Console.WriteLine(result.State.ToString() + " " + result.TestName);
            }
        }

        protected class ResultsInfo
        {
            public string MetaData { get; set; }
            public string RuntimeData { get; set; }
        }

        private RunOptions GetRunOptions()
        {
            var assembly = new AssemblyOptions(@"P:\projects\mooseprofiler.git\MMProfiler\main\x86\Debug\Test.Target.dll");
            //assembly.AddTest(TestTarget);
            assembly.AddMember(TestTarget);
            var run = new RunnerOptions("NUnit");
            run.AddAssembly(assembly);
            var options = new RunOptions();
            options.AddTestRun(run);
            return options;
        }

        private void ProfilerWrapper(Platform platform, Action<ProcessStartInfo> testProcess)
        {
            Control.RunProcess(new Filter() { Includes = IncludeFilter },
                               (env) =>
                                   {
                                       var info = new ProcessStartInfo();
                                       env(info.EnvironmentVariables);
                                       testProcess(info);
                                   },
                               OnCallback,
                               (exception) => Assert.Fail(),
                               () => { }
                );
        }

        private void OnCallback(int length, byte[] data)
        {
            _stream.Write(data, 0, length);
        }

        protected Stream _stream;

        protected string IncludeFilter { get; set; }
        protected string TestTarget { get; set; }

        protected List<ResultsInfo> ResultsInfos { get; set; }

        [TestInitialize]
        virtual public void TestInitialize()
        {
            _stream = new MemoryStream();
            ResultsInfos = new List<ResultsInfo>();
        }

        protected void ExecuteTestRunner()
        {
            var runner = new TestRunProcess(new RunnerFeedback());
            runner
                .WrapTestProcessWith(ProfilerWrapper)
                .ProcessTestRuns(GetRunOptions());

            var reader = new BinaryReader(_stream);
            reader.BaseStream.Position = 0;
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var key = reader.ReadByte();
                switch (key)
                {
                    case 1:
                        reader.ReadUInt32();
                        reader.ReadDouble();
                        reader.ReadInt32();
                        reader.ReadUInt64();
                        var info = new ResultsInfo();
                        info.RuntimeData = Encoding.Unicode.GetString(reader.ReadBytes(reader.ReadInt32() * 2));
                        info.MetaData = Encoding.Unicode.GetString(reader.ReadBytes(reader.ReadInt32() * 2));
                        ResultsInfos.Add(info);
                        break;
                    case 2:
                        reader.ReadUInt32();
                        reader.ReadDouble();
                        reader.ReadInt32();
                        reader.ReadUInt64();
                        break;
                    case 3:
                        reader.ReadUInt32();
                        reader.ReadDouble();
                        reader.ReadInt32();
                        reader.ReadUInt64();
                        break;
                    default:
                        Assert.Fail("Validation failed {0} {1}", key, reader.BaseStream.Position);
                        return;
                }
            }
        }
    }
}