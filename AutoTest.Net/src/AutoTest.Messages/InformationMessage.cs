using System;
using System.IO;
namespace AutoTest.Messages
{
	[Serializable]
	public class InformationMessage : IMessage, ICustomBinarySerializable
    {
        private string _message;

        public string Message { get { return _message; } }

        public InformationMessage(string message)
        {
            _message = message;
        }

		#region ICustomBinarySerializable implementation
		public void WriteDataTo(BinaryWriter writer)
		{
			writer.Write((string) _message);
		}

		public void SetDataFrom(BinaryReader reader)
		{
			_message = reader.ReadString();
		}
		#endregion
}
}

