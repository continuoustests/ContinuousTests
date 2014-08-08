using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AutoTest.Messages;

namespace AutoTest.VM.Messages
{
    public class IsSolutionInitializedRequest : ICustomBinarySerializable
    {
        public IsSolutionInitializedRequest() { }

        public void SetDataFrom(BinaryReader reader) { }
        public void WriteDataTo(BinaryWriter writer) { }
    }

    public class IsSolutionInitializedResponse : ReplyMessage, ICustomBinarySerializable
    {
        public override Guid CorrelationID { get; protected set; }
        public bool IsInitialized { get; private set; }

        public IsSolutionInitializedResponse() { }
        public IsSolutionInitializedResponse(Guid correlationID, bool isInitialized)
        {
            CorrelationID = correlationID;
            IsInitialized = isInitialized;
        }

        public void SetDataFrom(BinaryReader reader)
        {
            CorrelationID = new Guid(reader.ReadString());
            IsInitialized = reader.ReadBoolean();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(CorrelationID.ToString());
            writer.Write(IsInitialized);
        }
    }
}
