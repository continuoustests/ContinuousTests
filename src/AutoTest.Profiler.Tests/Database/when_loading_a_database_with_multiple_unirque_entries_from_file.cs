using System.IO;
using AutoTest.Profiler.Database;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Database
{
    [TestFixture]
    public class when_loading_a_database_with_multiple_unirque_entries_from_file
    {
        private string filename;
        private TestRunInformationDatabase db;

        [SetUp]
        public void SetUp()
        {
            filename = Path.GetTempFileName();
            db = new TestRunInformationDatabase(filename);
            db.AddNewEntries(new[] { 
                                       TestData.BuildTestInformatonFor("Test1", "test"),
                                       TestData.BuildTestInformatonFor("Test2", "test"),
                                       TestData.BuildTestInformatonFor("Test3", "test")
                                   });
            db = new TestRunInformationDatabase(filename);
            db.LoadAll();
        }

        [Test]
        public void should_be_three_distint_entries()
        {
            Assert.AreEqual(3, db.TotalEntries);
        }

        [Test]
        public void should_be_no_file_waste()
        {
            Assert.AreEqual(0, db.FileWaste);
        }

        [TearDown]
        public void Cleanup()
        {
            File.Delete(filename);
        }
    }
}