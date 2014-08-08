using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Client;
using EnvDTE;
using AutoTest.Client.Logging;
using AutoTest.VS.Util.CommandHandling;
using ContinuousTests.VS;

namespace AutoTest.VS.CommandHandling
{
    class AutoTestVSClearTestCache : ICommandHandler
    {
        private readonly ATEClient _client;

        public AutoTestVSClearTestCache(ATEClient client)
        {
            _client = client;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            var dummy = new object();
            var dummyBool = false;
            Logger.Write("Clearing cached tests");
            new AutoTestVSStop(_client).Exec(vsCommandExecOption.vsCommandExecOptionDoDefault, ref dummy, ref dummy, ref dummyBool);
            System.Threading.Thread.Sleep(500);
            new AutoTestVSStart(_client).Exec(vsCommandExecOption.vsCommandExecOptionDoDefault, ref dummy, ref dummy, ref dummyBool);
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = Connect.IsSolutionOpened
                               ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled
                               : vsCommandStatus.vsCommandStatusSupported;
        }

        public string Name
        {
            get { return "ContinuousTests.VS.Connect.ContinuousTests_ClearTestCache"; }
        }
    }
}
