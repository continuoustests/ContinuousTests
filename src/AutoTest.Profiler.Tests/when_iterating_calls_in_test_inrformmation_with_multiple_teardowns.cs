using System.Collections.Generic;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_iterating_calls_in_test_inrformmation_with_multiple_teardowns
    {
        private List<string> items;

        [SetUp]
        public void Setup()
        {
            var info = new TestRunInformation();
            info.TestChain = TestData.BuildChainWithPrefix("Test");
            info.AddTearDown(TestData.BuildChainWithPrefix("TD"));
            info.AddTearDown(TestData.BuildChainWithPrefix("TD2"));
            items = new List<string>(info.IterateAll());
        }

        [Test]
        public void fifteen_items_are_walked()
        {
            Assert.AreEqual(15, items.Count);
        }

        [Test]
        public void test_is_walked_before_teardowns()
        {
            Assert.AreEqual("TestRoot", items[0]);
        }

        [Test]
        public void both_teardowns_are_walked_after_test()
        {
            Assert.AreEqual("TDRoot", items[5]);
            Assert.AreEqual("TD2Root", items[10]);
        }
    }
}