using AutoTest.Messages;
using AutoTest.Core.Caching.Projects;
using System;
using AutoTest.Core.Messaging.MessageConsumers;

namespace AutoTest.Core.BuildRunners
{
    public interface IBuildRunner
    {
        BuildRunResults RunBuild(string solution, bool rebuild, string buildExecutable, Func<bool> abortIfTrue);
        BuildRunResults RunBuild(RunInfo project, string buildExecutable, Func<bool> abortIfTrue);
    }
}