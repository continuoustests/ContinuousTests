using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    //[TestFixture]
    //public class when_build_test_run_information_with_setup_with_no_exit_then_a_test
    //{
    //    private List<TestRunInformation> items;

    //    [SetUp]
    //    public void Setup()
    //    {
    //        int x = 5;
    //        var assembler = new TestRunInfoAssembler();
    //        var entries = new[]
    //                          {
    //                              new ProfilerEntry {Type = ProfileType.Enter, Functionid = 1, Method = "Test1", Runtime = "Test1", Sequence = 1, IsTest = true},
    //                              new ProfilerEntry {Type = ProfileType.Enter, Functionid = 1, Method = "Test2", Runtime = "Test2", Sequence = 1, IsTest = true},
    //                              new ProfilerEntry {Type = ProfileType.Enter, Functionid = 11, Method = "TD1", Runtime = "TD1", Sequence = 1, IsTeardown = true},
    //                              new ProfilerEntry {Type = ProfileType.Enter, Functionid = 2, Method = "Method2", Runtime = "Method2", Sequence = 2},
    //                              new ProfilerEntry {Type = ProfileType.Enter, Functionid = 6, Method = "Method3", Runtime = "Method3", Sequence = 3},
    //                              new ProfilerEntry {Type = ProfileType.Leave, Functionid = 6, Method = "Method3", Runtime = "Method3", Sequence = 4},
    //                              new ProfilerEntry {Type = ProfileType.Enter, Functionid = 3, Method = "TD2", Runtime = "TD2", Sequence = 7, IsTeardown = true},
    //                              new ProfilerEntry {Type = ProfileType.Enter, Functionid = 4, Method = "Method1", Runtime = "RMethod", Sequence = 8},
    //                              new ProfilerEntry {Type = ProfileType.Leave, Functionid = 4, Method = "Method1", Runtime = "RMethod", Sequence = 9},
    //                              new ProfilerEntry {Type = ProfileType.Leave, Functionid = 3, Method = "TD2", Runtime = "TD2", Sequence = 10, IsTeardown = true},
    //                          };
    //        //items = assembler.Assemble(entries).ToList();
    //    }

    //    [Test]
    //    public void one_test_information_is_created()
    //    {
    //        Assert.AreEqual(1, items.Count);
    //    }

    //    [Test]
    //    public void two_teardowns_are_added_to_the_test_information()
    //    {

    //        Assert.AreEqual(2, items[0].Teardowns.Count);
    //    }

    //    [Test]
    //    public void the_first_teardown_has_correct_name()
    //    {
    //        Assert.AreEqual("TD1", items[0].Setups[1].Name);
    //    }

    //    [Test]
    //    public void the_second_setup_has_correct_name()
    //    {
    //        Assert.AreEqual("TD2", items[0].Setups[0].Name);
    //    }
    //    [Test]
    //    public void the_first_teardown_has_one_chain()
    //    {
    //        Assert.AreEqual(1, items[0].Setups[1].Children.Count());
    //    }

    //    [Test]
    //    public void the_first_teardown_chain_has_two_entries()
    //    {
    //        Assert.AreEqual(2, items[0].Setups[1].Children[0].IterateAll().Count());
    //    }
    //    [Test]
    //    public void the_second_teardown_has_one_chain()
    //    {
    //        Assert.AreEqual(1, items[0].Setups[0].Children.Count());
    //    }

    //    [Test]
    //    public void the_second_teardown_chain_has_one_entry()
    //    {
    //        Assert.AreEqual(1, items[0].Setups[0].Children[0].IterateAll().Count());
    //    }
    //}


    [TestFixture]
    public class when_build_test_run_information_with_setup_with_no_exit_then_another_setup
    {
        private List<TestRunInformation> items;
        
        [SetUp]
        public void Setup()
        {
            var assembler = new TestRunInfoAssembler();
            var entries = new[]
                              {
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 1, Method = "Setup", Runtime = "Setup", Sequence = 1, IsSetup = true},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 2, Method = "Method2", Runtime = "Method2", Sequence = 2},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 6, Method = "Method3", Runtime = "Method3", Sequence = 3},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 6, Method = "Method3", Runtime = "Method3", Sequence = 4},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 3, Method = "Setup1", Runtime = "Setup1", Sequence = 7, IsSetup = true},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 4, Method = "Method1", Runtime = "RMethod", Sequence = 8},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 4, Method = "Method1", Runtime = "RMethod", Sequence = 9},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 3, Method = "Setup1", Runtime = "Setup1", Sequence = 10},
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 4, Method = "Test1", Runtime = "RMethod", Sequence = 11, IsTest = true},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 4, Method = "Test1", Runtime = "RMethod", Sequence = 12, IsTest = true},
                              };
            items = assembler.Assemble(entries).ToList();
        }

        [Test]
        public void one_test_information_is_created()
        {
            Assert.AreEqual(1, items.Count);
        }

        [Test]
        public void two_setups_are_added_to_the_test_information()
        {

            Assert.AreEqual(2, items[0].Setups.Count);
        }

        [Test]
        public void the_first_setup_has_correct_name()
        {
            Assert.AreEqual("Setup", items[0].Setups[1].Name);
        }

        [Test]
        public void the_second_setup_has_correct_name()
        {
            Assert.AreEqual("Setup1", items[0].Setups[0].Name);
        }
        [Test]
        public void the_first_setup_has_one_chain()
        {
            Assert.AreEqual(1, items[0].Setups[1].Children.Count());
        }

        [Test]
        public void the_first_setups_chain_has_two_entries()
        {
            Assert.AreEqual(2, items[0].Setups[1].Children[0].IterateAll().Count());
        }
        [Test]
        public void the_second_setup_has_one_chain()
        {
            Assert.AreEqual(1, items[0].Setups[0].Children.Count());
        }

        [Test]
        public void the_second_setups_chain_has_one_entry()
        {
            Assert.AreEqual(1, items[0].Setups[0].Children[0].IterateAll().Count());
        }
    }
}