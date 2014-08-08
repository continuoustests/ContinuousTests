using AutoTest.VS.Util;
using EnvDTE;
using EnvDTE80;
using AutoTest.VS.Util.Builds;
using System;
using System.Collections.Generic;
using AutoTest.Messages;

namespace AutoTest.VS.Util.CommandHandling
{
    public class RunTestsUnderCursor : ICommandHandler
    {
        private readonly string _commandName;
        private readonly Func<bool> _isEnabled;
        private readonly Func<bool> _manualBuild;
        private readonly Action<OnDemandRun> _runTests;
        private readonly DTE2 _applicationObject;
        private readonly IVSBuildRunner _buildRunner;
        private readonly Action<OnDemandRun> _peek;

        public RunTestsUnderCursor(string commandName, Func<bool> isEnabled, Func<bool> manualBuild, Action<OnDemandRun> runTests, DTE2 applicationObject, IVSBuildRunner buildRunner, Action<OnDemandRun> peek)
        {
            _commandName = commandName;
            _isEnabled = isEnabled;
            _manualBuild = manualBuild;
            _runTests = runTests;
            _applicationObject = applicationObject;
            _buildRunner = buildRunner;
            _peek = peek;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            var pos = new OnDemandRunFromCursorPosition(_applicationObject);
            var types = pos.FromCurrentPosition();
            _peek(types);
            if (!_manualBuild() || _buildRunner.Build(new[] { types.Project }))
                _runTests(types);
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