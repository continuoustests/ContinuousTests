using AutoTest.VM.Messages.Communication;
using System;

namespace AutoTest.VM
{
    class ClientDisconnectedArgs : EventArgs
    {
        public int ClientCount { get; private set; }

        public ClientDisconnectedArgs(int clientCount)
        {
            ClientCount = clientCount;
        }
    }

    class ServerFeedback : IServerFeedbackProvider
    {
        public event EventHandler<ClientDisconnectedArgs> ClientDisconnected;
        public event EventHandler<ClientDisconnectedArgs> ClientConnect;

        public void ServerStarted(string ip, int port)
        {
        }

        public void ClientConnected(string ip, int port, int clientCount)
        {
            Logger.WriteDebug(string.Format("Client connected to {0}:{1} (number of connected clients {2})", ip, port, clientCount));
            if (ClientConnect != null)
                ClientConnect(this, new ClientDisconnectedArgs(clientCount));
        }

        public void RemovingClient(string ip, int port, string reason, int clientCount)
        {
            Logger.WriteDebug(string.Format("Client disconnected from {0}:{1} number of connected clients {3} because {4}{2}{4}", ip, port, reason, clientCount, Environment.NewLine));
            if (ClientDisconnected != null)
                ClientDisconnected(this, new ClientDisconnectedArgs(clientCount));
        }

        public void OnError(string ex)
        {
            Logger.WriteError(ex);
        }
    }
}
