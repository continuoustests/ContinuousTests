using AutoTest.Client;
using AutoTest.Client.Logging;
using EnvDTE;
using AutoTest.VS.Util.CommandHandling;

namespace AutoTest.VS.CommandHandling
{
    public class AutoTestVSAbout : ICommandHandler
    {
        private ATEClient _client;

        public AutoTestVSAbout(ATEClient client)
        {
            _client = client;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            Logger.Write("About ContinuousTests");
            _client.ShowAboutBox();
            return;
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
        }

        public string Name
        {
            get { return "ContinuousTests.VS.Connect.ContinuousTests_About"; }
        }
    }
}