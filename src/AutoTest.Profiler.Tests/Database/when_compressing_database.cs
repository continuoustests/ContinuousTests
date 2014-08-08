using System.IO;
using AutoTest.Profiler.Database;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Database
{
    [TestFixture]
    public class when_compressing_database
    {
        private string filename;
        private TestRunInformationDatabase db
                                           ;

        [SetUp]
        public void SetUp()
        {
            filename = Path.GetTempFileName();
            db = new TestRunInformationDatabase(filename);
            db.AddNewEntries(new[] { TestData.BuildTestInformatonFor("Test1", "test") });
            db.AddNewEntries(new[] { TestData.BuildTestInformatonFor("Test1", "test") });
            db.Compress();
        }

        [Test]
        public void entry_count_stays_the_same()
        {
            Assert.AreEqual(1,db.TotalEntries);
        }

        [Test]
        public void waste_becomes_zero()
        {
            Assert.AreEqual(0, db.FileWaste);
        }

        [Test]
        public void can_look_up_pre_existing_item()
        {
            Assert.IsNotNull(db.LookUpByName("Test1"));
        }

        [TearDown]
        public void teardown()
        {
            File.Delete(filename);
        }

    }
}