using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using AutoTest.TestRunners.Shared.Options;
using System.IO;
using AutoTest.TestRunners.Shared.Plugins;
using AutoTest.Messages;
using AutoTest.TestRunners.Shared.Targeting;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.VS.Util.Debugger
{
    class AutoTestRunnerDebugProcess : IAutoTestRunnerDebugProcess
    {
        private Process _process = null;
        private Version _version = null;

        public int ID { get { return _process.Id; } }
        public Version Framework { get { return _version; } }

        public int StartPaused(RunOptions options, TestRunner runner)
        {
            var currentPath = Environment.CurrentDirectory;
            try
            {
                Environment.CurrentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                using (var assembly = Reflect.On(options.TestRuns.ElementAt(0).Assemblies.ElementAt(0).Assembly))
                {
                    _version = assembly.GetTargetFramework();
                }
                //var wrapper = getWrapper(options, runner);
                var inputFile = Path.GetTempFileName();
                var outputFile = Path.GetTempFileName();
                var writer = new OptionsXmlWriter(new Plugin[] {}, options);
                writer.Write(inputFile);
                AutoTest.Core.DebugLog.Debug.WriteDebug("About to debug:");
                AutoTest.Core.DebugLog.Debug.WriteDebug(File.ReadAllText(inputFile));
                var arguments = string.Format("--input=\"{0}\" --output=\"{1}\" --startsuspended --silent", inputFile, outputFile);
                var exe = new TestProcessSelector().Get(options.TestRuns.ElementAt(0).Assemblies.ElementAt(0).Assembly);
                AutoTest.Core.DebugLog.Debug.WriteDebug("Running: " + exe + " " + arguments);
                _process = new Process();
                _process.StartInfo = new ProcessStartInfo(exe, arguments);
                _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                _process.StartInfo.RedirectStandardInput = true;
                _process.StartInfo.UseShellExecute = false;
                _process.StartInfo.CreateNoWindow = true;
                _process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                _process.Start();
                return _process.Id;
            }
            catch (Exception ex)
            {
                AutoTest.Core.DebugLog.Debug.WriteException(ex);
            }
            finally
            {
                Environment.CurrentDirectory = currentPath;
            }
            return 0;
        }

        public void Resume()
        {
            _process.StandardInput.WriteLine("resume");
        }
    }
}
