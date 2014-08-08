using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;

namespace AutoTest.Core.BuildRunners
{
    public interface IPreProcessBuildruns
    {
        RunInfo[] PreProcess(RunInfo[] details);
        BuildRunResults PostProcessBuildResults(BuildRunResults runResults);
        RunInfo[] PostProcess(RunInfo[] details, ref RunReport runReport);
    }
}
