using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class GoToFileAndLineMessage : IMessage
    {
        public string File { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }

        public GoToFileAndLineMessage(string file, int line, int column)
        {
            File = file;
            Line = line;
            Column = column;
        }

        public void SetDataFrom(BinaryReader reader)
        {
            File = reader.ReadString();
            Line = reader.ReadInt32();
            Column = reader.ReadInt32();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(File);
            writer.Write(Line);
            writer.Write(Column);
        }
    }
}
