using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class AssembliesMinimizedMessage : IMessage
    {
        public void SetDataFrom(BinaryReader reader)
        {
            int x = 5;
        }

        public void WriteDataTo(BinaryWriter writer)
        {
        }
    }
}
