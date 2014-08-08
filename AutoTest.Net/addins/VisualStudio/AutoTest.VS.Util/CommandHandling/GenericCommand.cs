using EnvDTE;
using System;

namespace AutoTest.VS.Util.CommandHandling
{
    public class GenericCommand : ICommandHandler
    {
        private readonly string _commandName;
        private readonly Func<bool> _isEnabled;
        private readonly Action _command;

        public GenericCommand(string commandName, Func<bool> isEnabled, Action command)
        {
            _commandName = commandName;
            _isEnabled = isEnabled;
            _command = command;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            _command();
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