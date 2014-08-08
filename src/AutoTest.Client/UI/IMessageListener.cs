namespace AutoTest.Client.UI
{
    public interface IMessageListener
    {
        void Connecting(int port, bool startPaused);
        void Disconnecting(int port);
        void IncomingMessage(object message);
    }
}
