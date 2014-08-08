using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using AutoTest.Client;
using AutoTest.VS.Util;
using EnvDTE80;
using AutoTest.Client.Logging;
using AutoTest.VS.Util.Builds;
using AutoTest.VS.Util.CommandHandling;
using ContinuousTests.VS;

namespace AutoTest.VS.CommandHandling
{
    class RunRelatedTests : ICommandHandler
    {
        private readonly ATEClient _client;
        private readonly DTE2 _application;
        private readonly IVSBuildRunner _buildRunner;

        public RunRelatedTests(ATEClient client, DTE2 application, IVSBuildRunner buildRunner)
        {
            _client = client;
            _application = application;
            _buildRunner = buildRunner;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            var method = new ElementNameFromCursorPosition(_application).Get();
            if (method == null)
                return;
            Logger.Write("Running tests related to " + method);
            if (_client.IsRunning || _buildRunner.Build())
                _client.RunRelatedTestsFor(method);
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = Connect.IsSolutionOpened
                               ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled
                               : vsCommandStatus.vsCommandStatusSupported;
        }

        public string Name
        {
            get { return "ContinuousTests.VS.Connect.ContinuousTests_RunRelatedTests"; }
        }
    }
}
