using System.IO;
using AutoTest.Messages;

namespace AutoTest.VM.Messages
{
    public class TestProfilerCorruptedMessage : IMessage
    {
        public void SetDataFrom(BinaryReader reader)
        {
        }

        public void WriteDataTo(BinaryWriter writer)
        {
        }
    }
}