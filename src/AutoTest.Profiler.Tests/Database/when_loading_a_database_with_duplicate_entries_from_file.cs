using System.IO;
using AutoTest.Profiler.Database;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Database
{
    [TestFixture]
    public class when_loading_a_database_with_duplicate_entries_from_file
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
                                       TestData.BuildTestInformatonFor("Test1", "test2"),
                                       TestData.BuildTestInformatonFor("Test1", "test3")
                                   });
            db = new TestRunInformationDatabase(filename);
            db.LoadAll();
        }

        [Test]
        public void entry_count_should_not_include_duplicates()
        {
            Assert.AreEqual(1, db.TotalEntries);
        }
        
        [Test]
        public void the_entry_should_contain_data_of_the_last_update()
        {
            var item = db.LookUpByName("Test1");
            Assert.AreEqual("test3Root", item.TestChain.Name);
            Assert.AreEqual("test3Child1", item.TestChain.Children[0].Name);
            Assert.AreEqual("test3Child2", item.TestChain.Children[1].Name);
            Assert.AreEqual("test3Child3", item.TestChain.Children[2].Name);
            Assert.AreEqual("test3GrandChild", item.TestChain.Children[2].Children[0].Name);
        }

        [Test]
        public void file_waste_is_updated()
        {
            Assert.AreEqual(431, db.FileWaste);
        }

        [TearDown]
        public void Cleanup()
        {
            File.Delete(filename);
        }
    }
}