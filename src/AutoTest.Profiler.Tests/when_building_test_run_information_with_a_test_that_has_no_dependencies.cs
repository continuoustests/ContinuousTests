using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_building_test_run_information_with_a_test_that_has_no_dependencies
    {
        private List<TestRunInformation> items;

        [SetUp]
        public void Setup()
        {
            var assembler = new TestRunInfoAssembler();
            var entries = new[]
                              {
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 1, Method = "Test", Runtime = "Test", Sequence = 1, IsTest = true},
                                  new ProfilerEntry {Type = ProfileType.Leave, Functionid = 1, Method = "Test", Runtime = "Test", Sequence = 2}
                              };
            items = assembler.Assemble(entries).ToList();
        }
        
        [Test]
        public void a_single_test_information_is_created()
        {
            Assert.AreEqual(1, items.Count);  
        }

        [Test]
        public void the_test_information_has_a_test_chain()
        {
            Assert.IsNotNull(items[0].TestChain);
        }
    }
}