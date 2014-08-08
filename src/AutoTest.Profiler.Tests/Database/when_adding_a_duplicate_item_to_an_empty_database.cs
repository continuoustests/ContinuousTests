using System.IO;
using AutoTest.Profiler.Database;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Database
{
    [TestFixture]
    public class when_adding_a_duplicate_item_to_an_empty_database
    {
        private string filename;
        private TestRunInformationDatabase db;

        [SetUp]
        public void SetUp()
        {
            filename = Path.GetTempFileName();
            db = new TestRunInformationDatabase(filename);
            db.AddNewEntries(new[] { TestData.BuildTestInformatonFor("Test1", "test") });
            db.AddNewEntries(new[] { TestData.BuildTestInformatonFor("Test1", "test") });
        }

        [Test]
        public void only_one_entry_is_present()
        {
            Assert.AreEqual(1, db.TotalEntries);
        }

        [Test]
        public void file_waste_increases_by_amount_of_original_item()
        {
            Assert.AreEqual(213, db.FileWaste);
        }

        [Test]
        public void total_size_includes_both_items()
        {
            Assert.AreEqual(426, db.TotalSize);
        }

        [Test]
        public void the_entry_can_be_looked_up()
        {
            Assert.AreEqual("Test1", db.LookUpByName("Test1").Name);
        }

        [TearDown]
        public void Teardown()
        {
            File.Delete(filename);
        }
    }
}