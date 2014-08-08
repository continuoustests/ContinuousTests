using AutoTest.Client;
using AutoTest.Client.Logging;
using EnvDTE;
using AutoTest.VS.Util.CommandHandling;
using ContinuousTests.VS;

namespace AutoTest.VS.CommandHandling
{
    public class AutoTestVSStart : ICommandHandler
    {
        private readonly ATEClient _client;

        public AutoTestVSStart(ATEClient client)
        {
            _client = client;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            Logger.Write("Resuming engine");
            _client.ResumeEngine();
            Connect._control.Connecting(0, false);
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = !_client.IsRunning && Connect.IsSolutionOpened
                               ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled
                               : vsCommandStatus.vsCommandStatusSupported;
        }

        public string Name
        {
            get { return "ContinuousTests.VS.Connect.ContinuousTests_ResumeEngine"; }
        }
    }
}