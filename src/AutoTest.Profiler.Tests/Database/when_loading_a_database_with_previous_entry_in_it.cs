using System;
using System.IO;
using AutoTest.Profiler.Database;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Database
{
    [TestFixture]
    public class when_loading_a_database_with_previous_entry_in_it
    {
        private string filename;
        private TestRunInformationDatabase db;

        [SetUp]
        public void SetUp()
        {
            filename = Path.GetTempFileName();
            db = new TestRunInformationDatabase(filename);
            db.AddNewEntries(new[] { TestData.BuildTestInformatonFor("Test1", "test") });
            db = new TestRunInformationDatabase(filename);
            db.LoadAll();
        }

        [Test]
        public void only_one_entry_is_present()
        {
            Assert.AreEqual(1, db.TotalEntries);
        }

        [Test]
        public void the_entry_can_be_looked_up()
        {
            Assert.AreEqual("Test1", db.LookUpByName("Test1").Name);
        }

        [Test]
        public void the_information_in_the_entry_is_correct()
        {
            var item = db.LookUpByName("Test1");
            Assert.AreEqual("testRoot", item.TestChain.Name);
            Assert.AreEqual("testChild1", item.TestChain.Children[0].Name);
            Assert.AreEqual("testChild2", item.TestChain.Children[1].Name);
            Assert.AreEqual("testChild3", item.TestChain.Children[2].Name);
            Assert.AreEqual("testGrandChild", item.TestChain.Children[2].Children[0].Name);
        }

        [TearDown]
        public void Teardown()
        {
            File.Delete(filename);
        }
    }
}