using System.IO;
using AutoTest.Profiler.Database;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Database
{
    [TestFixture]
    public class when_loading_from_a_snapshotted_database_with_corrupted_file
    {
        private string filename;
        private TestRunInformationDatabase db;

        [Test, ExpectedException(typeof(CorruptSnapshotException))]
        public void a_corrupted_database_exception_is_thrown()
        {
            filename = Path.GetTempFileName();
            db = new TestRunInformationDatabase(filename);
            db.AddNewEntries(new[] { TestData.BuildTestInformatonFor("Test1", "test") });
            db.AddNewEntries(new[] { TestData.BuildTestInformatonFor("Test2", "test") });
            db.AddNewEntries(new[] { TestData.BuildTestInformatonFor("Test3", "test") });
            db.TakeSnapshot();
            File.Delete(filename + ".idx");
            File.Copy(filename, filename + ".idx"); //copying original profiler db over the top should cause some issues
            db = new TestRunInformationDatabase(filename);
            db.LoadWithSnapshot();
        }

        [TearDown]
        public void Teardown()
        {
            File.Delete(filename);
            File.Delete(filename + ".idx");
        }
    }
}