using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_build_test_run_information_with_test_with_depth_more_than_200
    {
        private List<TestRunInformation> items;

        [SetUp] 
        public void Setup()
        {
            var assembler = new TestRunInfoAssembler();
            var entries = new List<ProfilerEntry>
                              {
                                  new ProfilerEntry {Type = ProfileType.Enter, Functionid = 0, Method = "Test1", Runtime = "Test", Sequence = 1, IsTest = true},
                              };
            for (int i = 0; i < 5000; i++)
            {
                entries.AddRange(new[]
                                     {
                                         new ProfilerEntry {Type = ProfileType.Enter, Functionid = i + 1, Method = "Method" + i, Runtime = "MethodR" + i, Sequence = 1, IsTest = false},
                                     });
            }
            for (int i = 4999; i >=0; i--)
            {
                entries.AddRange(new[]
                                     {
                                         new ProfilerEntry {Type = ProfileType.Leave, Functionid = i + 1, Method = "Method" + i, Runtime = "MethodR" + i, Sequence = 1, IsTest = false},
                                     });
            }
            entries.AddRange(new[] {
                                       new ProfilerEntry {Type = ProfileType.Leave, Functionid = 0, Method = "Test1", Runtime = "Test", Sequence = 5, IsTest = true},
                                   });
            items = assembler.Assemble(entries).ToList();
        }

        [Test]
        public void one_test_information_is_created()
        {
            Assert.AreEqual(1, items.Count);
        }

        [Test]
        public void the_test_goes_to_a_depth_of_two_hundred()
        {
            Assert.AreEqual(200, GetDepth(items[0].TestChain));
        }

        private int GetDepth(CallChain testChain)
        {
            if (testChain == null || testChain.Children == null || testChain.Children.Count == 0) return 0;
            return GetDepth(testChain.Children[0]) + 1;
        }
    }
}