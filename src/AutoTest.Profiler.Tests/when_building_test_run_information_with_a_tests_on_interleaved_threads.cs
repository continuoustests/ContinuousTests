using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_building_test_run_information_with_a_tests_on_interleaved_threads
    {
        private List<TestRunInformation> items;

        [SetUp]
        public void Setup()
        {
            var assembler = new TestRunInfoAssembler();
            var entries = new[]
                              {
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 1, Thread=0 ,Method = "Test1", Runtime = "RTest1", Sequence = 1, IsTest = true},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 2, Thread=1 ,Method = "Test2", Runtime = "RTest2", Sequence = 1, IsTest = true},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 3, Thread=0, Method = "Method1", Runtime = "RMethod1", Sequence = 2},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 4, Thread=1, Method = "Method2", Runtime = "RMethod2", Sequence = 4},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 3, Thread=0, Method = "Method1", Runtime = "RMethod1", Sequence = 3},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 4, Thread=1, Method = "Method2", Runtime = "RMethod2", Sequence = 5},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 2, Thread=1, Method = "Test2", Runtime = "Test", Sequence = 6},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 1, Thread=0, Method = "Test1", Runtime = "Test", Sequence = 7}
                              };
            items = assembler.Assemble(entries).ToList();
        }

        [Test]
        public void two_dadsasdsfadsfdasdftestddda_informations_are_created()
        {
            Assert.AreEqual(2, items.Count);
        }

        [Test]
        public void foadjdatwo_test_informations_are_createddffdsdfsfs()
        {
            Assert.AreEqual(2, items.Count);
        }


        [Test]
        public void two_test_informations_are_created()
        {
            Assert.AreEqual(2, items.Count);
        }

        [Test]
        public void the_test_informations_are_ordered_by_completion()
        {
            Assert.AreEqual("RTest2", items[0].TestChain.Name);
            Assert.AreEqual("RTest1", items[1].TestChain.Name);
        }

        [Test]
        public void the_first_test_chain_has_correct_dependency()
        {
            Assert.AreEqual("RMethod2", items[0].TestChain.Children.ToList()[0].Name);
        }

        [Test]
        public void the_second_test_chain_has_correct_dependency()
        {
            Assert.AreEqual("RMethod1", items[1].TestChain.Children.ToList()[0].Name);
        }
    }
}