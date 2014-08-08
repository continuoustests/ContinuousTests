using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoTest.Graphs;
using AutoTest.Minimizer.Extensions;
using AutoTest.Minimizer.InterfaceFollowingStrategies;
using AutoTest.Minimizer.TestIdentifiers;
using AutoTest.Profiler;
using Mono.Cecil;

namespace AutoTest.Minimizer
{
    public class TestMinimizer
    {
        private readonly bool _debug;
        private readonly CouplingCache _cache;
        private readonly Dictionary<string, bool> _loadedAssemblies = new Dictionary<string, bool>();
        private readonly List<ITestIdentifier>  _testStrategies;
        private readonly int _numThreads;
        private IEnumerable<Change<MethodReference>> changes;
        private Func<string, string> _historyFilePathTranslator = s => s;
        private readonly List<CustomAssemblyResolver> toDispose = new List<CustomAssemblyResolver>();
        private AffectedGraph _lastAffectedGraph;


        public event EventHandler<MessageArgs> MinimizerMessage;

        public TestMinimizer(bool debug, int numThreads)
        {
            _debug = debug;
            _numThreads = numThreads;
            _testStrategies = new List<ITestIdentifier> {new NUnitTestIdentifier(), new MSTestTestIdentifier(), new mSpecTestIdentifier(), new XUnitTestIdentifier(), new SimpleTestingTestIdentifier()};
            _cache = new CouplingCache(_testStrategies);
        }

        public void TranslateHistoryFilePathsWith(Func<string, string> translator)
        {
            _historyFilePathTranslator = translator;
        }

        public ITestInformationEnricher GetTestEnricher()
        {
            InvokeMinimizerMessage(MessageType.Info, "Getting new Enricher");
            return new CouplingCacheTestInformationEnricher(_cache);
        }

        protected void InvokeMinimizerMessage(MessageType m, string message)
        {
            var handler = MinimizerMessage;
            if (handler != null) handler(this, new MessageArgs(m, message));
        }

        public void LoadOldCachedFiles(List<string> assemblies)
        {
            _cache.Clear();
            _loadedAssemblies.Clear();
            var maps = (from file in assemblies
                        let filename = GetHistoryFilename(file)
                        where File.Exists(filename)
                        select new BuildFullMapParams(filename, file)).ToList();
            BuildFullMap(maps);
        }

        public void DoInitialIndexOf(List<string> assemblies)
        {
            var param = from a in assemblies select new BuildFullMapParams(a, a);
            BuildFullMap(param);
            assemblies.ForkAndJoinTo(_numThreads, WriteHistoryFile);
            GC.Collect(2);
        }

        object WriteHistoryFile(string assemblyFileName)
        {
            var backupFilename = GetHistoryFilename(assemblyFileName);
            File.Copy(assemblyFileName, backupFilename, true);
            return null;
        }

        private string GetHistoryFilename(string assemblyFileName)
        {
            var fullPath = Path.GetFullPath(assemblyFileName);
            var directory = Path.GetDirectoryName(fullPath);
            var filename = Path.GetFileNameWithoutExtension(assemblyFileName);
            var extension = Path.GetExtension(assemblyFileName);
            var mmFile = directory + Path.DirectorySeparatorChar + filename + ".mm" + extension;
            return _historyFilePathTranslator(mmFile);
        }
         
        public List<TestEntry> GetTestsFor(List<string> assemblies)
        {
            var ret = new List<TestEntry>();
            var param = from a in assemblies select new BuildFullMapParams(a, a);
            BuildFullMap(param);
            toDispose.Clear();
            changes = GetChanges(assemblies);
            if (changes != null)
            {
                foreach (var change in changes)
                {
                    InvokeMinimizerMessage(MessageType.Debug, change.ChangeType + " " + change.ItemChanged.GetCacheName());
                }
                ReIndex(changes);
            }

            toDispose.ForEach(x => x.Dispose());
            ModuleDefinition.KillAllReadModules();
            var strategy = new GraphBuilder(_cache, _testStrategies, new IfInTestAssemblyContinueInterfaceFollowingStrategy(_cache), _debug, _numThreads);
            strategy.DebugMessage += (x,y) => InvokeMinimizerMessage(y.MessageType, y.Message);
            _lastAffectedGraph = strategy.GetAffectedGraphForChangeSet(changes);
            var entries = BuildEntriesFor(_lastAffectedGraph).ToList();
            foreach(var file in assemblies) WriteHistoryFile(file);
            return entries;
        }

