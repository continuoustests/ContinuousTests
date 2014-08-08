using System;
using System.IO;
namespace AutoTest.Messages
{
	public class RunAction : ICustomBinarySerializable
	{
		private InformationType _type;
        private string _project;
        private string _assembly;
        private TimeSpan _timeSpent;
        private bool _succeeded;

        public InformationType Type { get { return _type; } }
        public string Project { get { return _project; } }
        public string Assembly { get { return _assembly; } }
        public TimeSpan TimeSpent { get { return _timeSpent; } }
        public bool Succeeded { get { return _succeeded; } }

        public RunAction(InformationType type, string project, TimeSpan timeSpent, bool succeeded)
        {
            setProperties(type, project, "", timeSpent, succeeded);
        }

        public RunAction(InformationType type, string project, string assembly, TimeSpan timeSpent, bool succeeded)
        {
            setProperties(type, project, assembly, timeSpent, succeeded);
        }

        public void UpdateProject(string project)
        {
            _project = project;
        }

        private void setProperties(InformationType type, string project, string assembly, TimeSpan timeSpent, bool succeeded)
        {
            _type = type;
            _project = project;
            _assembly = assembly;
            _timeSpent = timeSpent;
            _succeeded = succeeded;
        }

		#region ICustomBinarySerializable implementation
		public void WriteDataTo (BinaryWriter writer)
		{
			writer.Write((int) _type);
			writer.Write((string) _project);
			writer.Write((string) _assembly);
			writer.Write((double) _timeSpent.Ticks);
			writer.Write((bool) _succeeded);
		}

		public void SetDataFrom (BinaryReader reader)
		{
			_type = (InformationType) reader.ReadInt32();
			_project = reader.ReadString();
			_assembly = reader.ReadString();
			_timeSpent = new TimeSpan((long) reader.ReadDouble());
			_succeeded = reader.ReadBoolean();
		}
		#endregion
}
}

