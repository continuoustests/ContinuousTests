using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.TestCounts
{
    [TestFixture]
    public class when_loading_from_a_corrupt_counts_snapshot
    {
        private CouplingCountAndNameProjection _projection;
        private readonly List<CountsAndTimes> _counts = new List<CountsAndTimes>();

        [Test, ExpectedException(typeof(CorruptedCountsProjectionSnapshotException))]
        public void an_exception_is_thrown()
        {
            string filename = Path.GetTempFileName();
            File.WriteAllText(filename, "this really is not a valid snapshot.");
            _projection = new CouplingCountAndNameProjection();
            _projection.LoadFromSnapshot(filename);
        }
    }
}