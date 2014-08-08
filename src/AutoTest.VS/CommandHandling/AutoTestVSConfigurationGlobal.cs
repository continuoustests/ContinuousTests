using AutoTest.Client;
using AutoTest.Client.Logging;
using EnvDTE;
using System.IO;
using System;
using AutoTest.VS.Util.CommandHandling;
using ContinuousTests.VS;

namespace AutoTest.VS.CommandHandling
{
    public class AutoTestVSConfigurationGlobal : ICommandHandler
    {
        private ATEClient _client;

        public AutoTestVSConfigurationGlobal(ATEClient client)
        {
            _client = client;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            Logger.Write("Initiating global configuration");
            _client.ShowConfiguration(false);
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = vsCommandStatus.vsCommandStatusInvisible | vsCommandStatus.vsCommandStatusUnsupported;
            //var config = Path.Combine(Path.Combine(Environment.GetFolderPath(_client.GetAppDataFolder()), "MightyMoose"), "AutoTest.config");
            //StatusOption = File.Exists(config) || Connect.IsSolutionOpened
            //                    ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled
            //                    : vsCommandStatus.vsCommandStatusSupported;
        }

        public string Name
        {
            get { return "ContinuousTests.VS.Connect.ContinuousTests_GlobalConfiguration"; }
        }
    }
}