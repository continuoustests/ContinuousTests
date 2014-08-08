using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;

namespace AutoTest.Core.BuildRunners
{
    class NullBuildPreProcessor : IPreProcessBuildruns
    {
        public RunInfo[] PreProcess(RunInfo[] details)
        {
            return details;
        }

        public BuildRunResults PostProcessBuildResults(BuildRunResults runResults)
        {
            return runResults;
        }

        public RunInfo[] PostProcess(RunInfo[] details, ref RunReport runReport)
        {
            return details;
        }
    }
}
