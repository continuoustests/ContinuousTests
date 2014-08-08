using System.Collections.Generic;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_iterating_calls_in_test_information_with_multiple_setups
    {
        private List<string> items;

        [SetUp]
        public void Setup()
        {
            var info = new TestRunInformation();
            info.TestChain = TestData.BuildChainWithPrefix("Test");
            info.AddSetup(TestData.BuildChainWithPrefix("Setup"));
            info.AddSetup(TestData.BuildChainWithPrefix("Setup2"));
            items = new List<string>(info.IterateAll());
        }

        [Test]
        public void fifteen_items_are_walked()
        {
            Assert.AreEqual(15, items.Count);
        }

        [Test]
        public void setups_are_walked_before_test()
        {
            Assert.AreEqual("SetupRoot", items[0]);
            Assert.AreEqual("Setup2Root", items[5]);
        }

        [Test]
        public void test_is_walked()
        {
            Assert.AreEqual("TestRoot", items[10]);
        }
    }
}