using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoTest.Profiler.Database;

namespace AutoTest.Profiler
{
    public class CouplingCountAndNameProjection : ICouplingGraphProjection
    {
        private Dictionary<string, Dictionary<string, bool>> _linkHash = new Dictionary<string, Dictionary<string, bool>>();
        private Dictionary<string, CountsAndTimes> _timesHash = new Dictionary<string, CountsAndTimes>();

        public object TotalMethods
        {
            get { return _linkHash.Count; }
        }

        public string GetSnapshotExtension()
        {
            return "cnt";
        }

        public void Index(TestRunInformation testInformation)
        {
            foreach(var item in testInformation.IterateNodes())
            {
                Index(testInformation.Name, item);
            }
        }

        private void Index(string test, CallChain chain)
        {
            string item = chain.Name.Replace('+', '/');
            
            Dictionary<string, bool> tests;
            if(!_linkHash.TryGetValue(item, out tests))
            {
                tests = new Dictionary<string, bool>();
                _linkHash.Add(item, tests);
            }
            CountsAndTimes counts;
            if (!_timesHash.TryGetValue(item, out counts))
            {
                counts = new CountsAndTimes();
                _timesHash.Add(item, counts);
            }
            if(!tests.ContainsKey(test))
            {
                tests.Add(test, false);
            }
            counts.ProcessNewEntry(chain);
        }

        private void Remove(string test, string item)
        {
            Dictionary<string, bool> tests;
            if (_linkHash.TryGetValue(item.Replace('+', '/'), out tests))
            {
                tests.Remove(test);
            }
            
        }

        private void RemoveCounts(TestRunInformation testInformation)
        {
            foreach (var item in testInformation.IterateNodes())
            {
                CountsAndTimes c;
                if (_timesHash.TryGetValue(item.Name.Replace('+', '/'), out c))
                {
                    c.RemoveEntry(item);
                }
            }
        }

        public void Remove(TestRunInformation testInformation)
        {
            if (testInformation == null) return;
            if (testInformation.Name == null) return;
            RemoveCounts(testInformation);

            foreach (var item in testInformation.IterateNodes())
            {
                
                Remove(testInformation.Name.Replace('+', '/'), item.Name.Replace('+', '/'));
            }            
        }

        public CountsAndTimes GetRuntimeCallTimingsFor(string method)
        {
            CountsAndTimes info;
            if (!_timesHash.TryGetValue(method.Replace('+', '/'), out info)) return CountsAndTimes.Empty;
            return info;
        
        }

        public int GetTestCountFor(string method)
        {
            Dictionary<string, bool> tests;
            if (!_linkHash.TryGetValue(method.Replace('+', '/'), out tests)) return 0;
            return tests.Count;
        }

        public List<string> GetTestsFor(string method)
        {
            Dictionary<string, bool> tests;
            var ret = new List<string>();
            if (_linkHash.TryGetValue(method.Replace('+', '/'), out tests))
            {
                ret.AddRange(tests.Select(e => e.Key));
            }
            return ret;
        }

        public IEnumerable<string> GetMethods()
        {
            return _linkHash.Keys;
        }

        public void SnapShotTo(string filename, long marker)
        {
            var snapshotfilename = Path.GetTempFileName();
            try
            {
                using (var file = File.Open(snapshotfilename, FileMode.OpenOrCreate))
                {
                    file.SetLength(0);
                    lock (_linkHash)
                    {
                        var writer = new BinaryWriter(file);
                        writer.Write("v1");
                        writer.Write(marker);
                        //write tests
                        writer.Write(_linkHash.Count);
                        foreach (var item in _linkHash)
                        {
                            writer.Write(item.Key);
                            writer.Write(item.Value.Count);
                            foreach (var link in item.Value.Keys)
                            {
                                writer.Write(link);
                            }
                        }
                        //write counts
                        writer.Write(_timesHash.Count);
                        foreach (var entry in _timesHash)
                        {
                            writer.Write(entry.Key);
                            writer.Write(entry.Value.MaxTime);
                            writer.Write(entry.Value.MaxTimeUnder);
                            writer.Write(entry.Value.MinTime);
                            writer.Write(entry.Value.MinTimeUnder);
                            writer.Write(entry.Value.TimesCalled);
                            writer.Write(entry.Value.TotalTime);
                            writer.Write(entry.Value.TotalTimeUnder);
                        }
                    }
                }
                File.Delete(filename);
                File.Move(snapshotfilename, filename);
            }
            finally
            {
                File.Delete(snapshotfilename);
            }
        }

        public long LoadFromSnapshot(string filename)
        {
            try
            {
                using(var file = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var reader = new BinaryReader(file);
                    var version = reader.ReadString();
                    if(version != "v1") throw new CorruptSnapshotException();
                    var lastKnown = reader.ReadInt64();
                    var totalLinks = reader.ReadInt32();
                    var hash = new Dictionary<string, Dictionary<string, bool>>();
                    for(int i=0;i<totalLinks;i++)
                    {
                        var from = reader.ReadString();
                        var linksCount = reader.ReadInt32();
                        var entry = new Dictionary<string, bool>();
                        for(int j=0;j<linksCount;j++)
                        {
                            var to = reader.ReadString();
                            entry.Add(to, true);
                        }
                        hash.Add(from, entry);
                    }
                    var timesCount = reader.ReadInt32();
                    var times = new Dictionary<string, CountsAndTimes>();
                    for(int q=0;q<timesCount;q++)
                    {
                        var name = reader.ReadString();
                        var maxtime = reader.ReadDouble();
                        var maxtimeu = reader.ReadDouble();
                        var mintime = reader.ReadDouble();
                        var mintimeu = reader.ReadDouble();
                        var timescalled = reader.ReadInt32();
                        var totaltime = reader.ReadDouble();
                        var totaltimeu = reader.ReadDouble();
                        times.Add(name, new CountsAndTimes(timescalled, totaltime, totaltimeu,
                                                       maxtime, maxtimeu, mintime, mintimeu));
                        
                    }

                    _timesHash = times;
                    _linkHash = hash;
                    return lastKnown;
                }

            }
            catch(Exception ex)
            {
                File.Delete(filename);
                throw new CorruptedCountsProjectionSnapshotException(ex);
            }
        }

        public void Clear()
        {
            _linkHash.Clear();
            _timesHash.Clear();
        }
    }

    public class CorruptedCountsProjectionSnapshotException : Exception
    {
        public CorruptedCountsProjectionSnapshotException(Exception innerException) : base("Corrupted counts snapshot", innerException)
        {
            
        }
    }

    public static class CouplingGraphProjectionExtensions
    {
        public static void ReIndexAll(this ICouplingGraphProjection projection, IEnumerable<TestRunInformation> testInformations)
        {
            foreach (var p in testInformations) projection.Index(p);
        }
    }
}