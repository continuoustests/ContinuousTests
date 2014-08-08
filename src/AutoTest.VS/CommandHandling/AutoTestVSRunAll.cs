using AutoTest.Client;
using AutoTest.Client.Logging;
using EnvDTE;
using EnvDTE80;
using AutoTest.VS.Util.Builds;
using AutoTest.VS.Util.CommandHandling;
using ContinuousTests.VS;

namespace AutoTest.VS.CommandHandling
{
    public class AutoTestVSRunAll : ICommandHandler
    {
        private readonly DTE2 _applicationObject;
        private readonly ATEClient _client;
        private readonly IVSBuildRunner _buildRunner;

        public AutoTestVSRunAll(DTE2 applicationObject, ATEClient client, IVSBuildRunner buildRunner)
        {
            _applicationObject = applicationObject;
            _client = client;
            _buildRunner = buildRunner;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            Logger.Write("Building all projects and running all tests");
            if (_client.IsRunning)
                _client.RunAll();
            else
                manualRunAll();
        }

        private void manualRunAll()
        {
            if (_buildRunner.Build())
                runTestsForSolution();
        }

        private void runTestsForSolution()
        {
            var dummy = new object();
            var boolValue = false;
            new RunTestsForSolution("ContinuousTests.VS.Connect.ContinuousTests_RunForSolution",
                () => { return !_client.IsRunning && Connect.IsSolutionOpened; }, () => { return _client.IsRunning; }, (tests) => _client.StartOnDemandRun(tests), _applicationObject, _buildRunner,
                (runs) => _client.SetLastRun(runs))
                .Exec(vsCommandExecOption.vsCommandExecOptionDoDefault, ref dummy, ref dummy, ref boolValue);
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = Connect.IsSolutionOpened
                               ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled
                               : vsCommandStatus.vsCommandStatusSupported;
        }

        public string Name
        {
            get { return "ContinuousTests.VS.Connect.ContinuousTests_RunAll"; }
        }
    }
}