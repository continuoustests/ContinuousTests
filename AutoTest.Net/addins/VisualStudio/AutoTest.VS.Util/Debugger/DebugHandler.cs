using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.Messages;
using EnvDTE80;
using System.IO;
using EnvDTE;
using AutoTest.TestRunners.Shared.Plugins;

namespace AutoTest.VS.Util.Debugger
{
    public class DebugHandler
    {
        private readonly DTE2 _application;

        public DebugHandler(DTE2 application)
        {
            _application = application;
        }

        public void Debug(CacheTestMessage test)
        {
            try
            {
                AutoTest.Core.DebugLog.Debug.WriteDebug("Starting debug session");
                var found = false;
                var targetFramework = "";
                found = setBreakpointFromMethod(test, ref targetFramework);
                if (!found)
                    found = setBreakpointFromStacktrace(test, ref targetFramework);

                if (!found)
                    return;

                var process = new AutoTestRunnerDebugProcess();
                var assembly = test.Assembly;

                AutoTest.Core.DebugLog.Debug.WriteDebug("Starting process suspended");
                var command = "";
                var options = new RunOptions();
                var runner = new RunnerOptions(getTestRunner(TestRunnerConverter.ToString(test.Test.Runner), test.Assembly, test.Test.Name));
                var asm = new AssemblyOptions(test.Assembly);
                asm.AddTest(test.Test.Name);
                runner.AddAssembly(asm);
                options.AddTestRun(runner);
                AutoTest.Core.DebugLog.Debug.WriteDebug(string.Format("Starting {0}", command));
                var processID = process.StartPaused(options, test.Test.Runner);
                try
                {
                    AutoTest.Core.DebugLog.Debug.WriteDebug("Locating debugger for Visual Studio " + _application.Version);
                    var dbg2 = (EnvDTE80.Debugger2)_application.Debugger;
                    var trans = (EnvDTE80.Transport)dbg2.Transports.Item("Default");
                    EnvDTE80.Engine[] dbgeng = null;
                    if (_application.Version == "9.0")
                    {
                        dbgeng = new EnvDTE80.Engine[] { trans.Engines.Item("Managed") };
                    }
                    else
                    {
                        foreach (var item in trans.Engines) {
                            var engine = (EnvDTE80.Engine)item;
                            if (engine.Name.Contains(string.Format("v{0}.{1}", process.Framework.Major, process.Framework.Minor))) {
                                dbgeng = new EnvDTE80.Engine[] { engine };
                                break;
                            }
                        }
                    }
                    
                    EnvDTE80.Process2 proc2 = null;
                    foreach (EnvDTE80.Process2 proc in dbg2.GetProcesses(trans, null))
                    {
                        if (proc.ProcessID == processID)
                        {
                            proc2 = proc;
                            break;
                        }
                    }
                    if (proc2 != null)
                        proc2.Attach2(dbgeng);
                }
                catch (Exception ex)
                {
                    AutoTest.Core.DebugLog.Debug.WriteException(ex);
                    throw;
                }
                finally
                {
                    AutoTest.Core.DebugLog.Debug.WriteDebug("Resuming process");
                    process.Resume();
                }
            }
            catch (Exception ex)
            {
                AutoTest.Core.DebugLog.Debug.WriteException(ex);
            }
        }

        private string getTestRunner(string testRunner, string assembly, string test)
        {
            if (testRunner.ToLower() != "any")
                return testRunner;

            var currentDirectory = Environment.CurrentDirectory;
            try
            {
                Environment.CurrentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var plugins = new PluginLocator().Locate();
                foreach (var plugin in plugins)
                {
                    var instance = plugin.New();
                    if (instance == null)
                        continue;
                    if (instance.IsTest(assembly, test))
                        return instance.Identifier;
                }
            }
            catch (Exception ex)
            {
                AutoTest.Core.DebugLog.Debug.WriteException(ex);
            }
            finally
            {
                Environment.CurrentDirectory = currentDirectory;
            }
            return testRunner;
        }

        private bool setBreakpointFromMethod(CacheTestMessage test, ref string targetFramework)
        {
            AutoTest.Core.DebugLog.Debug.WriteDebug("Attempting to set breakpoint through test method for " + test.Test.Name);
            var breakpoint = _application.Debugger.Breakpoints.Add(test.Test.Name.Replace("+", "."));
            if (breakpoint != null)
                return true;
            return false;
        }

        private bool setBreakpointFromStacktrace(CacheTestMessage test, ref string targetFramework)
        {
            // pull test name from full name
            var vsTestName = test.Test.Name.Replace("+", ".");
            var testName = vsTestName.Substring(vsTestName.LastIndexOf('.') + 1, vsTestName.Length - (vsTestName.LastIndexOf('.') + 1));
            AutoTest.Core.DebugLog.Debug.WriteDebug("Attempting to set breakpoint through call stack for " + testName);
            foreach (var line in test.Test.StackTrace)
            {
                if (!File.Exists(line.File))
                    continue;
                AutoTest.Core.DebugLog.Debug.WriteDebug(string.Format("Locating file {0}", line.File));
                var window = _application.OpenFile(EnvDTE.Constants.vsViewKindCode, line.File);
                if (breakOnFunctionElement(line.File, window.ProjectItem.FileCodeModel.CodeElements, testName))
                {
                    AutoTest.Core.DebugLog.Debug.WriteDebug("Found code file containing test and set breakpoint");
                    targetFramework = window.Project.Properties.Item("TargetFrameworkMoniker").Value.ToString();
                    AutoTest.Core.DebugLog.Debug.WriteDebug(string.Format("Target framework is {0}", targetFramework));
                    return true;
                }
            }
            return false;
        }

        private bool breakOnFunctionElement(string file, CodeElements codeElements, string functionName)
        {
            foreach (CodeElement codeElement in codeElements)
            {
                try
                {
                    if (codeElement.Kind == vsCMElement.vsCMElementFunction)
                    {
                        if (codeElement.Name.Equals(functionName))
                        {
                            AutoTest.Core.DebugLog.Debug.WriteDebug(string.Format("Found function element in {0} at {1}", file, codeElement.StartPoint.Line));
                            var lines = File.ReadAllLines(file);
                            for (int i = 0; i < 20; i++)
                            {
                                if (lines[codeElement.StartPoint.Line + i].Contains('{'))
                                {
                                    if (tryToSetBreakpoint(file, codeElement.StartPoint.Line, i + 1))
                                        return true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    AutoTest.Core.DebugLog.Debug.WriteException(ex);
                }
                if (codeElement.Kind.Equals(vsCMElement.vsCMElementNamespace))
                {
                    if (breakOnFunctionElement(file, codeElement.Children, functionName))
                        return true;
                }
                else if (codeElement.Kind.Equals(vsCMElement.vsCMElementClass))
                {
                    if (breakOnFunctionElement(file, codeElement.Children, functionName))
                        return true;
                }
            }
            AutoTest.Core.DebugLog.Debug.WriteDebug("Function element not found");
            return false;
        }

        private bool tryToSetBreakpoint(string fileName, int line, int offset)
        {
            int position = line + offset;
            try
            {
                _application.Debugger.Breakpoints.Add("", fileName, position);
                AutoTest.Core.DebugLog.Debug.WriteDebug(string.Format("Breakpoint set in {0} at {1}", fileName, position));
                return true;
            }
            catch
            {
                AutoTest.Core.DebugLog.Debug.WriteDebug(string.Format("Coult not set breakpoint in {0} at {1}", fileName, position));
                return false;
            }
        }
    }
}
