using System;
using System.IO;
namespace AutoTest.Messages
{
	public class BuildMessage : ICustomBinarySerializable
	{
		public string File { get; set; }
        public int LineNumber { get; set; }
        public int LinePosition { get; set; }
        public string ErrorMessage { get; set; }

        public void UpdateFile(string file)
        {
            File = file;
        }

        public override bool Equals(object obj)
        {
            var other = (BuildMessage)obj;
            return this.GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
        {
            // Overflow is fine, just wrap
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (File == null ? 0 : File.GetHashCode());
                hash = hash * 23 + LineNumber.GetHashCode();
                hash = hash * 23 + LinePosition.GetHashCode();
                hash = hash * 23 + (ErrorMessage == null ? 0 : ErrorMessage.GetHashCode());
                return hash;
            }
        }
	

		#region ICustomBinarySerializable implementation
		public void WriteDataTo(BinaryWriter writer)
		{
            writer.Write(File == null ? "" : File);
			writer.Write((int) LineNumber);
			writer.Write((int) LinePosition);
			writer.Write(ErrorMessage == null ? "" : ErrorMessage);
		}

		public void SetDataFrom (BinaryReader reader)
		{
			File = reader.ReadString();
			LineNumber = reader.ReadInt32();
			LinePosition = reader.ReadInt32();
			ErrorMessage = reader.ReadString();
		}
		#endregion
    }
}

