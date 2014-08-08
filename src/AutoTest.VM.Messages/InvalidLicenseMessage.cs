using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class InvalidLicenseMessage : IMessage
    {
        public void SetDataFrom(BinaryReader reader)
        {
        }

        public void WriteDataTo(BinaryWriter writer)
        {
        }
    }
}
