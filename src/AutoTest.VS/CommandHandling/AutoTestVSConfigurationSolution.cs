using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using AutoTest.Client;
using AutoTest.Client.Logging;
using AutoTest.VS.Util.CommandHandling;
using ContinuousTests.VS;

namespace AutoTest.VS.CommandHandling
{
    public class AutoTestVSConfigurationSolution : ICommandHandler
    {
        private ATEClient _client;

        public AutoTestVSConfigurationSolution(ATEClient client)
        {
            _client = client;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            Logger.Write("Initiating local configuration");
            new System.Threading.Thread(() => _client.ShowConfiguration(true)).Start();
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = Connect.IsSolutionOpened
                               ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled
                               : vsCommandStatus.vsCommandStatusSupported;
        }

        public string Name
        {
            get { return "ContinuousTests.VS.Connect.ContinuousTests_SolutionConfiguration"; }
        }
    }
}
