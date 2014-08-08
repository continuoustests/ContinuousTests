using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class RegisterLicenseMessage : IMessage
    {
        public string Key { get; private set; }

        public RegisterLicenseMessage(string key)
        {
            Key = key;
        }

        public void SetDataFrom(BinaryReader reader)
        {
            Key = reader.ReadString();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(Key);
        }
    }
}
