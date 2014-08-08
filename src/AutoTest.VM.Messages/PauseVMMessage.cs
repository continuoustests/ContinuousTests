using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class PauseVMMessage : IMessage
    {
        public void SetDataFrom(BinaryReader reader)
        {
            reader.ReadString();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write((string)"hello");
        }
    }
}
