using System;
using System.IO;
using AutoTest.Profiler.Database;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Database
{
    [TestFixture]
    public class when_loading_an_empty_database
    {
        private string filename;
        private TestRunInformationDatabase db;

        [SetUp]
        public void SetUp()
        {
            filename = Path.GetTempFileName();
            db = new TestRunInformationDatabase(filename);
            db.LoadAll();
        }

        [Test]
        public void no_entries_are_loaded()
        {
            Assert.AreEqual(0, db.TotalEntries);
        }

        [Test]
        public void total_size_is_zero()
        {
            Assert.AreEqual(0, db.TotalSize);
        }

        [Test]
        public void no_wasted_entries_are_found()
        {
            Assert.AreEqual(0,db.FileWaste);
        }

        [TearDown]
        public void Teardown()
        {
            File.Delete(filename);
        }
    }
}