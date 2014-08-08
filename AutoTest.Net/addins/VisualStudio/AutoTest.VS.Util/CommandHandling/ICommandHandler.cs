using EnvDTE;

namespace AutoTest.VS.Util.CommandHandling
{
    public interface ICommandHandler
    {
        void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled);
        void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText);
        string Name { get;}
    }
}