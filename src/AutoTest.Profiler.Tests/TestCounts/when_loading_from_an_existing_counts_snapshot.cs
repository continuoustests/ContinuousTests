using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests.TestCounts
{
    [TestFixture]
    public class when_loading_from_an_existing_counts_snapshot
    {
        private CouplingCountAndNameProjection _projection;
        private readonly List<CountsAndTimes> _counts = new List<CountsAndTimes>();
        private long position;

        [SetUp]
        public void Setup()
        {
            var filename = Path.GetTempFileName();
            var info = new TestRunInformation
                           {
                               Name = "TESTNAME",
                               TestChain = TestData.BuildChainWithPrefix("Test")
                           };
            _projection = new CouplingCountAndNameProjection();
            _projection.Index(info);
            _projection.SnapShotTo(filename, 2048);
            _counts.Add(_projection.GetRuntimeCallTimingsFor("TestRoot"));
            _counts.Add(_projection.GetRuntimeCallTimingsFor("TestChild1"));
            _counts.Add(_projection.GetRuntimeCallTimingsFor("TestChild2"));
            _counts.Add(_projection.GetRuntimeCallTimingsFor("TestChild3"));
            _counts.Add(_projection.GetRuntimeCallTimingsFor("TestGrandChild"));
            _projection = new CouplingCountAndNameProjection();
            position = _projection.LoadFromSnapshot(filename);
        }

        [Test]
        public void the_correct_position_is_maintained()
        {
            Assert.AreEqual(2048, (int) position);
        }

        [Test]
        public void there_are_five_total_methods()
        {
            Assert.AreEqual(5, _projection.TotalMethods);
        }

        [Test]
        public void all_entries_have_a_single_test()
        {
            Assert.AreEqual(1, _projection.GetTestCountFor("TestRoot"));
            Assert.AreEqual(1, _projection.GetTestCountFor("TestChild1"));
            Assert.AreEqual(1, _projection.GetTestCountFor("TestChild2"));
            Assert.AreEqual(1, _projection.GetTestCountFor("TestChild3"));
            Assert.AreEqual(1, _projection.GetTestCountFor("TestGrandChild"));
        }

        [Test]
        public void all_entries_link_back_to_test_name()
        {
            Assert.AreEqual("TESTNAME", _projection.GetTestsFor("TestRoot")[0]);
            Assert.AreEqual("TESTNAME", _projection.GetTestsFor("TestChild1")[0]);
            Assert.AreEqual("TESTNAME", _projection.GetTestsFor("TestChild2")[0]);
            Assert.AreEqual("TESTNAME", _projection.GetTestsFor("TestChild3")[0]);
            Assert.AreEqual("TESTNAME", _projection.GetTestsFor("TestGrandChild")[0]);
        }

        [Test]
        public void the_counts_are_same_for_root()
        {
            var item = _projection.GetRuntimeCallTimingsFor("TestRoot");
            Assert.AreEqual(_counts[0].AverageTime, item.AverageTime);
            Assert.AreEqual(_counts[0].AverageTimeUnder, item.AverageTimeUnder);
            Assert.AreEqual(_counts[0].MaxTime, item.MaxTime);
            Assert.AreEqual(_counts[0].MaxTimeUnder, item.MaxTimeUnder);
            Assert.AreEqual(_counts[0].MinTime, item.MinTime);
            Assert.AreEqual(_counts[0].MinTimeUnder, item.MinTimeUnder);
            Assert.AreEqual(_counts[0].TimesCalled, item.TimesCalled);
        }

        [Test]
        public void the_counts_are_same_for_child1()
        {
            var item = _projection.GetRuntimeCallTimingsFor("TestChild1");
            Assert.AreEqual(_counts[1].AverageTime, item.AverageTime);
            Assert.AreEqual(_counts[1].AverageTimeUnder, item.AverageTimeUnder);
            Assert.AreEqual(_counts[1].MaxTime, item.MaxTime);
            Assert.AreEqual(_counts[1].MaxTimeUnder, item.MaxTimeUnder);
            Assert.AreEqual(_counts[1].MinTime, item.MinTime);
            Assert.AreEqual(_counts[1].MinTimeUnder, item.MinTimeUnder);
            Assert.AreEqual(_counts[1].TimesCalled, item.TimesCalled);
        }

        [Test]
        public void the_counts_are_same_for_child2()
        {
            var item = _projection.GetRuntimeCallTimingsFor("TestChild2");
            Assert.AreEqual(_counts[2].AverageTime, item.AverageTime);
            Assert.AreEqual(_counts[2].AverageTimeUnder, item.AverageTimeUnder);
            Assert.AreEqual(_counts[2].MaxTime, item.MaxTime);
            Assert.AreEqual(_counts[2].MaxTimeUnder, item.MaxTimeUnder);
            Assert.AreEqual(_counts[2].MinTime, item.MinTime);
            Assert.AreEqual(_counts[2].MinTimeUnder, item.MinTimeUnder);
            Assert.AreEqual(_counts[2].TimesCalled, item.TimesCalled);
        }


        [Test]
        public void the_counts_are_same_for_child3()
        {
            var item = _projection.GetRuntimeCallTimingsFor("TestChild3");
            Assert.AreEqual(_counts[3].AverageTime, item.AverageTime);
            Assert.AreEqual(_counts[3].AverageTimeUnder, item.AverageTimeUnder);
            Assert.AreEqual(_counts[3].MaxTime, item.MaxTime);
            Assert.AreEqual(_counts[3].MaxTimeUnder, item.MaxTimeUnder);
            Assert.AreEqual(_counts[3].MinTime, item.MinTime);
            Assert.AreEqual(_counts[3].MinTimeUnder, item.MinTimeUnder);
            Assert.AreEqual(_counts[3].TimesCalled, item.TimesCalled);
        }

        [Test]
        public void the_counts_are_same_for_grandchild()
        {
            var item = _projection.GetRuntimeCallTimingsFor("TestGrandChild");
            Assert.AreEqual(_counts[4].AverageTime, item.AverageTime);
            Assert.AreEqual(_counts[4].AverageTimeUnder, item.AverageTimeUnder);
            Assert.AreEqual(_counts[4].MaxTime, item.MaxTime);
            Assert.AreEqual(_counts[4].MaxTimeUnder, item.MaxTimeUnder);
            Assert.AreEqual(_counts[4].MinTime, item.MinTime);
            Assert.AreEqual(_counts[4].MinTimeUnder, item.MinTimeUnder);
            Assert.AreEqual(_counts[4].TimesCalled, item.TimesCalled);
        }
    }
}