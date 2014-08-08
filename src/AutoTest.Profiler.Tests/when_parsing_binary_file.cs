using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_handling_simple_enter_and_leave
    {
        private List<ProfilerEntry> items;

        [SetUp]
        public void can_read_single_open_close()
        {
            var stream = DataBuilder.Create(
                Entry.Enter(1).At(43.5).WithFunctionId(12345).For("Method").RuntimeOf("RuntimeMethod").OnThread(1),
                Entry.Leave(2).At(43.6).WithFunctionId(12345).OnThread(1)
            );

            var parser = new BinaryFileProfilerDataParser();
            items = parser.Parse(stream).ToList();
        }

        [Test]
        public void the_thread_is_correctly_set_on_enter()
        {
            Assert.AreEqual(1, items[0].Thread);
        }


        [Test]
        public void the_thread_is_correctly_set_on_leave()
        {
            Assert.AreEqual(1, items[0].Thread);
        }

        [Test]
        public void the_method_name_gets_populated_on_leave()
        {
            Assert.AreEqual("Method", items[1].Method);
        }

        [Test]
        public void the_runtime_name_gets_populated_on_leave()
        {
            Assert.AreEqual("RuntimeMethod", items[1].Runtime);
        }

        [Test]
        public void the_leave_has_correct_functionid()
        {
            Assert.AreEqual(12345, items[0].Functionid);
        }
        [Test]
        public void the_enter_has_correct_functionid()
        {
            Assert.AreEqual(12345, items[0].Functionid);
        }

        [Test]
        public void the_entry_traps_runtime_method()
        {
            Assert.AreEqual("RuntimeMethod", items[0].Runtime);
        }

        [Test]
        public void the_first_sequence_is_read()
        {
            Assert.AreEqual(1, items[0].Sequence);
        }

        [Test]
        public void the_second_sequence_is_read()
        {
            Assert.AreEqual(2, items[1].Sequence);
        }
        [Test]
        public void the_first_entry_is_for_method()
        {
            Assert.AreEqual("Method", items[0].Method);
        }
        [Test]
        public void the_correct_number_of_items_is_read()
        {
            Assert.AreEqual(2, items.Count);
        }

        [Test]
        public void the_first_entry_is_a_enter()
        {
            Assert.IsTrue(items[0].Type == ProfileType.Enter);
        }

        [Test]
        public void the_second_entry_is_a_leave()
        {
            Assert.IsTrue(items[1].Type == ProfileType.Leave);
        }
    }

    [TestFixture]
    public class when_parsing_profiler_info_with_nested_enters
    {
        private List<ProfilerEntry> items;

        [SetUp]
        public void setup()
        {
            var stream = DataBuilder.Create(
                Entry.Enter(1).At(43.5).WithFunctionId(12345).For("Method").RuntimeOf("RuntimeMethod"),
                Entry.Enter(2).At(43.5).WithFunctionId(12346).For("InnerMethod").RuntimeOf("InnerRuntimeMethod"),
                Entry.Leave(3).At(43.6).WithFunctionId(12346),
                Entry.Leave(4).At(43.6).WithFunctionId(12345)
            );

            var parser = new BinaryFileProfilerDataParser();
            items = parser.Parse(stream).ToList();
        }

        [Test]
        public void inner_enter_has_correct_method_name()
        {
            Assert.AreEqual("InnerMethod", items[1].Method);
        }

        [Test]
        public void inner_enter_has_correct_runtime_name()
        {
            Assert.AreEqual("InnerRuntimeMethod", items[1].Runtime);
        }

        [Test]
        public void inner_leave_has_correct_runtime_name()
        {
            Assert.AreEqual("InnerRuntimeMethod", items[2].Runtime);
        }
        [Test]
        public void inner_leave_has_correct_method_name()
        {
            Assert.AreEqual("InnerMethod", items[2].Method);
        }
        [Test]
        public void outer_leave_has_correct_runtime_name()
        {
            Assert.AreEqual("RuntimeMethod", items[3].Runtime);
        }
        [Test]
        public void outer_leave_has_correct_method_name()
        {
            Assert.AreEqual("Method", items[3].Method);
        }

        [Test]
        public void outer_enter_has_correct_name()
        {
            Assert.AreEqual("Method", items[0].Method);
        }

        [Test]
        public void outer_enter_has_correct_runtime()
        {
            Assert.AreEqual("RuntimeMethod", items[0].Runtime);
        }
    }

    [TestFixture]
    public class when_parsing_profiler_info_with_too_many_leaves
    {
        [Test, ExpectedException(typeof(ProfilerStackUnderFlowException))]
        public void a_stack_underflow_exception_is_thrown()
        {
            var stream = DataBuilder.Create(
                            Entry.Enter(1).At(43.5).WithFunctionId(12345).For("Method").RuntimeOf("RuntimeMethod"),
                            Entry.Leave(2).At(43.6).WithFunctionId(12345),
                            Entry.Leave(3).At(43.7).WithFunctionId(12346)
             );

            var parser = new BinaryFileProfilerDataParser();
            parser.Parse(stream).ToList();   
        }
    }

    [TestFixture]
    public class when_parsing_profiler_info_with_too_few_leaves
    {
        [Test, ExpectedException(typeof(ProfilerMissingLeavesException))]
        public void a_stack_underflow_exception_is_thrown()
        {
            var stream = DataBuilder.Create(
                            Entry.Enter(1).At(43.5).WithFunctionId(12345).For("Method").RuntimeOf("RuntimeMethod"),
                            Entry.Enter(2).At(43.5).WithFunctionId(12346).For("Method").RuntimeOf("RuntimeMethod"),
                            Entry.Leave(3).At(43.6).WithFunctionId(12346)
             );

            var parser = new BinaryFileProfilerDataParser();
            parser.Parse(stream).ToList();
        }
    }

    [TestFixture]
    public class when_parsing_profiler_info_with_non_mathing_functionid_on_leave
    {
        [Test, ExpectedException(typeof(NonmatchingFunctionIdException))]
        public void a_stack_underflow_exception_is_thrown()
        {
            var stream = DataBuilder.Create(
                            Entry.Enter(1).At(43.5).WithFunctionId(12345).For("Method").RuntimeOf("RuntimeMethod"),
                            Entry.Leave(3).At(43.6).WithFunctionId(12346)
             );

            var parser = new BinaryFileProfilerDataParser();
            parser.Parse(stream).ToList();
        }
    }

    [TestFixture]
    public class when_parsing_profiler_info_with_multiple_threads
    {
        private List<ProfilerEntry> items;

        [SetUp]
        public void setup()
        {
            var stream = DataBuilder.Create(
                            Entry.Enter(1).At(43.5).WithFunctionId(12345).For("Method1").RuntimeOf("RuntimeMethod1").OnThread(0),
                            Entry.Enter(2).At(43.5).WithFunctionId(12346).For("Method2").RuntimeOf("RuntimeMethod2").OnThread(1),
                            Entry.Leave(3).At(43.6).WithFunctionId(12345).OnThread(0),
                            Entry.Leave(4).At(43.6).WithFunctionId(12346).OnThread(1)
             );

            var parser = new BinaryFileProfilerDataParser();
            items = parser.Parse(stream).ToList();
        }

        [Test]
        public void the_leave_on_first_thread_has_correct_method_name()
        {
            Assert.AreEqual("Method1", items[2].Method);
        }

        [Test]
        public void the_leave_on_first_thread_has_correct_runtime_name()
        {
            Assert.AreEqual("RuntimeMethod1", items[2].Runtime);
        }
        [Test]
        public void the_leave_on_second_thread_has_correct_method_name()
        {
            Assert.AreEqual("Method2", items[3].Method);
        }

        [Test]
        public void the_leave_on_second_thread_has_correct_runtime_name()
        {
            Assert.AreEqual("RuntimeMethod2", items[3].Runtime);
        }
    }

    [TestFixture]
    public class when_parsing_profiler_info_with_too_few_leaves_on_one_thread
    {
        [Test, ExpectedException(typeof(ProfilerMissingLeavesException))]
        public void a_stack_underflow_exception_is_thrown()
        {
            var stream = DataBuilder.Create(
                            Entry.Enter(1).At(43.5).WithFunctionId(12345).For("Method").RuntimeOf("RuntimeMethod").OnThread(1),
                            Entry.Enter(2).At(43.5).WithFunctionId(12346).For("Method").RuntimeOf("RuntimeMethod").OnThread(2),
                            Entry.Leave(3).At(43.6).WithFunctionId(12346).OnThread(2)
             );

            var parser = new BinaryFileProfilerDataParser();
            parser.Parse(stream).ToList();
        }
    }
}
