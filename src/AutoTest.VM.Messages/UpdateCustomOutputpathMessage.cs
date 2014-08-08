using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class UpdateCustomOutputpathMessage : IMessage
    {
        public string Outputpath { get; private set; }

        public UpdateCustomOutputpathMessage(string path)
        {
            Outputpath = path;
        }

        public void SetDataFrom(BinaryReader reader)
        {
            Outputpath = reader.ReadString();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(Outputpath);
        }
    }
}
