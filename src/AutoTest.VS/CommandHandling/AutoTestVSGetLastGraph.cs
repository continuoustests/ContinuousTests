using AutoTest.Client;
using AutoTest.VS.Util.CommandHandling;
using ContinuousTests.VS;
using EnvDTE;

namespace AutoTest.VS.CommandHandling
{
    class AutoTestVSGetLastGraph : ICommandHandler
    {
        private readonly ATEClient _client;

        public AutoTestVSGetLastGraph(ATEClient client)
        {
            _client = client;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            _client.GetLastAffectedGraph();
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = Connect.IsSolutionOpened
                               ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled
                               : vsCommandStatus.vsCommandStatusSupported;
        }

        public string Name
        {
            get { return "ContinuousTests.VS.Connect.ContinuousTests_LastGraph"; }
        }
    }
}