using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Paths
{
    [TestFixture]
    public class when_finding_single_path_in_test
    {
        private List<IEnumerable<string>> _paths;

        [SetUp]
        public void SetUp()
        {
            var information = TestData.BuildTestInformatonFor("Test1", "test");
            _paths = PathFinder.FindPathsTo(information, "testChild2").ToList();
        }

        [Test]
        public void only_one_path_is_present()
        {
            Assert.AreEqual(1, _paths.Count);
        }

        [Test]
        public void the_path_starts_at_the_root()
        {
            Assert.AreEqual("testRoot", _paths[0].Last());
        }

        [Test]
        public void the_path_has_depth_of_two()
        {
            Assert.AreEqual(2, _paths[0].Count());
        }

        [Test]
        public void the_searched_node_is_first_node()
        {
            Assert.AreEqual("testChild2", _paths[0].First());
        }
    }
}
