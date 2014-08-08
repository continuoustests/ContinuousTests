using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_build_test_run_information_with_test_with_no_exit_then_new_test
    {
        private List<TestRunInformation> items;

        [SetUp]
        public void Setup()
        {
            var assembler = new TestRunInfoAssembler();
            var entries = new[]
                              {
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 3, Method = "Test1", Runtime = "Test", Sequence = 1, IsTest = true},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 2, Method = "Method2", Runtime = "Method2", Sequence = 2},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 6, Method = "Method3", Runtime = "Method3", Sequence = 3},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 6, Method = "Method3", Runtime = "Method3", Sequence = 4},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 7, Method = "Test2", Runtime = "Test2", Sequence = 5, IsTest = true},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 7, Method = "Test2", Runtime = "Test2", Sequence = 5, IsTest = true},
                              };
            items = assembler.Assemble(entries).ToList();
        }

        [Test]
        public void two_test_informations_are_created()
        {
            Assert.AreEqual(2, items.Count);
        }

        [Test]
        public void the_first_test_has_one_child()
        {
            Assert.AreEqual(1, items[0].TestChain.Children.Count);
        }

        [Test]
        public void there_are_three_items_in_the_first_test_chain()
        {
            Assert.AreEqual(3, items[0].TestChain.IterateAll().Count());
        }
    }
}