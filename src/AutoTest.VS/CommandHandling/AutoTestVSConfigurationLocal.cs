using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using AutoTest.Client;
using AutoTest.Client.Logging;

namespace AutoTest.VS.CommandHandling
{
    public class AutoTestVSConfigurationLocal : ICommandHandler
    {
        private ATEClient _client;

        public AutoTestVSConfigurationLocal(ATEClient client)
        {
            _client = client;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            Logger.Write("Initiating local configuration");
            _client.ShowConfiguration(true);
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = Connect.IsSolutionOpened
                               ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled
                               : vsCommandStatus.vsCommandStatusSupported;
        }

        public string Name
        {
            get { return "AutoTest.VS.Connect.AutoTestVSConfigurationLocal"; }
        }
    }
}
