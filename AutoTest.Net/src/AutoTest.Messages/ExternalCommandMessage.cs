using System;
using System.IO;
namespace AutoTest.Messages
{
	public class ExternalCommandMessage : IMessage, ICustomBinarySerializable
	{
		public string Sender { get; private set; }
		public string Command { get; private set; }
		
		public ExternalCommandMessage(string sender, string command)
		{
			Sender = sender;
			Command = command;
		}

		public void WriteDataTo(BinaryWriter writer)
		{
			writer.Write(Sender);
			writer.Write(Command);
		}

		public void SetDataFrom(BinaryReader reader)
		{
			Sender = reader.ReadString();
			Command = reader.ReadString();
		}
	}
}

