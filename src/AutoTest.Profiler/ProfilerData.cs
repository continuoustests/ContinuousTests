using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AutoTest.Profiler.Database;
using AutoTest.Graphs;
namespace AutoTest.Profiler
{
    public class ProfilerData
    {
        private readonly string _filename;
        private readonly IProfilerDataParser _parser;
        private readonly IContextChangeFinder _finder;
        private readonly decimal _compressRatio;
        private readonly TestRunInformationDatabase _database;
        private readonly TestRunInfoAssembler _assembler;
        private readonly CouplingCountAndNameProjection _counts;
        private readonly object _lock = new object();

        public ProfilerData(string filename, IProfilerDataParser parser, IContextChangeFinder finder) : this(filename, parser, finder, 0.5m)
        {
        }

        public ProfilerData(string filename, IProfilerDataParser parser, IContextChangeFinder finder, decimal compressRatio)
        {
            _assembler = new TestRunInfoAssembler(finder);
            _database = new TestRunInformationDatabase(filename);
            _counts = new CouplingCountAndNameProjection();
            _finder = finder;
            _filename = filename;
            _parser = parser;
            _compressRatio = compressRatio;
        }

        public int TotalEntries
        {
            get { return _database.TotalEntries; }
        }

        public long Waste
        {
            get { return _database.FileWaste; }
            
        }

        public double TotalSize
        {
            get { return _database.TotalSize; }
        }

        public void UpdateInfo(string filename, ITestInformationEnricher enricher)
        {
            try
            {
                if (Monitor.TryEnter(_lock, 5000))
                {
                    try
                    {
                        using (var f = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                        {
                            enricher.ClearCache();
                            var items = enricher.Enrich(_parser.Parse(f));
                            items = Printall(items);
                            var infos = _assembler.Assemble(items);
                            //var infos2 = printInfos(infos);
                            _database.AddNewEntries(infos);
                            _database.TakeSnapshot();
                            enricher.ClearCache();
                        }
                    }
                    finally
                    {
                        Monitor.Exit(_lock);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public CountsAndTimes GetCountsAndTimesFor(string name)
        {
            return _counts.GetRuntimeCallTimingsFor(name);
        }

        private IEnumerable<TestRunInformation> printInfos(IEnumerable<TestRunInformation> infos)
        {
            foreach (var info in infos)
            {
                Debug("TestInfo: " + info.Name + " setups = " + info.Setups.Count + " teardowns=" + info.Teardowns.Count);
                yield return info;
            }
        }

        public AffectedGraph GetProfiledGraphFor(string method)
        {
            var graph = new AffectedGraph();
            var tests = GetTestsFor(method);
            if (tests == null || !tests.Any()) return graph;
            var paths = new List<IEnumerable<string>>();
            foreach(var test in tests)
            {
                var info = GetTestInformationFor(test);
                if (info != null)
                {
                    var path = PathFinder.FindPathsTo(info, method);
                    paths.AddRange(path);
                }
            }
            return GraphBuilder.BuildGraphFor(paths);
        }

        public Dictionary<string, bool> GetProfiledNodesFor(string method)
        {
            var ret = new Dictionary<string, bool>();
            var tests = _counts.GetTestsFor(method);
            foreach (var node in
                tests.Select(t => _database.LookUpByName(t)).Select(test => PathFinder.FindPathsTo(test, method)).SelectMany(paths => paths.SelectMany(path => path)))
            {
                ret[node] = true;
            }
            return ret;
        }

        public void Compress()
        {
            if(!Monitor.TryEnter(_lock, 5000)) throw new Exception("Unable to acquire profiler compression lock.");
            try
            {
                Debug("Compressing database");
                _database.Compress();
            }
            finally
            {
                Monitor.Exit(_lock);
            }

        }

        public IEnumerable<string> GetTestsFor(string cacheName)
        {
            Debug("getting test counts for " + cacheName + " there are " + _counts.TotalMethods + " in count database.");
            return _counts.GetTestsFor(cacheName);
        }

        public IEnumerable<string> GetProfiledMethods()
        {
            return _counts.GetMethods();
        }

        public TestRunInformation GetTestInformationFor(string name)
        {
            return _database.LookUpByName(name);
        }

        public void Remove(string key)
        {
            if(!Monitor.TryEnter(_lock,5000))
            {
                throw new Exception("Unable to acquire lock");
            }
            try
            {
                _database.RemoveEntryIfExist(key);
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        public event EventHandler<ProfilerLogEventArgs> DebugMessage;

        private IEnumerable<ProfilerEntry> Printall(IEnumerable<ProfilerEntry> items)
        {
            foreach(var item in items)
            {
                Debug("'" + item.Method + "' isnewfixture=" + item.IsFixtureConstructor +" runtime=" + item.Runtime + " found=" + item.Found + " istest=" + item.IsTest + " su=" + item.IsSetup + " td=" + item.IsTeardown);
                yield return item;
            }
        }

        public void Debug(string message)
        {
            if(DebugMessage != null) DebugMessage(this, new ProfilerLogEventArgs(message));
        }

        public void Load()
        {
            lock (_lock)
            {
                _database.AttachProjectionWithSnapshotting(_counts);
            }
        }

        public void DeleteAllData()
        {
            _database.DeleteAllData();
        }
    }

    public class ProfilerLogEventArgs : EventArgs
    {
        public string Message { get; set; }

        public ProfilerLogEventArgs(string message)
        {
            Message = message;
        }
    }
}