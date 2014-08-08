using System.Collections.Generic;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests {
    [TestFixture]
    public class MapKeyDifferenceFinderTests
    {
        [Test]
        public void CanFindAddedItem()
        {
            var oldData = new Dictionary<string, int>();
            var newData = new Dictionary<string, int>();
            oldData.Add("test", 1);
            oldData.Add("foo", 2);
            newData.Add("test", 1);
            newData.Add("foo", 2);
            newData.Add("greg", 3);
            var changes = MapKeyDifferenceFinder.GetChangesBetween(oldData, newData);
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(ChangeType.Add, changes[0].ChangeType);
            Assert.AreEqual(3, changes[0].ItemChanged);
        }

        [Test]
        public void CanFindRemovedItem()
        {
            var newData = new Dictionary<string, int>();
            var oldData = new Dictionary<string, int>();
            oldData.Add("test", 1);
            oldData.Add("foo", 2);
            oldData.Add("greg", 3);
            newData.Add("test", 1);
            newData.Add("foo", 2);
            var changes = MapKeyDifferenceFinder.GetChangesBetween(newData, oldData);
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(ChangeType.Add, changes[0].ChangeType);
            Assert.AreEqual(3, changes[0].ItemChanged);
        }
    }
}
