using System;
using System.Linq;
using ContinuousTests.ExtensionModel.Arguments;

namespace ContinuousTests.ExtensionModel.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new EngineRepository().GetEngine(@"C:\Users\ack\src\AutoTest.Net\AutoTest.net.sln", true);
            if (engine == null)
                engine = new EngineRepository().StartEngine(@"C:\Users\ack\src\AutoTest.Net\AutoTest.net.sln", @"C:\Users\ack\src\AutoTestExtensions\src\AutoTest.VS\bin");
            engine.EngineConnecting += engine_EngineConnecting;
            engine.EngineDisconnecting += engine_EngineDisconnecting;
            engine.SessionStarted += engine_SessionStarted;
            engine.SessionFinished += engine_SessionFinished;
            engine.BuildFinished += engine_BuildFinished;
            engine.TestsFinished += engine_TestsFinished;
            engine.TestProgress += engine_TestProgress;
            engine.Connect();
            System.Console.WriteLine("Waiting for connection");
            engine.WaitForConnection();
            System.Console.WriteLine("System connected");
            engine.RunAll();
            //engine.RunPartial(@"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Test\AutoTest.Test.csproj");
            //engine.RunTests(new TestScope(@"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Test\AutoTest.Test.csproj"));
            System.Console.ReadLine();
            engine.Disconnect();
        }

        static void engine_EngineDisconnecting(object sender, EventArgs e)
        {
            System.Console.WriteLine("Engine disconnecting");
        }

        static void engine_TestProgress(object sender, ImmediateTestFeedbackArgs e)
        {
            System.Console.WriteLine("Completed {0} tests, current assembly {1}", e.TestsCompleted, e.CurrentAssembly);
        }

        static void engine_TestsFinished(object sender, TestsFinishedArgs e)
        {
            e.All.ToList()
                .ForEach(x => System.Console.WriteLine("{1} {0}", x.Name, x.Status));
        }

        static void engine_BuildFinished(object sender, BuildFinishedArgs e)
        {
            System.Console.WriteLine(string.Format("Built {0} with {1} errors and {2} warnings", e.Project, e.ErrorCount, e.WarningCount));
        }

        static void engine_SessionFinished(object sender, SessionFinishedArgs e)
        {
            System.Console.WriteLine("Run finished with {0} failed builds and {1} tests", e.Run.NumberOfBuildsFailed, e.Run.NumberOfTestsRan);
        }

        static void engine_SessionStarted(object sender, EventArgs e)
        {
            System.Console.WriteLine("Run started");
        }

        static void engine_EngineConnecting(object sender, EventArgs e)
        {
            System.Console.WriteLine("Engine connecting");
        }
    }
}
