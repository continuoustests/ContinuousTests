using System;
using System.IO;
using System.Reflection;
namespace AutoTest.Messages
{
	public enum InformationType
    {
        Build = 1,
        TestRun = 2,
        PreProcessing = 3
    }

	[Serializable]
    public class RunInformationMessage : IMessage, ICustomBinarySerializable
    {
        public InformationType Type { get; private set; }
        public string Project { get; private set; }
        public string Assembly { get; private set; }
        public Type Runner { get; private set; }

        public RunInformationMessage(InformationType type, string project, string assembly, Type runner)
        {
            Type = type;
            Project = project;
            Assembly = assembly;
            Runner = runner;
        }
    
		#region ICustomBinarySerializable implementation
		public void WriteDataTo(BinaryWriter writer)
		{
			writer.Write((int) Type);
			writer.Write((string) Project);
			writer.Write((string) Assembly);
			writer.Write((string) Runner.FullName);
		}

		public void SetDataFrom(BinaryReader reader)
		{
			Type = (InformationType) reader.ReadInt32();
			Project = reader.ReadString();
			Assembly = reader.ReadString();
			Runner = System.Type.GetType(reader.ReadString());
		}
		#endregion
}
}

