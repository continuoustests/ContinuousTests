using System;
using System.IO;
namespace AutoTest.Messages
{
	public class BuildRunMessage : IMessage, ICustomBinarySerializable
    {
        private BuildRunResults _results;

        public BuildRunResults Results { get { return _results; } }

        public BuildRunMessage(BuildRunResults results)
        {
            _results = results;
        }

		#region ICustomBinarySerializable implementation
		public void WriteDataTo(BinaryWriter writer)
		{
			_results.WriteDataTo(writer);
		}

		public void SetDataFrom(BinaryReader reader)
		{
			_results = new BuildRunResults("");
			_results.SetDataFrom(reader);
		}
		#endregion
    }
}

