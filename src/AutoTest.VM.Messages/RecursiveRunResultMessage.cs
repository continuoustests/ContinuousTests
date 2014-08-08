using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AutoTest.Messages;

namespace AutoTest.VM.Messages
{
    public class RecursiveRunResultMessage : IMessage
    {
        public string[] Files { get; private set; }

        public RecursiveRunResultMessage(string[] files)
        {
            Files = files;
        }

        public void SetDataFrom(BinaryReader reader)
        {
            var files = new List<string>();
            var length = reader.ReadInt32();
            for (int i = 0; i < length; i++)
                files.Add(reader.ReadString());
            Files = files.ToArray();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write((int)Files.Length);
            foreach (var file in Files)
                writer.Write(file);
        }
    }
}
