using NUnit.Framework;

namespace AutoTest.Profiler.Tests.TestCounts
{
        [TestFixture]
        public class when_adding_single_items_counts_with_times
        {
            private CouplingCountAndNameProjection projection;

            [SetUp]
            public void Setup()
            {
                var info = new TestRunInformation();
                info.Name = "TESTNAME";
                var chain = new CallChain("TestRoot", "TestRootR", 1);
                chain.StartTime = 0;
                chain.EndTime = 10;
                var c = new CallChain("TestChild1", "TestChild1R", 2);
                c.StartTime = 1;
                c.EndTime = 2;
                chain.AddChild(c);
                var c2 = new CallChain("TestChild2", "TestChild2R", 3);
                c2.StartTime = 3;
                c2.EndTime = 9;
                chain.AddChild(c2);
                var child = new CallChain("TestChild3", "TestChild3R", 4);
                child.StartTime = 4;
                child.EndTime = 8;
                var grandchild = new CallChain("TestGrandChild", "TestGrandChildR", 5);
                grandchild.StartTime = 5;
                grandchild.EndTime = 7;
                child.AddChild(grandchild);
                chain.AddChild(child);
                info.TestChain = chain;
                projection = new CouplingCountAndNameProjection();
                projection.Index(info);
            }

            [Test]
            public void there_are_five_total_methods()
            {
                Assert.AreEqual(5, projection.TotalMethods);
            }

            [Test]
            public void the_test_method_is_called_once()
            {
                Assert.AreEqual(1, projection.GetRuntimeCallTimingsFor("TestRoot").TimesCalled);
            }

            [Test]
            public void the_test_method_has_correct_time_under()
            {
                Assert.AreEqual(10, projection.GetRuntimeCallTimingsFor("TestRoot").AverageTimeUnder);
            }

            [Test]
            public void the_test_method_has_correct_max_time_under()
            {
                Assert.AreEqual(10, projection.GetRuntimeCallTimingsFor("TestRoot").MaxTimeUnder);
            }

            [Test]
            public void the_test_method_has_correct_min_time_under()
            {
                Assert.AreEqual(10, projection.GetRuntimeCallTimingsFor("TestRoot").MinTimeUnder);
            }

            [Test]
            public void the_first_child_method_is_called_once()
            {
                Assert.AreEqual(1, projection.GetRuntimeCallTimingsFor("TestChild1").TimesCalled);
            }

            [Test]
            public void the_first_child_method_has_correct_time_under()
            {
                Assert.AreEqual(1, projection.GetRuntimeCallTimingsFor("TestChild1").AverageTimeUnder);
            }

            [Test]
            public void the_first_child_method_has_correct_time_in()
            {
                Assert.AreEqual(1, projection.GetRuntimeCallTimingsFor("TestChild1").AverageTime);
            }

            [Test]
            public void the_second_child_method_is_called_once()
            {
                Assert.AreEqual(1, projection.GetRuntimeCallTimingsFor("TestChild2").TimesCalled);
            }

            [Test]
            public void the_second_child_method_has_correct_time_under()
            {
                Assert.AreEqual(6, projection.GetRuntimeCallTimingsFor("TestChild2").AverageTimeUnder);
            }

            [Test]
            public void the_second_child_method_has_correct_time_in()
            {
                Assert.AreEqual(6, projection.GetRuntimeCallTimingsFor("TestChild2").AverageTime);
            }

            [Test]
            public void the_third_child_method_is_called_once()
            {
                Assert.AreEqual(1, projection.GetRuntimeCallTimingsFor("TestChild3").TimesCalled);
            }

            [Test]
            public void the_grand_child_method_is_called_once()
            {
                Assert.AreEqual(1, projection.GetRuntimeCallTimingsFor("TestGrandChild").TimesCalled);
            }
        }
}
