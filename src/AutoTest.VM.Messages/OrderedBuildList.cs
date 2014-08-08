using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class OrderedBuildList : ReplyMessage, ICustomBinarySerializable
    {
        public override Guid CorrelationID { get; protected set; }

        public List<string> Projects { get; private set; }

        public OrderedBuildList() { }
        public OrderedBuildList(IEnumerable<string> projects)
        {
            Projects = new List<string>(projects);
        }
        public OrderedBuildList(Guid guid, IEnumerable<string> projects)
        {
            CorrelationID = guid;
            Projects = new List<string>(projects);
        }

        public void SetDataFrom(BinaryReader reader)
        {
            CorrelationID = new Guid(reader.ReadString());
            Projects = new List<string>();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                Projects.Add(reader.ReadString());
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(CorrelationID.ToString());
            writer.Write(Projects.Count);
            foreach (var project in Projects)
                writer.Write(project);
        }
    }
}
