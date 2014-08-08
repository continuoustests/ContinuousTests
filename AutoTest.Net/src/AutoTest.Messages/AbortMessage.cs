using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTest.Messages
{
    public class AbortMessage : IMessage
    {
        public string Reason { get; private set; }

        public AbortMessage(string reason)
        {
            Reason = reason;
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(Reason);
        }

        public void SetDataFrom(BinaryReader reader)
        {
            Reason = reader.ReadString();
        }
    }
}
