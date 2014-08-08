using System;

namespace AutoTest.VM.Messages.Communication
{
    public interface ITcpServer
    {
        int Port { get; }
        int ClientCount { get; }

        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        void Send(object o);
        void StartServer(string ip, int port);
        void Stop();
    }
}
