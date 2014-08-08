namespace AutoTest.Profiler.Database
{
    public class FileLocation
    {
        public readonly long Position;
        public readonly long Length;

        public FileLocation(long length, long position)
        {
            Length = length;
            Position = position;
        }
    }
}