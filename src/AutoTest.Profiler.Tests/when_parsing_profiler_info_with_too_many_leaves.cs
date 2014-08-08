using System;
using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
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
}