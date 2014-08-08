using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_building_test_run_information_without_a_test
    {
        private List<TestRunInformation> items;

        [SetUp]
        public void Setup()
        {
            var assembler = new TestRunInfoAssembler();
            var entries = new[]
                              {
                                  new ProfilerEntry {Functionid = 1, Method = "Test", Runtime = "Test", Sequence = 1},
                                  new ProfilerEntry {Functionid = 1, Method = "Test", Runtime = "Test", Sequence = 2}
                              };
            items = assembler.Assemble(entries).ToList();
        }

        [Test]
        public void no_test_run_informations_are_generated()
        {
            Assert.AreEqual(0, items.Count);
        }
    }
}