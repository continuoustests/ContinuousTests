using System;
using System.IO;
namespace AutoTest.Messages
{
	public class RunFinishedMessage : IMessage, ICustomBinarySerializable
	{
		public RunReport Report { get; private set; }

        public RunFinishedMessage(RunReport report) 
        {
            Report = report;
        }

		#region ICustomBinarySerializable implementation
		public void WriteDataTo (BinaryWriter writer)
		{
			Report.WriteDataTo(writer);
		}

		public void SetDataFrom (BinaryReader reader)
		{
			Report = new RunReport();
			Report.SetDataFrom(reader);
		}
		#endregion
}
}

