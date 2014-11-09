using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Runtime.InteropServices;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Targeting;
using ProfilerHost.Profiler;

namespace ProfilerHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new TestRunProcess(new RunnerFeedback());
            runner
                .WrapTestProcessWith(profilerWrapper)
                .ProcessTestRuns(getRunOptions());
            //Console.ReadLine();
        }

        private static void profilerWrapper(Platform platform, Action<ProcessStartInfo> testProcess)
        {
            var fileName = Path.Combine(Path.GetTempPath(), "MMProfiler.log");
            File.Delete(fileName);
            Console.WriteLine(fileName);
            var fs = File.Create(fileName);

            Control.RunProcess(new Filter() { BufferSize = 1024 * 1024, ThresholdSize = 64 * 1024, Includes = "Test" }, 
                (env) =>
                    {
                        var info = new ProcessStartInfo();
                        env(info.EnvironmentVariables);
                        testProcess(info);
                    },
                (length, buffer) => fs.Write(buffer, 0, length),
                (exception) => {},
                fs.Close);

            ValidateOutputFile(fileName);
        }

        private static RunOptions getRunOptions()
        {
            var assembly = new AssemblyOptions(@"P:\projects\mooseprofiler.git\MMProfiler\main\x86\Debug\Test.Target.dll");
            var run = new RunnerOptions("NUnit");
            run.AddAssembly(assembly);     
            var optoins = new RunOptions();
            optoins.AddTestRun(run);
            return optoins;
        }

        private static void ValidateOutputFile(string fileName)
        {
            var validate = File.OpenRead(fileName);
            var reader = new BinaryReader(validate);
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
                        Console.WriteLine("{0}", Encoding.Unicode.GetString(reader.ReadBytes(reader.ReadInt32() * 2)));
                        Console.WriteLine("\t{0}", Encoding.Unicode.GetString(reader.ReadBytes(reader.ReadInt32() * 2)));
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
                        Console.WriteLine("Validation failed {0} {1}", key, reader.BaseStream.Position);
                        return;
                }
            }

            Console.WriteLine("File validated");
        }
    
    
    }

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

}
