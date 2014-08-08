using System.IO;
using AutoTest.Profiler.Database;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Database
{
    [TestFixture]
    public class when_loading_an_invalid_database_file
    {
        private string filename;

        [Test, ExpectedException(typeof(CorruptedProfilerDatabaseException))]
        public void a_corrupt_database_exception_is_thrown()
        {
            filename = Path.GetTempFileName();
            CreateJunkFile(filename);
            var db = new TestRunInformationDatabase(filename);
            db.LoadAll();
        }

        private static void CreateJunkFile(string file)
        {
            using (var f = File.Open(file, FileMode.Append))
            {
                using(var s = new StreamWriter(f))
                {
                    s.WriteLine("this is a test");
                    s.WriteLine("of some bad data it cant read properly");
                }
            }
        }

        [TearDown]
        public void cleanup()
        {
            File.Delete(filename);
        }
    }
}