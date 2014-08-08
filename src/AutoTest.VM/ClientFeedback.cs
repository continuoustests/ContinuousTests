using AutoTest.VM.Messages.Communication;

namespace AutoTest.VM
{
    class ClientFeedback : IClientFeedbackProvider
    {
        public void OnError(string ex)
        {
            Logger.WriteError(ex);
        }
    }
}
