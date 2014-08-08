using System.IO;
using System.Net.Sockets;

namespace AutoTest.VM.Messages.Communication
{
    class ConnectedClient
    {
        public bool IsDiagnostic { get; private set; }
        public NetworkStream Stream { get; private set; }
        public byte[] Buffer { get; set; }
        public MemoryStream ReadBuffer { get; set; }

        public ConnectedClient(NetworkStream client, byte [] buffer, MemoryStream readBuffer)
        {
            IsDiagnostic = false;
            Stream = client;
            Buffer = buffer;
            ReadBuffer = readBuffer;
        }

        public void SetAsDiagnostic()
        {
            IsDiagnostic = true;
        }
    }
}
