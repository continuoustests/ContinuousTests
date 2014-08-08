using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Profiler;
namespace AutoTest.Profiler.Tests
{

    [TestFixture]
    public class when_iterating_a_test_chain_with_time_information
    {
        private List<TestRunInformation> items;

        [SetUp]
        public void Setup()
        {
            var assembler = new TestRunInfoAssembler();
            var entries = new[]
                              {
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 3, Method = "Test1", Runtime = "Test", Sequence = 1, IsTest = true, Time=.0001},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 4, Method = "Method1", Runtime = "RMethod", Sequence = 2, Time=.0002},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 4, Method = "Method1", Runtime = "RMethod", Sequence = 3, Time=.0003},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 3, Method = "Test1", Runtime = "Test", Sequence = 4, Time=.0004},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 1, Method = "Teardown", Runtime = "Teardown", Sequence = 5, IsTeardown = true, Time=.0005},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 1, Method = "Teardown", Runtime = "Teardown", Sequence = 6, Time=.0006},
                              };
            items = assembler.Assemble(entries).ToList();
        }

        public void one_test_information_is_created()
        {
            Assert.AreEqual(1, items.Count);
        }

        [Test]
        public void a_single_teardown_is_added_to_the_test_information()
        {
            Assert.AreEqual(1, items[0].Teardowns.Count);
        }

        [Test]
        public void the_start_time_on_tear_down_is_correct()
        {
            Assert.AreEqual(.0005, items[0].Teardowns[0].StartTime);
        }
        [Test]
        public void the_end_time_on_tear_down_is_correct()
        {
            Assert.AreEqual(.0005, items[0].Teardowns[0].StartTime);
        }

        [Test]
        public void there_is_a_single_test()
        {
            Assert.IsNotNull(items[0].TestChain);
        }

        [Test]
        public void the_test_chain_has_correct_start_time()
        {
            Assert.AreEqual(.0001, items[0].TestChain.StartTime);
        }

        [Test]
        public void the_test_chain_has_correct_end_time()
        {
            Assert.AreEqual(.0004, items[0].TestChain.EndTime);
        }

        [Test]
        public void the_test_chain_has_a_single_child()
        {
            Assert.AreEqual(1, items[0].TestChain.Children.Count);
        }

        [Test]
        public void the_test_chain_child_has_correct_start_time()
        {
            Assert.AreEqual(.0002, items[0].TestChain.Children[0].StartTime);
        }

        [Test]
        public void the_test_chain_child_has_correct_end_time()
        {
            Assert.AreEqual(.0003, items[0].TestChain.Children[0].EndTime);
        }
    }
}
