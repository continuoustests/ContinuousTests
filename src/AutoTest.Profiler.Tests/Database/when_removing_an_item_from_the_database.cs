using System;
using System.IO;
using AutoTest.Profiler.Database;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Database
{
    [TestFixture]
    public class when_removing_an_item_from_the_database
    {
        private string filename;
        private TestRunInformationDatabase db;
        private TestProjection projection;

        [SetUp]
        public void SetUp()
        {
            filename = Path.GetTempFileName();
            db = new TestRunInformationDatabase(filename);
            projection = new TestProjection();
            db.AddNewEntries(new[] { TestData.BuildTestInformatonFor("Test1", "test") });
            db.AttachProjection(projection);
            db.RemoveEntryIfExist("Test1");
        }

        [Test]
        public void it_should_not_be_in_the_database_after()
        {
            Assert.IsNull(db.LookUpByName("Test1"));
        }

        [Test]
        public void waste_should_increment_for_removed_item()
        {
            Assert.AreEqual(215, db.FileWaste);
        }

        [Test]
        public void total_entries_in_database_should_decrease_by_one()
        {
            Assert.AreEqual(0, db.TotalEntries);
        }

        [Test]
        public void projections_get_alerted_for_remove()
        {
            Assert.AreEqual(1, projection.RemoveCalledCount);
        }


        [Test]
        public void projections_index_not_called()
        {
            Assert.AreEqual(0, projection.IndexCalledCount);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(filename);
        }
    }
}