using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Profiler;
namespace AutoTest.Profiler.Tests.TestCounts
{
    [TestFixture]
    public class CountsAndTimes_Tests
    {
        [Test]
        public void can_calulate_avereage_time_under_properly_for_single_item()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) {StartTime=0, EndTime=10});
            Assert.AreEqual(10, count.AverageTimeUnder);
        }

        [Test]
        public void updates_avg_under_when_removing_a_sample()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            count.RemoveEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            Assert.AreEqual(double.NaN, count.AverageTimeUnder);
        }

        [Test]
        public void updates_avg_when_removing_a_sample()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            count.RemoveEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            Assert.AreEqual(double.NaN, count.AverageTime);
        }

        [Test]
        public void updates_total_under_when_removing_a_sample()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            count.RemoveEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            Assert.AreEqual(0, count.TotalTimeUnder);
        }

        [Test]
        public void updates_total_when_removing_a_sample()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            count.RemoveEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            Assert.AreEqual(0, count.TotalTime);
        }

        [Test]
        public void updates_count_when_removing_a_sample()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            count.RemoveEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            Assert.AreEqual(0, count.TimesCalled);
        }

        [Test]
        public void can_calulate_max_time_under_properly_for_single_item()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            Assert.AreEqual(10, count.MaxTimeUnder);
        }

        [Test]
        public void can_calulate_min_time_under_properly_for_single_item()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            Assert.AreEqual(10, count.MinTimeUnder);
        }

        [Test]
        public void does_not_change_min_under_if_not_lower()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 11 });
            Assert.AreEqual(10, count.MinTimeUnder);
        }

        [Test]
        public void does_not_change_max_under_if_not_higher()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 6 });
            Assert.AreEqual(10, count.MaxTimeUnder);
        }

        [Test]
        public void does_change_max_under_if_higher()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 16 });
            Assert.AreEqual(16, count.MaxTimeUnder);
        }

        [Test]
        public void does_change_min_under_lower_if_lower()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 6 });
            Assert.AreEqual(6, count.MinTimeUnder);
        }

        [Test]
        public void can_calulate_total_time_under_properly_for_single_item()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            Assert.AreEqual(10, count.TotalTimeUnder);
        }

        [Test]
        public void can_calulate_total_time_under_properly_for_multiple_items()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            Assert.AreEqual(30, count.TotalTimeUnder);
        }
        
        [Test]
        public void can_calulate_total_time_properly_for_single_item_with_no_children()
        {
            var count = new CountsAndTimes();
            count.ProcessNewEntry(new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 });
            Assert.AreEqual(10, count.TotalTime);
            Assert.AreEqual(10, count.MaxTime);
            Assert.AreEqual(10, count.MinTime);
            Assert.AreEqual(10, count.AverageTime);
        }

        [Test]
        public void can_calulate_total_time_properly_for_single_item_with_single_child()
        {
            var count = new CountsAndTimes();
            var chain = new CallChain("TEST", "TESTR", 1) { StartTime = 0, EndTime = 10 };
            chain.Children.Add(new CallChain("TEST1", "TESTR", 3) { StartTime = 3, EndTime = 5 });
            count.ProcessNewEntry(chain);
            Assert.AreEqual(8, count.TotalTime);
            Assert.AreEqual(8, count.MaxTime);
            Assert.AreEqual(8, count.MinTime);
            Assert.AreEqual(8, count.AverageTime);
        }

        [Test]
        public void can_calulate_total_time_properly_for_single_item_with_multiple_children()
        {
            var count = new CountsAndTimes();
            var chain = new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 };
            chain.Children.Add(new CallChain("TEST1", "TESTR", 3) { StartTime = 3, EndTime = 5 });
            chain.Children.Add(new CallChain("TEST1", "TESTR", 3) { StartTime = 5, EndTime = 7 });
            count.ProcessNewEntry(chain);
            Assert.AreEqual(6, count.TotalTime);
            Assert.AreEqual(6, count.MaxTime);
            Assert.AreEqual(6, count.MinTime);
            Assert.AreEqual(6, count.AverageTime);
        }

        [Test]
        public void does_not_include_children_that_are_itself_when_counting_with_multiple_children()
        {
            var count = new CountsAndTimes();
            var chain = new CallChain("TEST", "TESTR", 3) { StartTime = 0, EndTime = 10 };
            chain.Children.Add(new CallChain("TEST", "TESTR", 3) { StartTime = 3, EndTime = 5 });
            chain.Children.Add(new CallChain("TEST1", "TESTR", 3) { StartTime = 5, EndTime = 7 });
            count.ProcessNewEntry(chain);
            Assert.AreEqual(8, count.TotalTime);
            Assert.AreEqual(8, count.MaxTime);
            Assert.AreEqual(8, count.MinTime);
            Assert.AreEqual(8, count.AverageTime);
        }

        [Test]
        public void null_test_information_to_remove_does_not_fail()
        {
            var counts = new CouplingCountAndNameProjection();
            Assert.DoesNotThrow(() => counts.Remove(null));
        }

        [Test]
        public void test_information_with_null_name_to_remove_does_not_fail()
        {
            var counts = new CouplingCountAndNameProjection();
            Assert.DoesNotThrow(() => counts.Remove(new TestRunInformation() {Name = null}));
        }
    }
}
