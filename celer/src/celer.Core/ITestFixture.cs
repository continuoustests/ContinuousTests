using System.Collections.Generic;
using System.Reflection;

namespace celer.Core
{
    public interface ITestFixture
    {
        IEnumerable<RunResult> Run();
        IEnumerable<RunResult> Run(IEnumerable<MethodInfo> methods);
    }
}