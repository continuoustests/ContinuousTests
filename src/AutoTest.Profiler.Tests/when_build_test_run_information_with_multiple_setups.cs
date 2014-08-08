using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_build_test_run_information_with_multiple_setups
    {
        private List<TestRunInformation> items;

        [SetUp]
        public void Setup()
        {
            var assembler = new TestRunInfoAssembler();
            var entries = new[]
                              {
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 1, Method = "Setup1", Runtime = "Setup1", Sequence = 1, IsSetup = true},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 1, Method = "Setup1", Runtime = "Setup1", Sequence = 2},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 1, Method = "Setup2", Runtime = "Setup2", Sequence = 3, IsSetup = true},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 1, Method = "Setup2", Runtime = "Setup2", Sequence = 4},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 3, Method = "Test1", Runtime = "Test", Sequence = 5, IsTest = true},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 4, Method = "Method1", Runtime = "RMethod", Sequence = 6},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 4, Method = "Method1", Runtime = "RMethod", Sequence = 7},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 3, Method = "Test1", Runtime = "Test", Sequence = 8}
                              };
            items = assembler.Assemble(entries).ToList();
        }

        [Test]
        public void one_test_information_is_created()
        {
            Assert.AreEqual(1, items.Count);
        }

        [Test]
        public void two__setups_are_added_to_the_test_information()
        {
            Assert.AreEqual(2, items[0].Setups.Count);
        }

        [Test]
        public void the_first_setup_has_correct_name()
        {
            Assert.AreEqual("Setup2", items[0].Setups[0].Name);
        }

        [Test]
        public void the_second_setup_has_correct_name()
        {
            Assert.AreEqual("Setup1", items[0].Setups[1].Name);
        }

        [Test]
        public void the_first_setup_has_no_children()
        {
            Assert.AreEqual(0, items[0].Setups[0].Children.Count());
        }

        [Test]
        public void the_second_setup_has_no_childreb()
        {
            Assert.AreEqual(0, items[0].Setups[0].Children.Count());
        }

    }
}