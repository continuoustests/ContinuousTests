using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_parsing_profiler_information_with_an_exception
    {
        private List<ProfilerEntry> items;

        [SetUp]
        public void SetUp()
        {
            var stream = DataBuilder.Create(
                Entry.Enter(1).At(43.5).WithFunctionId(12345).For("Method").RuntimeOf("RuntimeMethod"),
                Entry.Enter(2).At(43.5).WithFunctionId(12346).For("Method1").RuntimeOf("RuntimeMethod"),
                Entry.Enter(3).At(43.5).WithFunctionId(12347).For("Method2").RuntimeOf("RuntimeMethod"),
                Entry.Leave(4).At(43.6).WithFunctionId(12345)
                );

            var parser = new BinaryFileProfilerDataParser();
            items = parser.Parse(stream).ToList();
        }

        //main idea is that this does not underflow (do to missing matchups on stack)

        [Test]
        public void there_are_four_entries()
        {
            Assert.AreEqual(4, items.Count);
        }
    }
}