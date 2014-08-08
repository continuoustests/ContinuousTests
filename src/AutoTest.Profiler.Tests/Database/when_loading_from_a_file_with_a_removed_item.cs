using System.IO;
using AutoTest.Profiler.Database;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Database
{
    [TestFixture]
    public class when_loading_from_a_file_with_a_removed_item 
    {
        private string filename;
        private TestRunInformationDatabase db;

        [SetUp]
        public void SetUp()
        {
            filename = Path.GetTempFileName();
            db = new TestRunInformationDatabase(filename);
            db.AddNewEntries(new[] { TestData.BuildTestInformatonFor("Test1", "test") });
            db.RemoveEntryIfExist("Test1");
            db = new TestRunInformationDatabase(filename);
            db.LoadAll();
        }

        [Test]
        public void it_should_not_be_in_the_database_after()
        {
            Assert.IsNull(db.LookUpByName("Test1"));
        }

        [Test]
        public void waste_should_be_updated()
        {
            Assert.AreEqual(215, db.FileWaste);
        }

        [Test]
        public void total_entries_in_database_should_be_zero()
        {
            Assert.AreEqual(0, db.TotalEntries);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(filename);
        }
    }
}