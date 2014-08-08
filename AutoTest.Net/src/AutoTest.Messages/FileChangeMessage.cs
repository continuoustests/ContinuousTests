using System;
using System.IO;
using System.Collections.Generic;
namespace AutoTest.Messages
{
	public class FileChangeMessage : IMessage
    {
        private List<ChangedFile> _files = new List<ChangedFile>();

        public ChangedFile[] Files { get { return _files.ToArray(); } }

        public void AddFile(ChangedFile file)
        {
            _files.Add(file);
        }

        public void AddFile(ChangedFile[] files)
        {
            _files.AddRange(files);
        }
		
		#region ICustomBinarySerializable implementation
		public void WriteDataTo(BinaryWriter writer)
		{
			writer.Write((int) _files.Count);
			foreach (var file in _files)
				file.WriteDataTo(writer);
		}

		public void SetDataFrom(BinaryReader reader)
		{
			_files = new List<ChangedFile>();
			var count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				var file = new ChangedFile();
				file.SetDataFrom(reader);
				_files.Add(file);
			}
		}
		#endregion
    }
}

