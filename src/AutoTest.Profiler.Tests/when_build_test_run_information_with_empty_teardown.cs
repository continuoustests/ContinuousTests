using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{

    [TestFixture]
    public class when_build_test_run_information_with_empty_teardown
    {
        private List<TestRunInformation> items;

        [SetUp]
        public void Setup()
        {
            var assembler = new TestRunInfoAssembler();
            var entries = new[]
                              {
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 3, Method = "Test1", Runtime = "Test", Sequence = 1, IsTest = true},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 4, Method = "Method1", Runtime = "RMethod", Sequence = 2},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 4, Method = "Method1", Runtime = "RMethod", Sequence = 3},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 3, Method = "Test1", Runtime = "Test", Sequence = 4},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 1, Method = "Teardown", Runtime = "Teardown", Sequence = 5, IsTeardown = true},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 1, Method = "Teardown", Runtime = "Teardown", Sequence = 6},
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
            int i = 3;
            Assert.AreEqual(1, items[0].Teardowns.Count);
        }

        [Test]
        public void the_teardown_has_correct_name()
        {
            Assert.AreEqual("Teardown", items[0].Teardowns[0].Name);
        }

        [Test]
        public void the_teardown_has_no_children()
        {
            Assert.AreEqual(0, items[0].Teardowns[0].Children.Count());
        }
    }
}