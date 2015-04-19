using System.IO;
using System.Collections.Generic;
namespace AutoTest.Messages
{
	public class RunStartedMessage : IMessage, ICustomBinarySerializable
    {
	    public ChangedFile[] Files { get; private set; }

	    public RunStartedMessage(ChangedFile[] files)
        {
            Files = files;
        }

        public void WriteDataTo (BinaryWriter writer)
		{
			writer.Write((int) Files.Length);
			foreach (var file in Files)
				file.WriteDataTo(writer);
		}

		public void SetDataFrom (BinaryReader reader)
		{
			var files = new List<ChangedFile>();
			var count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				var file = new ChangedFile();
				file.SetDataFrom(reader);
				files.Add(file);
			}
			Files = files.ToArray();
		}
		
    }
}

