using AutoTest.Client;
using AutoTest.Client.Logging;
using EnvDTE;
using AutoTest.VS.Util.CommandHandling;
using ContinuousTests.VS;

namespace AutoTest.VS.CommandHandling
{
    public class AutoTestVSStop : ICommandHandler
    {
        private readonly ATEClient _client;

        public AutoTestVSStop(ATEClient client)
        {
            _client = client;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            Logger.Write("Pausing engine");
            _client.PauseEngine();
            Connect._control.Disconnecting(0);
            Connect._control.ClearList();
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            Logger.Write("client running = " + _client.IsRunning);
            Logger.Write("soution opened = " + Connect.IsSolutionOpened);
            StatusOption = _client.IsRunning && Connect.IsSolutionOpened
                               ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled
                               : vsCommandStatus.vsCommandStatusSupported;
            return;
        }

        public string Name
        {
            get { return "ContinuousTests.VS.Connect.ContinuousTests_PauseEngine"; }
        }
    }
}