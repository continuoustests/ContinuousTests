using System.Collections.Generic;
using EnvDTE;

namespace AutoTest.VS.Util.CommandHandling
{
    public class CommandDispatcher
    {
        private readonly Dictionary<string, ICommandHandler> handlers = new Dictionary<string, ICommandHandler>();
        public void RegisterHandler(ICommandHandler handler)
        {
            if (handlers.ContainsKey(handler.Name)) return;
            handlers.Add(handler.Name, handler);
        }

        public void DispatchExec(string commandName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            var handler = GetHandler(commandName);
            if(handler!=null)
                handler.Exec(ExecuteOption, ref VariantIn, ref VariantOut, ref Handled);
        }

        private ICommandHandler GetHandler(string name)
        {
            return handlers[name];
        }

        public void QueryStatus(string commandName, vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            var handler = GetHandler(commandName);
            if (handler != null)
                handler.QueryStatus(NeededText, ref StatusOption, ref CommandText);
        }
    }
}