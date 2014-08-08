using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_iterating_calls_in_test_information_with_setup
    {
        private List<string> items;

        [SetUp]
        public void Setup()
        {
            var info = new TestRunInformation();
            info.TestChain = TestData.BuildChainWithPrefix("Test");
            info.AddSetup(TestData.BuildChainWithPrefix("Setup"));
            items = new List<string>(info.IterateAll());
            
        }

        [Test]
        public void ten_items_are_walked()
        {
            Assert.AreEqual(10, items.Count);
        }

        [Test]
        public void setup_is_walked_before_test()
        {
            Assert.AreEqual("SetupRoot", items[0]);
        }

        [Test]
        public void test_is_walked()
        {
            Assert.AreEqual("TestRoot", items[5]);
        }
    }
}