using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_iterating_all_items_in_call_chain
    {
        private List<string> items;

        [SetUp]
        public void SetUp()
        {
            var chain = TestData.BuildChainWithPrefix("");
            items = new List<string>(chain.IterateAll());
        }

        [Test]
        public void five_items_are_walked()
        {
            Assert.AreEqual(5, items.Count);
        }

        [Test]
        public void root_node_is_first()
        {
            Assert.AreEqual(items[0], "Root");
        }

        [Test]
        public void children_are_in_order()
        {
            Assert.AreEqual(items[1], "Child1");
            Assert.AreEqual(items[2], "Child2");
            Assert.AreEqual(items[3], "Child3");
        }

        [Test]
        public void grandchildren_are_found()
        {
            Assert.AreEqual(items[4], "GrandChild");
        }
    }
}