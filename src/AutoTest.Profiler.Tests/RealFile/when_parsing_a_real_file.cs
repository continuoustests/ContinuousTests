using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoTest.Profiler.Database;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.RealFile
{
    [TestFixture]
    public class when_parsing_a_real_file
    {
        private List<ProfilerEntry> items;

        [SetUp]
        public void SetUp()
        {
            using (var f = File.Open("RealFile\\MMProfiler.log", FileMode.Open))
            {
                var parser = new BinaryFileProfilerDataParser();
                items = parser.Parse(f).ToList();
            }
        }

        [Test]
        public void the_correct_number_of_items_read_from_the_file()
        {
            Assert.AreEqual(18055, items.Count);
        }
    }
    //[TestFixture]
    //public class when_loading_a_real_file_into_database
    //{
    //    private List<ProfilerEntry> items;

    //    private TestRunInformationDatabase db;

    //    [SetUp]
    //    public void SetUp()
    //    { 
    //        db = new TestRunInformationDatabase("RealFile\\MMProfiler.log");
    //        db.LoadAll();
    //    }

    //    [Test]
    //    public void counts_get_initialized()
    //    {
    //        Assert.AreEqual(1300, db.TotalEntries);
    //    }
    //}
}
