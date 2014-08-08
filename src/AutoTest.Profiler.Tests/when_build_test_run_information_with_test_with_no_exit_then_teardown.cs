using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_build_test_run_information_with_test_with_no_exit_then_teardown
    {
        private List<TestRunInformation> items;

        [SetUp]
        public void Setup()
        {
            var assembler = new TestRunInfoAssembler();
            var entries = new[]
                              {
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 3, Method = "Test1", Runtime = "Test", Sequence = 3, IsTest = true},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 2, Method = "Method2", Runtime = "Method2", Sequence = 4},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 6, Method = "Method3", Runtime = "Method3", Sequence = 5},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 6, Method = "Method3", Runtime = "Method3", Sequence = 6},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 6, Method = "Teardown", Runtime = "Teardown", Sequence = 6, IsTeardown = true},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 6, Method = "Teardown", Runtime = "Teardown", Sequence = 7, IsTeardown = true},
                              };
            items = assembler.Assemble(entries).ToList();
        }

        [Test]
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
        public void the_teardown_has_correct_name()
        {
            Assert.AreEqual("Teardown", items[0].Teardowns[0].Name);
        }

        [Test]
        public void the_test_has_one_child()
        {
            Assert.AreEqual(1, items[0].TestChain.Children.Count);
        }

        [Test]
        public void there_are_three_items_in_the_chain()
        {
            Assert.AreEqual(3, items[0].TestChain.IterateAll().Count());
        }
    }
}