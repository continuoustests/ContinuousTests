using NUnit.Framework;

namespace AutoTest.Profiler.Tests.TestCounts
{
    [TestFixture]
    public class when_removing_same_item_from_counts
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
            projection.Remove(info);
        }

        [Test]
        public void there_are_five_total_methods_after()
        {
            Assert.AreEqual(5, projection.TotalMethods);
        }

        [Test]
        public void count_information_is_updated_for_test()
        {
            var counts = projection.GetRuntimeCallTimingsFor("TESTNAME");
            Assert.AreEqual(0, counts.TimesCalled);
            Assert.AreEqual(0, counts.TotalTime);
            Assert.AreEqual(0, counts.TotalTimeUnder);
        }


        [Test]
        public void all_entries_have_a_no_tests()
        {
            Assert.AreEqual(0, projection.GetTestCountFor("TestRoot"));
            Assert.AreEqual(0, projection.GetTestCountFor("TestChild1"));
            Assert.AreEqual(0, projection.GetTestCountFor("TestChild2"));
            Assert.AreEqual(0, projection.GetTestCountFor("TestChild3"));
            Assert.AreEqual(0, projection.GetTestCountFor("TestGrandChild"));
        }

    }
}