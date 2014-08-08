using System.Collections.Generic;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_iterating_calls_in_test_information
    {
        private List<string> items;

        [SetUp]
        public void Setup()
        {
            var info = new TestRunInformation();
            info.TestChain = TestData.BuildChainWithPrefix("Test");
            items = new List<string>(info.IterateAll());
        }

        [Test]
        public void items_in_test_are_walked()
        {
            Assert.AreEqual(5, items.Count);
        }
    }
}