using AutoTest.Client;
using AutoTest.Client.Logging;
using AutoTest.VS.Util;
using EnvDTE;
using EnvDTE80;
using AutoTest.VS.Util.CommandHandling;

namespace AutoTest.VS.CommandHandling
{
    public class GetAffectedCodeGraph : ICommandHandler
    {
        private readonly ATEClient _client;
        private readonly DTE2 _applicationObject;

        public GetAffectedCodeGraph(ATEClient client, DTE2 applicationObject)
        {
            _client = client;
            _applicationObject = applicationObject;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            Logger.Write("Getting coupling graph");
            var methodName = GetMethodNameAtCursor();
            if (methodName != null)
                _client.GetGraphFor(methodName);
        }

        private string GetMethodNameAtCursor()
        {
            return new ElementNameFromCursorPosition(_applicationObject).Get();
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
        }

        public string Name
        {
            get { return "ContinuousTests.VS.Connect.ContinuousTests_GetAffectedCodeGraph"; }
        }
    }


}