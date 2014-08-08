using NUnit.Framework;

namespace AutoTest.Profiler.Tests.TestCounts
{
    [TestFixture]
    public class when_adding_item_to_counts
    {
        private CouplingCountAndNameProjection projection;

        [SetUp]
        public void Setup()
        {
            var info = new TestRunInformation
            {
                Name = "TESTNAME",
                TestChain = TestData.BuildChainWithPrefix("Test")
            }; 
            projection = new CouplingCountAndNameProjection();
            projection.Index(info);
        }

        [Test]
        public void there_are_five_total_methods()
        {
            Assert.AreEqual(5, projection.TotalMethods);
        }

        [Test]
        public void all_entries_have_a_single_test()
        {
            Assert.AreEqual(1, projection.GetTestCountFor("TestRoot"));
            Assert.AreEqual(1, projection.GetTestCountFor("TestChild1"));
            Assert.AreEqual(1, projection.GetTestCountFor("TestChild2"));
            Assert.AreEqual(1, projection.GetTestCountFor("TestChild3"));
            Assert.AreEqual(1, projection.GetTestCountFor("TestGrandChild"));
        }

        [Test]
        public void all_entries_link_back_to_test_name()
        {
            Assert.AreEqual("TESTNAME", projection.GetTestsFor("TestRoot")[0]);
            Assert.AreEqual("TESTNAME", projection.GetTestsFor("TestChild1")[0]);
            Assert.AreEqual("TESTNAME", projection.GetTestsFor("TestChild2")[0]);
            Assert.AreEqual("TESTNAME", projection.GetTestsFor("TestChild3")[0]);
            Assert.AreEqual("TESTNAME", projection.GetTestsFor("TestGrandChild")[0]);
        }
    }
}
