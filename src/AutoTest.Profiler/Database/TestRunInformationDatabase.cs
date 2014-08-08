using System;
using System.Collections.Generic;
using System.IO;

namespace AutoTest.Profiler.Database
{
    public class TestRunInformationDatabase
    {
        private readonly Dictionary<string, TestRunInformation> _hash = new Dictionary<string, TestRunInformation>(10000);
        private Dictionary<string, FileLocation> _fileLocations = new Dictionary<string, FileLocation>();
        private readonly string _filename;
        private readonly bool _useCache = false; //GFY dont enable or snapshotting will get messed up
        private readonly List<ICouplingGraphProjection> _projections = new List<ICouplingGraphProjection>();
        private int _totalEntries;
        private long _fileWaste;
        private long _totalSize;


        public TestRunInformationDatabase(string filename, bool useCache)
        {
            _filename = filename;
            _useCache = useCache;
        }

        public TestRunInformationDatabase(string filename) : this(filename, false)
        {

        }

        public long FileWaste
        {
            get { return _fileWaste; }
        }
		
        public int TotalEntries
        {
            get { return _totalEntries; }
        }

        public long TotalSize
        {
            get { return _totalSize;  }
        }

        public void TakeSnapshot()
        {
            var snapshotfilename = _filename + ".idx";
            var filename = Path.Combine(Path.GetTempPath(), "snapshot" + Guid.NewGuid());
            try
            {
                lock (_hash)
                {
                    var lastposition = GetLastPosition();
                    using (var f = File.Open(filename, FileMode.OpenOrCreate))
                    {
                        var writer = new BinaryWriter(f);
                        writer.Write("v2");
                        writer.Write(lastposition);
                        writer.Write(_fileLocations.Count);
                        foreach (var item in _fileLocations.Keys)
                        {
                            if (item == null) continue;
                            writer.Write(item);
                            writer.Write(_fileLocations[item].Position);
                            writer.Write(_fileLocations[item].Length);
                        }
                        f.Close();
                        File.Delete(snapshotfilename);
                        File.Move(filename, snapshotfilename);
                    }

                    foreach (var projection in _projections)
                    {
                        projection.SnapShotTo(_filename + "." + projection.GetSnapshotExtension(), lastposition);
                    }
                }
            }
            finally
            {
               File.Delete(filename); 
            }
        }
         
