using AutoTest.Messages;

namespace AutoTest.VM.Messages
{
    public class VMTerminating : IMessage
    {
        public int Port { get; private set; }

        public VMTerminating(int port)
        {
            Port = port;
        }

        public void SetDataFrom(System.IO.BinaryReader reader)
        {
            Port = reader.ReadInt32();
        }

        public void WriteDataTo(System.IO.BinaryWriter writer)
        {
            writer.Write((int)Port);
        }
    }
}
