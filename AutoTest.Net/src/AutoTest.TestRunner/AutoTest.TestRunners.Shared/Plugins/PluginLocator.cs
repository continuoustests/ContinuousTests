using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Communication;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners.Shared.Plugins
{
    [Serializable]
    public class Plugin
    {
        public string Assembly { get; private set; }
        public string Type { get; private set; }

        public Plugin(string assembly, string type)
        {
            Assembly = assembly;
            Type = type;
        }

        public IAutoTestNetTestRunner New()
        {
            IAutoTestNetTestRunner runner;
            var hitPaths = new string[]
                                {
                                    Path.GetDirectoryName(Assembly),
                                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                                };

            hitPaths.ToList().ForEach(x => Logger.Write("Hint path (new): " + x));
            using (var resolver = new AssemblyResolver(hitPaths))
            {
                try
                {
                    var asm = System.Reflection.Assembly.LoadFrom(Assembly);
                    runner = (IAutoTestNetTestRunner)asm.CreateInstance(Type);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                    return null;
                }
            }
            runner.SetLogger(Logger.Instance);
            runner.SetReflectionProvider((assembly) => { return Reflect.On(assembly); });
            runner.SetLiveFeedbackChannel(new NullTestFeedbackProvider());
            return runner;
        }
    }

    public class PluginLocator
    {
        private string _path;

        public PluginLocator()
        {
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _path = Path.Combine(path, "TestRunners");
        }

        public PluginLocator(string path)
        {
            _path = path;
        }

        public IEnumerable<Plugin> Locate()
        {
            var plugins = new List<Plugin>();
            var hitPaths = new string[]
                                {
                                    _path,
                                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                                };
            hitPaths.ToList().ForEach(x => Logger.Write("Hint path (locate): " + x));
            using (var resolver = new AssemblyResolver(hitPaths))
            {
                var currentDirectory = Environment.CurrentDirectory;
                try
                {
                    Environment.CurrentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    var files = Directory.GetFiles(_path, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                        plugins.AddRange(getPlugins(Path.GetFullPath(file)));
                }
                finally
                {
                    Environment.CurrentDirectory = currentDirectory;
                }
            }
            return plugins;
        }

        public IEnumerable<Plugin> GetPluginsFrom(RunOptions options)
        {
            var plugins = new List<Plugin>();
            foreach (var plugin in Locate())
            {
                var instance = plugin.New();
                if (options.TestRuns.Where(x => x.ID.Equals(instance.Identifier)).Count() > 0)
                    plugins.Add(plugin);
            }
            return plugins;
        }

        private IEnumerable<Plugin> getPlugins(string file)
        {
            var assembly = loadAssembly(Path.GetFullPath(file));
            if (assembly == null)
                return new Plugin[] {};
            return assembly
                .GetExportedTypes()
                .Where(x => x.GetInterfaces().Contains(typeof(IAutoTestNetTestRunner)) && x.GetConstructor(Type.EmptyTypes) != null)
                .Select(x => new Plugin(file, x.FullName));
        }

        private System.Reflection.Assembly loadAssembly(string file)
        {
            try
            {
                return System.Reflection.Assembly.LoadFrom(file);
            }
            catch
            {
                return null;
            }
        }
    }

    public class AssemblyResolver : IDisposable
    {
        private List<string> _directories = new List<string>();
        private Dictionary<string, string> _assemblyCache = new Dictionary<string, string>();

        public AssemblyResolver(string[] hintPaths)
        {
            _directories.AddRange(hintPaths);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (_assemblyCache.ContainsKey(args.Name))
            {
                if (_assemblyCache[args.Name] == "NotFound")
                    return null;
                else
                    return System.Reflection.Assembly.LoadFrom(_assemblyCache[args.Name]);
            }
            foreach (var directory in _directories)
            {
                var file = Directory.GetFiles(directory).Where(f => isMissingAssembly(args, f)).Select(x => x).FirstOrDefault();
                if (file == null)
                    continue;
                return System.Reflection.Assembly.LoadFrom(file);
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
                var name = System.Reflection.Assembly.ReflectionOnlyLoadFrom(f).FullName;
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

        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }
    }
}