        private long ReadFromSnapshot()
        {
            var snapshotfilename = _filename + ".idx";
            using (var file = File.Open(snapshotfilename, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                var reader = new BinaryReader(file);
                var version = reader.ReadString();
                if(version != "v2") throw new CorruptSnapshotException();
                var lastKnownPosition = reader.ReadInt64();
                var count = reader.ReadInt32();
                var tmp = new Dictionary<string, FileLocation>();
                for(var i=0;i<count;i++)
                {
                    var key = string.Intern(reader.ReadString());
                    var position = reader.ReadInt64();
                    var length = reader.ReadInt64();
                    tmp.Add(key, new FileLocation(length, position));
                }
                _totalEntries = count;
                 _fileLocations = tmp;
                return lastKnownPosition;
            }
        }

        private long GetLastPosition()
        {
            using (var f = File.Open(_filename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            {
                f.Seek(0, SeekOrigin.End);
                var pos = f.Position;
                f.Close();
                return pos;
            }
        }

        public void LoadFrom(long position)
        {
            try
            {
                using (var f = new FileStream(_filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 8096, FileOptions.SequentialScan))
                {
                    
                    f.Seek(position, SeekOrigin.Begin);
                    using (var reader = new BinaryReader(f))
                    {
                        while (f.Position < f.Length)
                        {
                            ReadIndividualItem(reader);
                        }
                        reader.Close();
                    }
                    f.Close();
                }
            }
            catch (Exception ex)
            {
                throw new CorruptedProfilerDatabaseException(ex);
            }
        }

        public void LoadWithSnapshot()
        {
            try
            {
                var known = ReadFromSnapshot();
                LoadFrom(known);
            }
            catch(Exception ex)
            {
                throw new CorruptSnapshotException();
            }
        }

        public void LoadAll()
        {
            LoadFrom(0);
        }

        private void AddOrUpdate(TestRunInformation item, FileLocation location)
        {
            lock (_hash)
            {
                if (_fileLocations.ContainsKey(item.Name))
                {
                    UpdateProjectionsRemoval(LookUpByName(item.Name));
                    _fileWaste += _fileLocations[item.Name].Length;
                    _fileLocations[item.Name] = location;
                    _totalSize += location.Length;
                    if (_useCache)
                        _hash[item.Name] = item;

                }
                else
                {
                    _fileLocations.Add(item.Name, location);
                    _totalEntries++;
                    _totalSize += location.Length;
                    if (_useCache)
                        _hash.Add(item.Name, item);
                }
            }
            UpdateProjections(item);
        }

        private void UpdateProjections(TestRunInformation item)
        {
            foreach (var p in _projections) p.Index(item);
        }

        private void UpdateProjectionsRemoval(TestRunInformation item)
        {
            foreach (var p in _projections) p.Remove(item);
        }

        public void RemoveEntryIfExist(string name)
        {
            WriteRemoval(name);
            Remove(name);
        }

        private void Remove(string name)
        {
            TestRunInformation info;
            var item = LookUpByName(name);
            if(item != null)
            {
                UpdateProjectionsRemoval(item);
            }
            if(_fileLocations.ContainsKey(name)) {
                _fileLocations.Remove(name);
                _fileWaste += _fileLocations[name].Length;
                _fileWaste += 2;
                _totalEntries--;
            }
            if(_useCache && _hash.ContainsKey(name))
                _hash.Remove(name);
        }

        private void WriteRemoval(string name)
        {
            using (var f = File.Open(_filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                f.Seek(0, SeekOrigin.End);
                var mem = new MemoryStream();
                using (var writer = new BinaryWriter(mem))
                {
                    writer.Write((byte) 1);
                    writer.Write(name);
                    
                    using (var fileWriter = new BinaryWriter(f))
                    {     
                        var position = mem.Position;
                        fileWriter.Write((Int32) position);
                        mem.Seek(0, SeekOrigin.Begin);
                        fileWriter.Flush();
                        CopyStream(mem, f, position, new byte[8096]);
                    }
                    writer.Close();
                }
                f.Close();
            }
        }

        public void AddNewEntries(IEnumerable<TestRunInformation> entries)
        {
            using (var f = new FileStream(_filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 8096, FileOptions.SequentialScan))
            {
                var buffer = new byte[8096];
                f.Seek(0, SeekOrigin.End);
                var memStream = new MemoryStream();
                using (var memwriter = new BinaryWriter(memStream))
                {
                    using (var writer = new BinaryWriter(f))
                    {
                        foreach (var entry in entries)
                        {
                            var location = f.Position;
                            memStream.SetLength(0);
                            WriteIndividualEntry(entry, memwriter);
                            //length
                            var length = memStream.Position;
                            writer.Write((Int32) length);
                            writer.Flush();
                            memStream.Seek(0, SeekOrigin.Begin);
                            CopyStream(memStream, f, length, buffer);
                            f.Flush();
                            AddOrUpdate(entry, new FileLocation(length, location));
                        }
                    }
                }
            }
        }

        private void CopyStream(Stream toCopy, Stream fileStream, long length, byte[] buffer)
        {
            int bytesRead = 1;
            // This will finish silently if we could not read length bytes.
            // An alternative would be to throw an exception
            while (length > 0 && bytesRead > 0)
            {
                bytesRead = toCopy.Read(buffer, 0, (int) Math.Min(length, buffer.Length));
                fileStream.Write(buffer, 0, bytesRead);
                length -= bytesRead;
            }
        }

        private void WriteIndividualEntry(TestRunInformation entry, BinaryWriter writer)
        {
            try
            {
                writer.Write((byte) 2);
                writer.Write((string) entry.Name);
                writer.Write((int) entry.Setups.Count);
                writer.Write((int) entry.Teardowns.Count);
                writer.Write((bool) (entry.TestChain != null));
                foreach (var setup in entry.Setups)
                {
                    WriteChainItem(writer, setup);
                }
                foreach (var teardown in entry.Teardowns)
                {
                    WriteChainItem(writer, teardown);
                }
                if (entry.TestChain != null)
                {
                    WriteChainItem(writer, entry.TestChain);
                }
            }
            catch(Exception ex)
            {
                throw new WritingEntryFailedException("writing failed for entry: " + entry);
            }
        }


        private TestRunInformation ReadInformationBody(BinaryReader reader, string name)
        {
            var setups = reader.ReadInt32();
            var teardowns = reader.ReadInt32();
            var hasTest = reader.ReadBoolean();
            var ret = new TestRunInformation {Name = name};
            for (int i = 0; i < setups; i++)
            {
                ret.AddSetup(ReadChainItem(reader));
            }
            for (int i = 0; i < teardowns; i++)
            {
                ret.AddTearDown(ReadChainItem(reader));
            }
            if(hasTest)
                ret.TestChain = ReadChainItem(reader);
            return ret;
        }

        private byte ReadHeader(BinaryReader reader, out string name)
        {
            var type = reader.ReadByte();
            name = reader.ReadString();
            return type;
        }

        private void ReadIndividualItem(BinaryReader reader)
        {
            
            var location = reader.BaseStream.Position;
            var length = reader.ReadInt32();
            string name;
            var type = ReadHeader(reader, out name);
            if(type == 1)
            {
                Remove(name);
            }
            if (type == 2)
            {
                var item = ReadInformationBody(reader, name);
                AddOrUpdate(item, new FileLocation(length, location));
            }
        }

        private static void WriteChainItem(BinaryWriter writer, CallChain chain)
        {
            writer.Write((string) chain.Name);
            //writer.Write((string)chain.Runtime);
            writer.Write((Int64) chain.FunctionId);
            writer.Write((int) chain.Children.Count);
            writer.Write((double)chain.StartTime);
            writer.Write((double)chain.EndTime);
            foreach(var child in chain.Children)
            {
                WriteChainItem(writer, child);
            }
        }

        private static CallChain ReadChainItem(BinaryReader reader)
        {
            var name = string.Intern(reader.ReadString());
            //var runtime = reader.ReadString();
            var functionId = reader.ReadInt64();

            var item = new CallChain(name, "", functionId);
            
            var count = reader.ReadInt32();
            var start = reader.ReadDouble();
            var end = reader.ReadDouble();
            item.StartTime = start;
            item.EndTime = end; 
            for (int i = 0; i < count; i++)
            {
                item.AddChild(ReadChainItem(reader));
            }
            return item;
        }

        public void AttachProjection(ICouplingGraphProjection projection)
        {
            _projections.Add(projection);
        }

        public TestRunInformation LookUpByName(string name)
        {
            TestRunInformation info = null;
            if (_useCache)
            {
                _hash.TryGetValue(name, out info);
            }
            else
            {
                FileLocation location;
                if(_fileLocations.TryGetValue(name, out location))
                {
                    using (var f = File.Open(_filename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using(var reader = new BinaryReader(f))
                        {
                            f.Seek(location.Position, SeekOrigin.Begin);
                            string itemName;
                            reader.ReadInt32();
                            var type = ReadHeader(reader, out itemName);
                            if (type == 2)
                            {
                                info = ReadInformationBody(reader, name);
                            }
                        }
                    }
                }
            }
            return info;
        }

        public void Compress()
        {
            var filename = Path.Combine(Path.GetTempPath(), "compress" + Guid.NewGuid());
            var newFilePositions = new Dictionary<string, FileLocation>();
            var buffer = new byte[8096];
            try
            {
                using (
                    var original = new FileStream(_filename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read,
                                                  8096, FileOptions.RandomAccess))
                {
                    using (
                        var f = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 8096,
                                               FileOptions.SequentialScan))
                    {
                        using (var reader = new BinaryReader(original))
                        {
                            f.SetLength(0);
                            using (var writer = new BinaryWriter(f))
                            {
                                var items = _fileLocations;
                                foreach (var pair in items)
                                {
                                    original.Seek(pair.Value.Position, SeekOrigin.Begin);
                                    var length = reader.ReadInt32();
                                    var location = f.Position;
                                    writer.Write(length);
                                    CopyStream(original, f, length, buffer);
                                    newFilePositions[pair.Key] = new FileLocation(length, location);
                                }
                            }
                        }
                    }
                }
                File.Copy(filename, _filename, true);
                _fileLocations = newFilePositions;
            }
            finally
            {
                File.Delete(filename);
                _fileWaste = 0;
            }
        }

        public void AttachProjectionWithSnapshotting(ICouplingGraphProjection projection)
        {
            //TODO GFY SUPPPORT MANY PROJECTIONS
            try
            {
                var filename = _filename + "." + projection.GetSnapshotExtension();
                var position = projection.LoadFromSnapshot(filename);
                //ok so we have loaded the projection lets to load us
                //TODO we can be smarter about this.
                _projections.Add(projection);
                LoadWithSnapshot();
            }
            catch(Exception ex)
            {
                _projections.Clear();
                _projections.Add(projection);
                LoadAll();
            }
        }

        public void DeleteAllData()
        {
            File.Delete(_filename);
            File.Delete(_filename + ".idx");
            foreach(var projection in _projections)
            {
                File.Delete(_filename + "." + projection.GetSnapshotExtension());
            }
            Clear();
        }

        private void Clear()
        {
            _hash.Clear();
            _fileLocations.Clear();
            foreach(var p in _projections)
            {
                p.Clear();
            }
            Compress();
        }
    }

    public class CorruptSnapshotException : Exception
    {
    }

    internal class WritingEntryFailedException : Exception
    {
        public WritingEntryFailedException(string s) : base(s) {}
    }
}