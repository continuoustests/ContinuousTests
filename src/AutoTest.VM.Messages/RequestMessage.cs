using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class RequestMessage<TRequest, TReply> : ICustomBinarySerializable
        where TRequest : ICustomBinarySerializable, new()
        where TReply : ReplyMessage, ICustomBinarySerializable, new()
    {
        public Guid CorrelationID { get; private set; }
        public TRequest Request { get; private set; }

        public RequestMessage(TRequest request)
        {
            CorrelationID = Guid.NewGuid();
            Request = request;
        }

        public void SetDataFrom(BinaryReader reader)
        {
            CorrelationID = new Guid(reader.ReadString());
            Request = new TRequest();
            Request.SetDataFrom(reader);
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(CorrelationID.ToString());
            Request.WriteDataTo(writer);
        }
    }
}
