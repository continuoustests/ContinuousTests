using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_build_test_run_information_with_teardown_with_no_exit_then_nothing
    {
        private List<TestRunInformation> items;

        [SetUp]
        public void Setup()
        {
            var assembler = new TestRunInfoAssembler();
            var entries = new[]
                              {
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 3, Method = "Test1", Runtime = "Test", Sequence = 3, IsTest = true},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 6, Method = "Method3", Runtime = "Method3", Sequence = 5},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 6, Method = "Method3", Runtime = "Method3", Sequence = 6},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 3, Method = "Test1", Runtime = "Test", Sequence = 3, IsTest = true},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 4, Method = "Teardown", Runtime = "Teardown", Sequence = 3, IsTeardown = true},
                              };
            items = assembler.Assemble(entries).ToList();
        }

        [Test]
        public void one_test_information_is_created()
        {
            Assert.AreEqual(1, items.Count);
        }

        [Test]
        public void no_teardowns_are_added_to_the_test_information()
        {
            Assert.AreEqual(0, items[0].Teardowns.Count);
        }

        [Test]
        public void the_test_has_one_child()
        {
            Assert.AreEqual(1, items[0].TestChain.Children.Count);
        }

        [Test]
        public void there_are_two_items_in_the_test_chain()
        {
            Assert.AreEqual(2, items[0].TestChain.IterateAll().Count());
        }
    }
}