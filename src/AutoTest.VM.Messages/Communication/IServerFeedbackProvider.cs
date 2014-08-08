namespace AutoTest.VM.Messages.Communication
{
    public interface IServerFeedbackProvider
    {
        void ServerStarted(string ip, int port);
        void ClientConnected(string ip, int port, int clientCount);
        void RemovingClient(string ip, int port, string reason, int clientCount);
        void OnError(string ex);
    }
}
