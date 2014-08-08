using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE80;
using EnvDTE;
using AutoTest.VS.Util.Builds;
using AutoTest.Messages;

namespace AutoTest.VS.Util.CommandHandling
{
    public class RerunLastManualTestRun : ICommandHandler
    {
        private readonly string _commandName;
        private readonly Func<bool> _isEnabled;
        private readonly Func<bool> _manualBuild;
        private readonly Action _runTests;
        private readonly DTE2 _applicationObject;
        private readonly IVSBuildRunner _buildRunner;
        private readonly Func<IEnumerable<OnDemandRun>> _getLastTestRun;

        public RerunLastManualTestRun(string commandName, Func<bool> isEnabled, Func<bool> manualBuild, Action runTests, DTE2 applicationObject, IVSBuildRunner buildRunner, Func<IEnumerable<OnDemandRun>> getLastTestRun)
        {
            _commandName = commandName;
            _isEnabled = isEnabled;
            _manualBuild = manualBuild;
            _runTests = runTests;
            _applicationObject = applicationObject;
            _buildRunner = buildRunner;
            _getLastTestRun = getLastTestRun;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            if (!_manualBuild() || _buildRunner.Build(_getLastTestRun().Select(x => x.Project)))
                _runTests();
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = _isEnabled()
                               ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled
                               : vsCommandStatus.vsCommandStatusSupported;
        }

        public string Name
        {
            get { return _commandName; }
        }
    }
}
