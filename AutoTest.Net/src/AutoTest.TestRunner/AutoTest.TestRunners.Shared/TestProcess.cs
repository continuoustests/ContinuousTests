using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Targeting;
using System.IO;
using System.Reflection;
using AutoTest.TestRunners.Shared.Plugins;
using System.Diagnostics;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Errors;
using System.Threading;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners.Shared
{
    class TestProcess
    {
        private ITestRunProcessFeedback _feedback;
        private TargetedRun _targetedRun;
        private bool _runInParallel = false;
        private bool _startSuspended = false;
        private Func<bool> _abortWhen = null;
        private Action<Platform, Version, Action<ProcessStartInfo, bool>> _processWrapper = null;
        private bool _compatabilityMode = false;
        private Process _proc = null;
        private string _executable;
        private string _input;
        private string _output;
        private PipeClient _pipeClient = null;

        public TestProcess(TargetedRun targetedRun, ITestRunProcessFeedback feedback)
        {
            _targetedRun = targetedRun;
            _feedback = feedback;
        }

        public TestProcess RunParallel()
        {
            _runInParallel = true;
            return this;
        }

        public TestProcess StartSuspended()
        {
            _startSuspended = true;
            return this;
        }

        public TestProcess AbortWhen(Func<bool> abortWhen)
        {
            _abortWhen = abortWhen;
            return this;
        }

        public TestProcess WrapTestProcessWith(Action<Platform, Version, Action<ProcessStartInfo, bool>> processWrapper)
        {
            _processWrapper = processWrapper;
            return this;
        }

        public TestProcess RunInCompatibilityMode()
        {
            _compatabilityMode = true;
            return this;
        }
		 
        public void Start()
        {
            _executable = getExecutable();
            _input = createInputFile();
            _output = Path.GetTempFileName();
            runProcess();
        }

        private void runProcess()
        {
            if (_processWrapper == null)
                run(new ProcessStartInfo(), false);
            else
                _processWrapper.Invoke(_targetedRun.Platform, _targetedRun.TargetFramework, run);
        }

        private void run(ProcessStartInfo startInfo, bool doNotshellExecute)
        {
            var channel = Guid.NewGuid().ToString();
            var listener = startChannelListener(channel);
            var arguments = string.Format("--input=\"{0}\" --output=\"{1}\" --silent --channel=\"{2}\"", _input, _output, channel);
            if (_runInParallel)
                arguments += " --run_assemblies_parallel";
            if (_startSuspended)
                arguments += " --startsuspended";
            if (_compatabilityMode)
                arguments += " --compatibility-mode";
            if (_feedback != null)
                _feedback.ProcessStart(_executable + " " + arguments);
            _proc = new Process();
            _proc.StartInfo = startInfo;
			if (OS.IsPosix)
			{
				_proc.StartInfo.FileName = "mono";
				_proc.StartInfo.Arguments =  " --debug " + _executable + " " + arguments;
			}
			else
			{
            	_proc.StartInfo.FileName = _executable;
				_proc.StartInfo.Arguments = arguments;
			}
            _proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _proc.StartInfo.UseShellExecute = !doNotshellExecute;
            _proc.StartInfo.CreateNoWindow = true;
            _proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(_executable);
            _proc.Start();
            var abortListener = new System.Threading.Thread(listenForAborts);
            abortListener.Start();
            _proc.WaitForExit();
            closeClient();
            if (listener != null)
                listener.Join();
            abortListener.Join();
            if (aborted())
                return;
            var results = getResults(_output);
            TestRunProcess.AddResults(results);
        }

        private Thread startChannelListener(string channel)
        {
			if (OS.IsPosix)
				return null;
            var thread = new Thread(
                (x) => 
                    {
                        _pipeClient = new PipeClient();
                        _pipeClient.Listen(
                                x.ToString(),
                                (msg) => 
                                    {
                                        if (msg == "")
                                            return;
                                        if (_feedback != null)
                                        {
                                            var testStarted = "Test started:";
                                            if (msg.StartsWith(testStarted))
                                                _feedback.TestStarted(msg.Substring(testStarted.Length, msg.Length - testStarted.Length));
                                            else
                                                _feedback.TestFinished(TestResult.FromXml(msg));
                                        }
                                    });
                    });
            thread.Start(channel);
            return thread;
        }

        private bool aborted()
        {
            if (_abortWhen == null)
                return false;
            return _abortWhen.Invoke();
        }

        private void listenForAborts()
        {
            if (_abortWhen == null)
                return;
            if (_proc == null)
                return;
            while (!_proc.HasExited)
            {
                if (_abortWhen.Invoke())
                {
                    closeClient();
                    _proc.Kill();
                    return;
                }
                System.Threading.Thread.Sleep(10);
            }
        }

        private void closeClient()
        {
            if (_pipeClient != null)
            {
                _pipeClient.Disconnect();
                _pipeClient = null;
            }
        }

        private List<TestResult> getResults(string output)
        {
            var results = new List<TestResult>();
            if (File.Exists(output))
                results.AddRange(getResultsFromFile(output));
            else
                results.Add(ErrorHandler.GetError("Could not find output file " + output));
            return results;
        }

        private IEnumerable<TestResult> getResultsFromFile(string output)
        {
            var reader = new ResultXmlReader(output);
            return reader.Read();
        }

        private RunOptions getRunOptions()
        {
            var options = new RunOptions();
            foreach (var run in _targetedRun.Runners)
                options.AddTestRun(run);
            return options;
        }

        private string createInputFile()
        {
            var options = getRunOptions();
            var file = Path.GetTempFileName();
            var writer = new OptionsXmlWriter(getPlugins(options), options);
            writer.Write(file);
            return file;
        }

        private IEnumerable<Plugin> getPlugins(RunOptions options)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestRunners");
            return new PluginLocator(path).GetPluginsFrom(options);
        }

        private string getExecutable()
        {
            return new TestProcessSelector().Get(_targetedRun.Platform, _targetedRun.TargetFramework);
        }
    }
}
