using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class AssemblyPathRequest : ICustomBinarySerializable
    {
        public string Project { get; private set; }

        public AssemblyPathRequest() { }
        public AssemblyPathRequest(string project)
        {
            Project = project;
        }

        public void SetDataFrom(BinaryReader reader)
        {
            Project = reader.ReadString();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(Project);
        }
    }

    public class AssemblyPathResponse : ReplyMessage, ICustomBinarySerializable
    {
        public override Guid CorrelationID { get; protected set; }
        public string Assembly { get; private set; }

        public AssemblyPathResponse() { }
        public AssemblyPathResponse(Guid correlationID, string assembly)
        {
            CorrelationID = correlationID;
            Assembly = assembly;
        }

        public void SetDataFrom(BinaryReader reader)
        {
            CorrelationID = new Guid(reader.ReadString());
            Assembly = reader.ReadString();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(CorrelationID.ToString());
            writer.Write(Assembly);
        }
    }
}
