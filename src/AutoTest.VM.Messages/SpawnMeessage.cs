using System;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class VMSpawnMeessage : IMessage
    {
        public Guid CorrelationId { get; private set; }
        public int Port { get; private set; }
        public string WatchPath { get; private set; }

        public VMSpawnMeessage(Guid correlationId, int port, string watchPath)
        {
            CorrelationId = correlationId;
            Port = port;
            WatchPath = watchPath;
        }
        
        public void SetDataFrom(BinaryReader reader)
        {
            CorrelationId = new Guid(reader.ReadString());
            Port = reader.ReadInt32();
            WatchPath = reader.ReadString();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(CorrelationId.ToString());
            writer.Write(Port);
            writer.Write(WatchPath);
        }
    }
}
