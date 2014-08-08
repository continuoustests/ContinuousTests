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
}