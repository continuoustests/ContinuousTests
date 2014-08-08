using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
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
}