using NUnit.Framework;

namespace AutoTest.Profiler.Tests.TestCounts
{
    [TestFixture]
    public class when_adding_item_to_counts_that_is_nested
    {
        private CouplingCountAndNameProjection projection;

        [SetUp]
        public void Setup()
        {
            var info = new TestRunInformation
                           {
                               Name = "TESTNAME",
                               TestChain = TestData.BuildChainWithPrefix("Foo+Test::")
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
            Assert.AreEqual(1, projection.GetTestCountFor("Foo/Test::Root"));
        }

        [Test]
        public void all_entries_link_back_to_test_name()
        {
            Assert.AreEqual("TESTNAME", projection.GetTestsFor("Foo/Test::Root")[0]);
        }
    }
}