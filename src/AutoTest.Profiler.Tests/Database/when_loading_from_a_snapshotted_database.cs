using System.IO;
using AutoTest.Profiler.Database;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Database
{
    [TestFixture]
    public class when_loading_from_a_snapshotted_database
    {
        private string filename;
        private TestRunInformationDatabase db;

        [SetUp]
        public void SetUp()
        {
            filename = Path.GetTempFileName();
            db = new TestRunInformationDatabase(filename);
            db.AddNewEntries(new[] { TestData.BuildTestInformatonFor("Test1", "test") });
            db.AddNewEntries(new[] { TestData.BuildTestInformatonFor("Test2", "test") });
            db.AddNewEntries(new[] { TestData.BuildTestInformatonFor("Test3", "test") });
            db.TakeSnapshot();
            db = new TestRunInformationDatabase(filename);
            db.LoadWithSnapshot();
        }

        [Test]
        public void three_entries_are_loaded()
        {
            Assert.AreEqual(3, db.TotalEntries);
        }

        [Test]
        public void can_find_first_item()
        {
            Assert.IsNotNull(db.LookUpByName("Test1"));
        }

        [Test]
        public void can_find_second_item()
        {
            Assert.IsNotNull(db.LookUpByName("Test2"));
        }

        [Test]
        public void can_find_third_item()
        {
            Assert.IsNotNull(db.LookUpByName("Test3"));
        }

        [TearDown]
        public void Teardown()
        {
            File.Delete(filename);
            File.Delete(filename + ".idx");
        }
    }
}