using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared.Options
{
    [Serializable]
    public class RunSettings
    {
        public AssemblyOptions Assembly { get; private set; }
        public string[] IgnoreCategories { get; private set; }
        public string PipeName { get; private set; }

        public RunSettings(AssemblyOptions assembly, string[] ignoreCategories, string pipeName)
        {
            Assembly = assembly;
            IgnoreCategories = ignoreCategories;
            PipeName = pipeName;
        }
    }
}
