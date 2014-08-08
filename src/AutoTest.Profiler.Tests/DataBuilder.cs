using System.IO;

namespace AutoTest.Profiler.Tests
{
    public class DataBuilder
    {
        public static Stream Create(params EntryBuilder[] entries)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            foreach(var entry in entries)
            {
                entry.WriteTo(writer);
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}