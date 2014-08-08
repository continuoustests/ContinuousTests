using System;
using System.Collections.Generic;
using System.IO;

namespace AutoTest.Profiler
{
    public class StreamProfilerParser : IProfilerDataParser
    {
        public IEnumerable<ProfilerEntry> Parse(Stream stream)
        {
            using(var reader = new BinaryReader(stream))
            {
                while (reader.PeekChar() != -1)
                {
                    var entry = new ProfilerEntry();
                    entry.Type = (ProfileType) reader.ReadByte();
                    entry.Sequence = reader.ReadInt32();
                    entry.Thread = reader.ReadInt32();
                    entry.Functionid = reader.ReadInt64();
                    entry.Time = reader.ReadDouble();
                    if (entry.Type == ProfileType.Enter)
                    {
                        var length = reader.ReadInt32();
                        entry.Method = new string(reader.ReadChars(length));
                        length = reader.ReadInt32();
                        entry.Runtime = new string(reader.ReadChars(length));
                    }
                    yield return entry;
                }
            }
        }
    }
}
