using System.IO;
using AutoTest.Profiler.Database;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Database
{
    [TestFixture]
    public class when_adding_an_item_to_an_empty_database
    {
        private string filename;
        private TestRunInformationDatabase db;
        private TestProjection projection;

        [SetUp]
        public void SetUp()
        {
            filename = Path.GetTempFileName();
            projection = new TestProjection();
            db = new TestRunInformationDatabase(filename);
            db.AttachProjection(projection);
            db.AddNewEntries(new[] { TestData.BuildTestInformatonFor("Test1", "test") });
        }

        [Test]
        public void only_one_entry_is_present()
        {
            Assert.AreEqual(1, db.TotalEntries);
        }

        [Test]
        public void file_waste_is_zero()
        {
            Assert.AreEqual(0, db.FileWaste);
        }

        [Test]
        public void file_size_is_set()
        {
            Assert.AreEqual(213, db.TotalSize);
        }

        [Test]
        public void the_entry_can_be_looked_up()
        {
            Assert.AreEqual("Test1", db.LookUpByName("Test1").Name);
        }

        [Test]
        public void projection_is_alerted_to_index()
        {
            Assert.AreEqual(1, projection.IndexCalledCount);
        }

        [Test]
        public void projection_is_not_told_to_remove()
        {
            Assert.AreEqual(0, projection.RemoveCalledCount);
        }

        [TearDown]
        public void Teardown()
        {
            File.Delete(filename);
        }
    }
}