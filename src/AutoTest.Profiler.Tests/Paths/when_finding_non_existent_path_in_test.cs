using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.Paths
{
    [TestFixture]
    public class when_finding_non_existent_path_in_test
    {
        private List<IEnumerable<string>> _paths;

        [SetUp]
        public void SetUp()
        {
            var information = TestData.BuildTestInformatonFor("Test1", "test");
            _paths = PathFinder.FindPathsTo(information, "testChild27").ToList();
        }

        [Test]
        public void no_paths_are_found()
        {
            Assert.AreEqual(0, _paths.Count);
        }
    }
}