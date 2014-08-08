using System;

namespace AutoTest.Profiler.Database
{
    public class CorruptedProfilerDatabaseException : Exception
    {
        public CorruptedProfilerDatabaseException(Exception inner) : base("Profiler database is unloadable", inner)
        {
        }
    }
}