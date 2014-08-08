using System;
namespace AutoTest.VS.Util.Debugger
{
    interface IAutoTestRunnerDebugProcess
    {
        int ID { get; }
        void Resume();
        int StartPaused(AutoTest.TestRunners.Shared.Options.RunOptions options, AutoTest.Messages.TestRunner runner);
    }
}
