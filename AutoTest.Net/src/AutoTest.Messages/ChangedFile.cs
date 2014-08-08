using System;
using System.IO;
namespace AutoTest.Messages
{
	public class ChangedFile : ICustomBinarySerializable
	{
		private string _fullName;
        private string _name;
        private string _extension;

        public string Extension { get { return _extension; } }
        public string FullName { get { return _fullName; } }
        public string Name { get { return _name; } }
		
		public ChangedFile()
		{
			_fullName = "";
			_name = "";
			_extension = "";
		}

        public ChangedFile(string fullFilePath)
        {
            if (fullFilePath == null)
                throw new ArgumentNullException("File path cannot be null");
            _fullName = fullFilePath;
            _name = Path.GetFileName(_fullName);
            _extension = Path.GetExtension(_name);
        }

		#region ICustomBinarySerializable implementation
		public void WriteDataTo(BinaryWriter writer)
		{
			writer.Write((string) _fullName);
			writer.Write((string) _name);
			writer.Write((string) _extension);
		}

		public void SetDataFrom(BinaryReader reader)
		{
			_fullName = reader.ReadString();
			_name = reader.ReadString();
			_extension = reader.ReadString();
		}
		#endregion
}
}

