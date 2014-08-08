using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    public interface IReflectionProvider : IDisposable
    {
        string GetName();
        Version GetTargetFramework();
        Targeting.Platform GetPlatform();
        IEnumerable<TypeName> GetReferences();

        string GetParentType(string type);
        SimpleClass LocateClass(string type);
        SimpleMethod LocateMethod(string type);
    }
}
