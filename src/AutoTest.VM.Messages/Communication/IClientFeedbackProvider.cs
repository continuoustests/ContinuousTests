namespace AutoTest.VM.Messages.Communication
{
    public interface IClientFeedbackProvider
    {
        void OnError(string ex);
    }
}
