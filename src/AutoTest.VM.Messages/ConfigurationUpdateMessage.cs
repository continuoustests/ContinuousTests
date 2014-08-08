using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class ConfigurationUpdateMessage : IMessage
    {
        public bool RequiresRestart { get; private set; }
        public string GlobalFile { get; private set; }
        public string GlobalContent { get; private set; }
        public string LocalFile { get; private set; }
        public string LocalContent { get; private set; }

        public ConfigurationUpdateMessage(bool requiresRestart, string globalFile, string globalContent, string localFile, string localContent)
        {
            RequiresRestart = requiresRestart;
            GlobalFile = globalFile;
            GlobalContent = globalContent;
            LocalFile = localFile;
            LocalContent = localContent;
        }

        public void SetDataFrom(BinaryReader reader)
        {
            RequiresRestart = reader.ReadBoolean();
            GlobalFile = reader.ReadString();
            GlobalContent = reader.ReadString();
            LocalFile = reader.ReadString();
            LocalContent = reader.ReadString();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(RequiresRestart);
            writer.Write(GlobalFile);
            writer.Write(GlobalContent);
            writer.Write(LocalFile);
            writer.Write(LocalContent);
        }
    }
}
