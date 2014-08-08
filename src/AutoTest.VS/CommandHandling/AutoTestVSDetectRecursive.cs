using AutoTest.Client;
using AutoTest.Client.Logging;
using EnvDTE;
using AutoTest.VS.Util.CommandHandling;
using ContinuousTests.VS;

namespace AutoTest.VS.CommandHandling
{
    public class AutoTestVSDetectRecursive : ICommandHandler
    {
        private readonly ATEClient _client;
        
        public AutoTestVSDetectRecursive(ATEClient client)
        {
            _client = client;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            Logger.Write("Initiating recursive detection run");
            _client.RunRecursiveRunDetection();
            Connect._control.NotifyAboutUpcomingRecursiveRun();
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = _client.IsRunning && Connect.IsSolutionOpened
                               ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled
                               : vsCommandStatus.vsCommandStatusSupported;
        }

        public string Name
        {
            get { return "ContinuousTests.VS.Connect.ContinuousTests_DetectRecursion"; }
        }
    }
}