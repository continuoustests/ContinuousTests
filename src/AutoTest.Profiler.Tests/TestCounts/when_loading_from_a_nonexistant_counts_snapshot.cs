using System.Collections.Generic;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.TestCounts
{
    [TestFixture]
    public class when_loading_from_a_nonexistant_counts_snapshot
    {
        private CouplingCountAndNameProjection _projection;
        private readonly List<CountsAndTimes> _counts = new List<CountsAndTimes>();

        [Test, ExpectedException(typeof(CorruptedCountsProjectionSnapshotException))]
        public void an_exception_is_thrown()
        {
            const string filename = "C:\\thisreallyshouldntexist.whatever";
            var info = new TestRunInformation
                           {
                               Name = "TESTNAME",
                               TestChain = TestData.BuildChainWithPrefix("Test")
                           };
            _projection = new CouplingCountAndNameProjection();
            _projection.LoadFromSnapshot(filename);
        }
    }
}