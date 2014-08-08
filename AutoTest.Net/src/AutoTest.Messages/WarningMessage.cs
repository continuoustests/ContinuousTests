using System.IO;
namespace AutoTest.Messages
{
	public class WarningMessage : IMessage, ICustomBinarySerializable
    {
        public string Warning { get; private set; }

        public WarningMessage(string warning)
        {
            Warning = warning;
        }

		public void WriteDataTo (BinaryWriter writer)
		{
			writer.Write((string) Warning);
		}

		public void SetDataFrom (BinaryReader reader)
		{
			Warning = reader.ReadString();
		}
	}
}

