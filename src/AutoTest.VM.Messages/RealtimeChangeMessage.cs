using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class RealtimeChangeList : IMessage
    {
        public List<RealtimeChangeMessage> Messages { get; private set; }

        public RealtimeChangeList(IEnumerable<RealtimeChangeMessage> messages)
        {
            Messages = new List<RealtimeChangeMessage>(messages);
        }

        public void SetDataFrom(BinaryReader reader)
        {
            Messages = new List<RealtimeChangeMessage>();
            var count = reader.ReadInt32();
            for (int i = 0; count > i; i++)
            {
                var message = new RealtimeChangeMessage("", "", "");
                message.SetDataFrom(reader);
                Messages.Add(message);
            }
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(Messages.Count);
            foreach (var message in Messages)
                message.WriteDataTo(writer);
        }
    }

    public class RealtimeChangeMessage : IMessage
    {
        public string Project { get; private set; }
        public string File { get; private set; }
        public string Content { get; private set; }

        public RealtimeChangeMessage(string project, string file, string content)
        {
            Project = project;
            File = file;
            Content = content;
        }

        public void SetDataFrom(BinaryReader reader)
        {
            Project = reader.ReadString();
            File = reader.ReadString();
            Content = reader.ReadString();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(Project);
            writer.Write(File);
            writer.Write(Content);
        }
    }
}
