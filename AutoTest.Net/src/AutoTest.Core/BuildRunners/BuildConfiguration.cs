using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.BuildRunners
{
    public class BuildConfiguration
    {
        public Func<string, string, bool> OptimisticBuildStrategy { get; private set; }

        public BuildConfiguration(Func<string, string, bool> optimisticBuildStrategy)
        {
            OptimisticBuildStrategy = optimisticBuildStrategy;
        }
    }
}
