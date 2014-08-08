using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
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
}