        private IEnumerable<TestEntry> BuildEntriesFor(AffectedGraph graph)
        {
            InvokeMinimizerMessage(MessageType.Info, "there are " + graph.AllNodes().Count() + "in the graph.");
            foreach(var n in graph.AllNodes())
            {
                if (n.TestDescriptors == null) continue;
                foreach (var desc in n.TestDescriptors)
                {
                    yield return new TestEntry
                                     {
                                         TestAssembly = n.Assembly,
                                         TestClass = n.Type,
                                         TestName = desc.Target,
                                         TestRunners = new List<string> {desc.TestRunner}
                                     };
                }
            }
        }

        public List<TestEntry> GetTestEntriesFor(IEnumerable<string> entries)
        {
            var ret = new List<TestEntry>();
            var descriptors = new List<TestDescriptor>();
            foreach(var entry in entries)
            {
                var method = _cache.TryGetEfferentCouplingNode(entry);
                if (method == null) continue;
                var reference = method.MemberReference as MethodReference;
                if (reference == null) continue;
                foreach(var identifier in _testStrategies)
                {
                    if(identifier.IsTest(reference))
                         descriptors.Add(identifier.GetTestDescriptorFor(reference));
                }
            }
            return BuildTestEntriesFrom(descriptors);
        }

        public AffectedGraph GetGraphFor(string cacheName, bool debug)
        {
            var strategy = new GraphBuilder(_cache, _testStrategies, new IfInTestAssemblyContinueInterfaceFollowingStrategy(_cache), debug, _numThreads);
            strategy.DebugMessage += (x,y) => InvokeMinimizerMessage(y.MessageType, y.Message);
            return strategy.GetCouplingGraphFor(cacheName);
        }

        private static List<TestEntry> BuildTestEntriesFrom(IEnumerable<TestDescriptor> tests)
        {
            var hash = new Dictionary<string, TestEntry>();
            foreach(var test in tests)
            {
                TestEntry entry;
                if(!hash.TryGetValue(test.GetKey(), out entry))
                {
                    entry = new TestEntry {TestAssembly = test.Assembly, TestClass = test.Type, TestName=test.Target};
                    hash.Add(test.GetKey(), entry);
                }
                entry.TestRunners.Add(test.TestRunner);
            }
            return new List<TestEntry>(hash.Values);
        }

        private IEnumerable<Change<MethodReference>> GetChanges(IEnumerable<string> assemblies)
        {
            InvokeMinimizerMessage(MessageType.Info, "detecting changes.");
            var start = DateTime.Now;
            var mdr = GetAssemblyResolver("C:\\");
            toDispose.Add(mdr);
                var ret = assemblies.ForkAndJoinTo(_numThreads,x => GetChangesForAssembly(x, mdr)).CombineUniques();
                var end = DateTime.Now;
                InvokeMinimizerMessage(MessageType.Debug, "total detection took: " + (end - start));
                return ret;
        }

        private void ReIndex(IEnumerable<Change<MethodReference>> changes)
        { //TODO parallelize?
            InvokeMinimizerMessage(MessageType.Debug, "Reindexing Changes");
            var start = DateTime.Now;
            foreach(var change in changes)
            {
                _cache.Invalidate(change.ItemChanged.GetCacheName());
                if(change.ChangeType == ChangeType.Add || change.ChangeType == ChangeType.Modify)
                    Index(change.ItemChanged.ThreadSafeResolve());
            }
            var end = DateTime.Now;
            InvokeMinimizerMessage(MessageType.Debug, "reindex took: " + (end - start));
        }

        private IEnumerable<Change<MethodReference>> GetChangesForAssembly(string filename, CustomAssemblyResolver resolver)
        {
            var detector = new ChangeDetector();
            var assmName = Path.GetFileNameWithoutExtension(filename);
            var path = Path.GetDirectoryName(filename);
            resolver.SetNewSearchDirectoryTo(path);
            //foreach (var module in newAssembly.Modules)
            //    module.ReadSymbols();
            var historyFilename = GetHistoryFilename(filename);
            IEnumerable<Change<MethodReference>> changes = new List<Change<MethodReference>>();
            if (IsSameFile(filename, historyFilename))
            {
                InvokeMinimizerMessage(MessageType.Info,
                                        filename + " and " + GetHistoryFilename(filename) +
                                        " appear to be the same. skipping.");
                return changes;
            }
            var newAssembly = resolver.Resolve(assmName);
            if (File.Exists(historyFilename))
            {
                var oldAssembly = resolver.Resolve(assmName + ".mm");
                InvokeMinimizerMessage(MessageType.Info,
                                        "between " + oldAssembly.FullName + " and " + newAssembly.FullName);
                InvokeMinimizerMessage(MessageType.Info,
                                        "detecting changes between " + historyFilename + " and " + filename);
                changes = detector.GetChangesBetween(oldAssembly, newAssembly);
                oldAssembly.Dispose();
            } 
            else
            {
                //TODO this should mark everything as being an additive change!
                InvokeMinimizerMessage(MessageType.Info, "history file " + historyFilename + " does not exist.");
            }
            return changes;
        }

