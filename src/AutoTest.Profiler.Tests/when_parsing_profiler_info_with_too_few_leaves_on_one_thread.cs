using System.Linq;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class when_parsing_profiler_info_with_too_few_leaves_on_one_thread
    {
        //TODO: GREG is this a valid case?
        //[Test, ExpectedException(typeof(ProfilerMissingLeavesException))]
        //public void a_stack_underflow_exception_is_thrown()
        //{
        //    var stream = DataBuilder.Create(
        //        Entry.Enter(1).At(43.5).WithFunctionId(12345).For("Method").RuntimeOf("RuntimeMethod").OnThread(1),
        //        Entry.Enter(2).At(43.5).WithFunctionId(12346).For("Method").RuntimeOf("RuntimeMethod").OnThread(2),
        //        Entry.Leave(3).At(43.6).WithFunctionId(12346).OnThread(2)
        //        );

        //    var parser = new BinaryFileProfilerDataParser();
        //    parser.Parse(stream).ToList();
        //}
    }
}