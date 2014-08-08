using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Plugins;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Options;
using System.Runtime.Remoting;
using AutoTest.TestRunners.Shared.Errors;
using System.Reflection;
using System.Threading;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners
{
    class Program
    {
        private static Arguments _arguments;
        private static List<TestResult> _results = new List<TestResult>();
        private static int _mainThreadID = 0;
        private static List<Thread> _haltedThreads = new List<Thread>();
        
        private static string getPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var atDir = Path.Combine(appData, "AutoTest.Net.Runner");
            if (!Directory.Exists(atDir))
                Directory.CreateDirectory(atDir);
            return atDir;
        }

        [STAThread]
        static void Main(string[] args)
        {
            _mainThreadID = Thread.CurrentThread.ManagedThreadId;
            var parser = new ArgumentParser(args);
            _arguments = parser.Parse();
            if (_arguments.Logging)
                Logger.SetLogger(new ConsoleLogger());
            //Logger.SetLogger(new FileLogger(true, Path.Combine(getPath(), "runner.log." + DateTime.Now.Ticks.ToString())));
            writeHeader();
            if (!File.Exists(_arguments.InputFile) || _arguments.OutputFile == null)
            {
                printUseage();
                return;
            }
            Write("Test run options:");
            Write(File.ReadAllText(_arguments.InputFile));
            if (_arguments.StartSuspended)
                Console.ReadLine();
            tryRunTests();
            Write(" ");
            if (File.Exists(_arguments.OutputFile))
            {
                Write("Test run result:");
                Write(File.ReadAllText(_arguments.OutputFile));
            }

			// We do this since NUnit threads some times keep staing in running mode even after finished.
            killHaltedThreads();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private static void killHaltedThreads()
        {
            lock (_haltedThreads)
            {
                if (_haltedThreads.Count == 0)
                    return;
                foreach (var thread in _haltedThreads)
                    thread.Abort();
                Thread.Sleep(100);
            }
        }

        private static void writeHeader()
        {
            Write("AutoTest.TestRunner v0.1");
            Write("Author - Svein Arne Ackenhausen");
            Write("AutoTest.TestRunner is a plugin based generic test runner. ");
            Write("");
        }

        private static void tryRunTests()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledExceptionHandler;
            using (var junction = new PipeJunction(_arguments.Channel))
            {
                try
                {
                    var parser = new OptionsXmlReader(_arguments.InputFile);
                    parser.Parse();
                    if (!parser.IsValid)
                        return;

                    run(parser, junction);

                    var writer = new ResultsXmlWriter(_results);
                    writer.Write(_arguments.OutputFile);
                }
                catch (Exception ex)
                {
                    try
                    {
                        var result = new List<TestResult>();
                        result.Add(ErrorHandler.GetError("Init", ex));
                        var writer = new ResultsXmlWriter(result);
                        writer.Write(_arguments.OutputFile);
                    }
                    catch
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomainUnhandledExceptionHandler;
        }

        public static void CurrentDomainUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var message = getException((Exception)args.ExceptionObject);

            // TODO: Seriously!? Firgure out what thread is causing the app domain unload exception
			// Yeah, seriously. When user code throws background exceptions we want them to know.
            if (!_arguments.CompatabilityMode && !args.ExceptionObject.GetType().Equals(typeof(System.AppDomainUnloadedException)))
            {
                var finalOutput = new TestResult("Any", "", "", 0, "An unhandled exception was thrown while running a test.", TestState.Panic,
                        "This is usually caused by a background thread throwing an unhandled exception. " +
                        "Most test runners run in clr 1 mode which hides these exceptions from you. If you still want to suppress these errors (not recommended) you can enable compatibility mode." + Environment.NewLine + Environment.NewLine +
                        "To head on to happy land where fluffy bunnies roam freely painting green left right and center do so by passing the --compatibility-mode switch to the test " +
                        "runner or set the <TestRunnerCompatibilityMode>true</TestRunnerCompatibilityMode> configuration option in " +
                        "AutoTest.Net." + Environment.NewLine + Environment.NewLine + message);
                AddResults(finalOutput);
            }

            if (args.IsTerminating)
            {
                var writer = new ResultsXmlWriter(_results);
                writer.Write(_arguments.OutputFile);
                Write(" ");
                if (File.Exists(_arguments.OutputFile))
                {
                    Write("Test run result:");
                    Write(File.ReadAllText(_arguments.OutputFile));
                }
                Environment.Exit(-1);
            }

            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Name = "Dead thread";
            lock (_haltedThreads)
            {
                _haltedThreads.Add(Thread.CurrentThread);
            }

            /*if (Thread.CurrentThread.ManagedThreadId != _mainThreadID)
            {
                while (true)
                    Thread.Sleep(TimeSpan.FromHours(1));
            }*/
        }

        private static string getException(Exception ex)
        {
            var text = ex.ToString();
            if (ex.InnerException != null)
                text += getException(ex.InnerException);
            return text;
        }

        private static void printUseage()
        {
            Write("Syntax: AutoTest.TestRunner.exe --input=options_file --output=result_file [--startsuspended] [--silent] [--logging] [--compatibility-mode]");
            Write("");
            Write("Options format");
            Write("<=====================================================>");
            Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            Write("<run>");
            Write("\t<!--It can contain 0-n plugins. If 0 the runner will load all available plugins-->");
            Write("\t<plugin type=\"Plugin.IAutoTestNetTestRunner.Implementation\">C:\\Some\\Path\\PluginAssembly.dll</plugin>");
            Write("\t<!--It can contain 0-n runners. The id is what determines which runner will handle that run-->");
            Write("\t<runner id=\"NUnit\">");
            Write("\t\t<!--It can contain 0-n categories to ignore-->");
            Write("\t\t<categories>");
            Write("\t\t\t<ignore_category>IgnoreCategory</ignore_category>");
            Write("\t\t</categories>");
            Write("\t\t<!--It can contain 1-n assemblies to test.-->");
            Write("\t\t<test_assembly name=\"C:\\my\\testassembly.dll\">");
            Write("\t\t\t<!--It can contain 0-n tests-->");
            Write("\t\t\t<tests>");
            Write("\t\t\t\t<test>testassembly.class.test1</test>");
            Write("\t\t\t</tests>");
            Write("\t\t\t<!--It can contain 0-n members-->");
            Write("\t\t\t<members>");
            Write("\t\t\t\t<member>testassembly.class2</member>");
            Write("\t\t\t</members>");
            Write("\t\t\t<!--It can contain 0-n namespaces-->");
            Write("\t\t\t<namespaces>");
            Write("\t\t\t\t<namespace>testassembly.somenamespace1</namespace>");
            Write("\t\t\t</namespaces>");
            Write("\t\t</test_assembly>");
            Write("\t</runner>");
            Write("</run>");
        }

        private static void run(OptionsXmlReader parser, PipeJunction junction)
        {
            foreach (var plugin in getPlugins(parser))
            {
                var instance = plugin.New();
                foreach (var currentRuns in parser.Options.TestRuns.Where(x => x.ID.ToLower().Equals("any") || instance.Handles(x.ID)))
                {
                    var run = getTestRunsFor(instance, currentRuns);
                    if (run == null)
                        continue;
                    foreach (var assembly in run.Assemblies)
                    {
                        WriteNow("Running tests for " + assembly.Assembly + " using " + plugin.Type);
                        var pipeName = Guid.NewGuid().ToString();
                        junction.Combine(pipeName);
                        var process = new SubDomainRunner(plugin, run.ID, run.Categories, assembly, _arguments.Logging, pipeName, _arguments.CompatabilityMode);
                        process.Run(null);
                    }
                }
            }
        }

        private static RunnerOptions getTestRunsFor(IAutoTestNetTestRunner instance, RunnerOptions run)
        {
            if (run.ID.ToLower() != "any")
                return run;

            var newRun = new RunnerOptions(run.ID);
            newRun.AddCategories(run.Categories.ToArray());
            foreach (var asm in run.Assemblies)
            {
                if (!asm.IsVerified && !instance.ContainsTestsFor(asm.Assembly))
                    continue;
                var assembly = new AssemblyOptions(asm.Assembly);
                assembly.AddNamespaces(asm.Namespaces.Where(x => asm.IsVerified || instance.ContainsTestsFor(asm.Assembly, x)).ToArray());
                assembly.AddMembers(asm.Members.Where(x => asm.IsVerified || instance.ContainsTestsFor(asm.Assembly, x)).ToArray());
                assembly.AddTests(asm.Tests.Where(x => asm.IsVerified || instance.IsTest(asm.Assembly, x)).ToArray());
                if (hasNoTests(asm) || hasTests(assembly))
                    newRun.AddAssembly(assembly);
            }
            if (newRun.Assemblies.Count() == 0)
                return null;
            return newRun;
        }

        private static bool hasTests(AssemblyOptions asm)
        {
            return asm.Namespaces.Count() != 0 || asm.Members.Count() != 0 || asm.Tests.Count() != 0;
        }

        private static bool hasNoTests(AssemblyOptions asm)
        {
            return asm.Namespaces.Count() == 0 && asm.Members.Count() == 0 && asm.Tests.Count() == 0;
        }

        public static void AddResults(IEnumerable<TestResult> results)
        {
            lock(_results)
            {
                _results.AddRange(results);
            }
        }

        public static void AddResults(TestResult result)
        {
            lock (_results)
            {
                _results.Add(result);
            }
        }

        private static IEnumerable<Plugin> getPlugins(OptionsXmlReader parser)
        {
            if (parser.Plugins.Count() == 0)
                return allPlugins();
            return parser.Plugins;
        }

        private static IEnumerable<Plugin> allPlugins()
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var dir = Path.Combine(currentDir, "TestRunners");
            if (!Directory.Exists(dir))
                return new Plugin[] { };
            var locator = new PluginLocator(dir);
            return locator.Locate();
        }

        public static void WriteNow(string message)
        {
            Write(string.Format("{0} {1}", DateTime.Now.ToLongTimeString(), message));
        }

        public static void Write(string message)
        {
            if (!_arguments.Silent)
                Console.WriteLine(message);
        }
    }
}
