using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_build_test_run_irnformation_with_empty_setup
    {
        private List<TestRunInformation> items;

        [SetUp]
        public void Setup()
        {
            var assembler = new TestRunInfoAssembler();
            var entries = new[]
                              {
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 1, Method = "Setup", Runtime = "Setup", Sequence = 1, IsSetup = true},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 1, Method = "Setup", Runtime = "Setup", Sequence = 2},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 3, Method = "Test1", Runtime = "Test", Sequence = 3, IsTest = true},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 4, Method = "Method1", Runtime = "RMethod", Sequence = 4},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 4, Method = "Method1", Runtime = "RMethod", Sequence = 5},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 3, Method = "Test1", Runtime = "Test", Sequence = 6}
                              };
            items = assembler.Assemble(entries).ToList();
        }

        [Test]
        public void one_test_information_is_created()
        {
            Assert.AreEqual(1, items.Count);
        }

        [Test]
        public void a_single_setup_is_added_to_the_test_information()
        {
            Assert.AreEqual(1, items[0].Setups.Count);
        }

        [Test]
        public void the_setup_has_correct_name()
        {
            Assert.AreEqual("Setup", items[0].Setups[0].Name);
        }

        [Test]
        public void the_setup_has_no_children()
        {
            Assert.AreEqual(0, items[0].Setups[0].Children.Count());
        }
    }
}