        private bool IsSameFile(string filename, string historyFilename)
        {
            if (!File.Exists(filename)) return false;
            if (!File.Exists(historyFilename)) return false;
            var original = File.GetLastWriteTime(historyFilename);
            var newfile = File.GetLastWriteTime(filename);
            InvokeMinimizerMessage(MessageType.Info, "history file = " + original + " new file = " + newfile);
            return original == newfile;
        }

        private static CustomAssemblyResolver GetAssemblyResolver(string path)
        {
            var mdr = new CustomAssemblyResolver();
            var paths = mdr.GetSearchDirectories();
            foreach(var p in paths) mdr.RemoveSearchDirectory(p);
            mdr.AddSearchDirectory(path);
            
            return mdr;
        }

        public class BuildFullMapParams
        {
            private readonly string _filename;
            private readonly string _asFile;

            public BuildFullMapParams(string filename, string asFile)
            {
                _filename = filename;
                _asFile = asFile;
            }

            public string Filename
            {
                get { return _filename; }
            }

            public string AsFile
            {
                get { return _asFile; }
            }
        }

        private void BuildFullMap(IEnumerable<BuildFullMapParams> buildFullMapParams)
        {
            try
            {
                using (var mdr = GetAssemblyResolver("C:\\"))
                {
                    foreach (var param in buildFullMapParams)
                    {
                        if (_loadedAssemblies.ContainsKey(param.AsFile))
                        {
                            InvokeMinimizerMessage(MessageType.Debug, param.Filename + " is cached.");
                            continue;
                        }
                        var assmName = Path.GetFileNameWithoutExtension(param.Filename);
                        var path = Path.GetDirectoryName(param.Filename);
                        mdr.SetNewSearchDirectoryTo(path);
                        InvokeMinimizerMessage(MessageType.Debug,
                                               "doing full afferent/efferent coupling map: " + param.Filename);

                        var assemblyObj = mdr.Resolve(assmName);

                        //foreach(var module in assemblyObj.Modules)
                        //    module.ReadSymbols();
                        var start = DateTime.Now;
                        foreach (var method in assemblyObj.AllNonIgnoredMethods())
                        {
                            Index(method);
                        }
                        _loadedAssemblies.Add(param.AsFile, true);
                        var end = DateTime.Now;
                        assemblyObj.Dispose();
                        assemblyObj.Modules.ToList().ForEach(x => x.Dispose());
                        InvokeMinimizerMessage(MessageType.Debug, "Full Map took: " + (end - start));
                        foreach (var l in mdr.GetCachedAssemblies())
                        {
                            InvokeMinimizerMessage(MessageType.Debug, "\tCached: " + l);
                        }
                    }
                }
            }
            finally
            {
                ModuleDefinition.KillAllReadModules();
            }
        }


        private void Index(MethodDefinition method)
        {
            var q = TypeScanner.ScanMethod(method);
            _cache.SetCoupling(q);
        }

        public IEnumerable<Change<string>> GetLastChanges()
        {
            return changes.Select(x => new Change<string>(x.ChangeType, x.ItemChanged.GetCacheName()));
        }

        public AffectedGraph GetLastAffectedGraph()
        {
            return _lastAffectedGraph;
        }

        public void EnrichGraph(AffectedGraph graph)
        {
            _cache.EnrichGraph(graph);
        }
    }

    public enum MessageType 
    {
        Debug,
        Info,
        Warn,
        Error
    }
    public class MessageArgs : EventArgs
    {
        public readonly MessageType MessageType;
        public readonly string Message;

        public MessageArgs(MessageType messageType, string message)
        {
            MessageType = messageType;
            Message = message;
        }
    }

    public class TestEntry
    {
        public string TestName;
        public string TestAssembly;
        public string TestClass;
        public List<string> TestRunners = new List<string>();
        public string Key
        {
            get { return TestAssembly + ":" + TestClass + ":" + TestName; }
        }
    }

    public class SymbolReadingAssemblyResolver : CustomAssemblyResolver
    {
    }
}
