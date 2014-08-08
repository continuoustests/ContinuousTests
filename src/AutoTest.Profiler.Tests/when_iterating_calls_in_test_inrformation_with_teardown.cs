using System.Collections.Generic;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_iterating_calls_in_test_inrformation_with_teardown
    {
        private List<string> items;

        [SetUp]
        public void Setup()
        {
            var info = new TestRunInformation();
            info.TestChain = TestData.BuildChainWithPrefix("Test");
            info.AddTearDown(TestData.BuildChainWithPrefix("TD"));
            items = new List<string>(info.IterateAll());
        }

        [Test]
        public void ten_items_are_walked()
        {
            Assert.AreEqual(10, items.Count);
        }

        [Test]
        public void test_is_walked_before_teardown()
        {
            Assert.AreEqual("TestRoot", items[0]);
        }

        [Test]
        public void teardown_is_walked_after_test()
        {
            Assert.AreEqual("TDRoot", items[5]);
        }
    }
}