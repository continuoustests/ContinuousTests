using System;
using System.IO;
using AutoTest.Messages;

namespace AutoTest.VM.Messages
{
    public class RequestVisualGraphMessage : IMessage
    {
        public Guid CorrelationId { get; private set; }
        public string Item;

        public RequestVisualGraphMessage(string item)
        {
            Item = item;
            CorrelationId = Guid.NewGuid();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(CorrelationId.ToString());
            writer.Write(Item);
        }

        public void SetDataFrom(BinaryReader reader)
        {
            CorrelationId = new Guid(reader.ReadString());
            Item = reader.ReadString();
        }
    }
}