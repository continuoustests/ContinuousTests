using System;
using EnvDTE;

namespace AutoTest.VS.Util.CommandHandling
{
    public class CommandHandler : ICommandHandler
    {
        protected string _name;

        public virtual void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled) {
            throw new NotImplementedException();
        }

        public virtual void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            throw new NotImplementedException();
        }

        public string Name { get { return _name;  }}
    }
}