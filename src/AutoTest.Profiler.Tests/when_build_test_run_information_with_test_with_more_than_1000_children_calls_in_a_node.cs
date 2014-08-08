using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_build_test_run_information_with_test_with_more_than_1000_children_calls_in_a_node
    {
        private List<TestRunInformation> items;

        [SetUp]
        public void Setup()
        {
            var assembler = new TestRunInfoAssembler();
            var entries = new List<ProfilerEntry>
                              {
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 3, Method = "Test1", Runtime = "Test", Sequence = 1, IsTest = true},
                              };
            for (int i = 0; i < 5000;i++)
            {
                entries.AddRange(new []
                                     {
                                         new ProfilerEntry {Type = ProfileType.Enter, Functionid = 4, Method = "Method" + i, Runtime = "MethodR" + i, Sequence = 1, IsTest = false},
                                         new ProfilerEntry {Type = ProfileType.Leave, Functionid = 4, Method = "Method" + i, Runtime = "MethodR" + i, Sequence = 1, IsTest = false},
                                     });
            }
            entries.AddRange(new[] {
                                       new ProfilerEntry {Type = ProfileType.Leave, Functionid = 3, Method = "Test2", Runtime = "Test2", Sequence = 5, IsTest = true},
                                   });
            items = assembler.Assemble(entries).ToList();
        }

        [Test]
        public void one_test_information_is_created()
        {
            Assert.AreEqual(1, items.Count);
        }

        [Test]
        public void the_first_test_has_one_thousand_children()
        {
            Assert.AreEqual(1000, items[0].TestChain.Children.Count);
        }
    }
}