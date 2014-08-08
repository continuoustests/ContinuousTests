using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Plugins;
using AutoTest.TestRunners.Shared.Errors;
using System.IO;
using System.Reflection;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Communication;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners
{
    class TestRunner : MarshalByRefObject, ITestRunner
    {
        private List<TestResult> _results = new List<TestResult>();
        private List<string> _directories = new List<string>();
        private Dictionary<string, string> _assemblyCache = new Dictionary<string, string>();

        private static string getPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var atDir = Path.Combine(appData, "AutoTest.Net.Runner");
            if (!Directory.Exists(atDir))
                Directory.CreateDirectory(atDir);
            return atDir;
        }

        public void SetupResolver(bool startLogger)
        {
            if (startLogger)
                Logger.SetLogger(new ConsoleLogger());
            //Logger.SetLogger(new FileLogger(true, Path.Combine(getPath(), "runner.log." + DateTime.Now.Ticks.ToString())));
            _directories.Add(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        public IEnumerable<TestResult> Run(Plugin plugin, string id, RunSettings settings)
        {
            IEnumerable<TestResult> resultSet = null;
            _directories.Add(Path.GetDirectoryName(settings.Assembly.Assembly));
            _directories.Add(Path.GetDirectoryName(plugin.Assembly));
            Logger.Write("About to create plugin {0} in {1} for {2}", plugin.Type, plugin.Assembly, id);
            var runner = getRunner(plugin);
            var currentDirectory = Environment.CurrentDirectory;
            try
            {
                if (runner == null)
                    return _results;
                using (var server = new PipeServer(settings.PipeName))
                {
                    Logger.Write("Matching plugin identifier ({0}) to test identifier ({1})", runner.Identifier, id);
                    if (!runner.Identifier.ToLower().Equals(id.ToLower()) && !id.ToLower().Equals("any"))
                        return _results;
                    Logger.Write("Checking whether assembly contains tests for {0}", id);
                    if (!settings.Assembly.IsVerified && !runner.ContainsTestsFor(settings.Assembly.Assembly))
                        return _results;
                    Logger.Write("Initializing channel");
                    runner.SetLiveFeedbackChannel(new TestFeedbackProvider(server));
                    var newCurrent = Path.GetDirectoryName(settings.Assembly.Assembly);
                    Logger.Write("Setting current directory to " + newCurrent);
                    Environment.CurrentDirectory = newCurrent;
                    Logger.Write("Starting test run");
                    resultSet = runner.Run(settings);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Environment.CurrentDirectory = currentDirectory;
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            }
            return resultSet;
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (_assemblyCache.ContainsKey(args.Name))
            {
                if (_assemblyCache[args.Name] == "NotFound")
                    return null;
                else
                    return Assembly.LoadFrom(_assemblyCache[args.Name]);
            }
            foreach (var directory in _directories)
            {
                var file = Directory.GetFiles(directory).Where(f => isMissingAssembly(args, f)).Select(x => x).FirstOrDefault();
                if (file == null)
                    continue;
                return Assembly.LoadFrom(file);
            }
            _assemblyCache.Add(args.Name, "NotFound");
            return null;
        }

        private bool isMissingAssembly(ResolveEventArgs args, string f)
        {
            try
            {
                if (_assemblyCache.ContainsValue(f))
                    return false;
                var name = Assembly.ReflectionOnlyLoadFrom(f).FullName;
                if (!_assemblyCache.ContainsKey(name))
                    _assemblyCache.Add(name, f);
                return name.Equals(args.Name);
            }
            catch
            {
                var key = "invalid signature for " + Path.GetFileName(f);
                if (!_assemblyCache.ContainsKey(key))
                    _assemblyCache.Add(key, f);
                return false;
            }
        }

        private IAutoTestNetTestRunner getRunner(Plugin plugin)
        {
            try
            {
                return plugin.New();
            }
            catch (Exception ex)
            {
                _results.Add(ErrorHandler.GetError(plugin.Type, ex));
            }
            return null;
        }
    }
}