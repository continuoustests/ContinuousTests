using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class PartialRunMessage : IMessage
    {
        public string[] Projects { get; private set; }

        public PartialRunMessage(IEnumerable<string> projects)
        {
            Projects = projects.ToArray();
        }

        public void SetDataFrom(BinaryReader reader)
        {
            var projects = new List<string>();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                projects.Add(reader.ReadString());
            Projects = projects.ToArray();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(Projects.Length);
            foreach (var project in Projects)
                writer.Write(project);
        }
    }
}
