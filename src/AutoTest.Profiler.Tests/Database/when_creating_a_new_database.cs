using System.IO;
using AutoTest.Profiler.Database;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Database
{
    [TestFixture]
    public class when_creating_a_new_database
    {
        private string filename;
        private TestRunInformationDatabase db;

        [SetUp]
        public void Setup()
        {
            filename = Path.GetTempFileName();
            db = new TestRunInformationDatabase(filename);
        }

        [Test]
        public void there_are_no_entries_in_the_database()
        {
            Assert.AreEqual(0, db.TotalEntries);
        }

        [Test]
        public void lookup_returns_null_for_non_existent_item()
        {
            Assert.IsNull(db.LookUpByName("greg"));
        }

        [TearDown]
        public void Teardown()
        {
            File.Delete(filename);
        }
    }
}