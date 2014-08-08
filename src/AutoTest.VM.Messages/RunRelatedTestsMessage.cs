using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class RunRelatedTestsMessage : IMessage
    {
        public string Member;

        public RunRelatedTestsMessage(string member)
        {
            Member = member;
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(Member);
        }

        public void SetDataFrom(BinaryReader reader)
        {
            Member = reader.ReadString();
        }
    }
}
