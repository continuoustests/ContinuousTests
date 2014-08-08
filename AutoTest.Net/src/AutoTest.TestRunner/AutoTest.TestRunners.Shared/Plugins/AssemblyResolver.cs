using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Runners.Shared.Plugins
{
    class AssemblyResolver
    {
        IAutoTestNetTestRunner Resolve(string assembly, string typeName)
        {
            return (IAutoTestNetTestRunner) Activator.CreateInstance(assembly, typeName);
        }
    }
}
