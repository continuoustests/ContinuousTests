using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE80;
using EnvDTE;
using AutoTest.VS.Util.Debugger;
using AutoTest.VS.Util.Builds;

namespace AutoTest.VS.Util.CommandHandling
{
    public class RerunLastDebugSession : ICommandHandler
    {
        private readonly string _commandName;
        private readonly Func<bool> _isEnabled;
        private readonly Func<bool> _manualBuild;
        private readonly Action _debug;
        private readonly DTE2 _applicationObject;
        private readonly IVSBuildRunner _buildRunner;

        public RerunLastDebugSession(string commandName, Func<bool> isEnabled, Func<bool> manualBuild, Action debug, DTE2 applicationObject, IVSBuildRunner buildRunner)
        {
            _commandName = commandName;
            _isEnabled = isEnabled;
            _manualBuild = manualBuild;
            _debug = debug;
            _applicationObject = applicationObject;
            _buildRunner = buildRunner;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            if (!_manualBuild() || _buildRunner.Build())
                _debug();
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
