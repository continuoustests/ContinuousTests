using System;
using System.Collections.Generic;
using System.IO;
namespace AutoTest.Messages
{
	public class BuildRunResults : ICustomBinarySerializable
	{
		private string _project;
        private TimeSpan _timeSpent;
        private readonly List<BuildMessage> _errors = new List<BuildMessage>();
        private readonly List<BuildMessage> _warnings = new List<BuildMessage>();

        public string Project { get { return _project; } }
        public TimeSpan TimeSpent { get { return _timeSpent; } }
        public int ErrorCount { get { return _errors.Count; } }
        public int WarningCount { get { return _warnings.Count; } }
        public BuildMessage[] Errors { get { return _errors.ToArray(); } }
        public BuildMessage[] Warnings { get { return _warnings.ToArray(); } }

        public BuildRunResults(string project)
        {
            _project = project;
        }

        public void AddError(BuildMessage error)
        {
            _errors.Add(error);
        }

        public void AddWarning(BuildMessage warning)
        {
            _warnings.Add(warning);
        }

        public void SetTimeSpent(TimeSpan timeSpent)
        {
            _timeSpent = timeSpent;
        }

        public void UpdateProject(string project)
        {
            _project = project;
        }

		#region ICustomBinarySerializable implementation
		public void WriteDataTo(BinaryWriter writer)
		{
			writer.Write((string) _project);
			writer.Write((double) _timeSpent.Ticks);
			writer.Write((int) _errors.Count);
			foreach (var message in _errors)
				message.WriteDataTo(writer);
			writer.Write((int) _warnings.Count);
			foreach (var message in _warnings)
				message.WriteDataTo(writer);
		}

		public void SetDataFrom(BinaryReader reader)
		{
			_project = reader.ReadString();
			_timeSpent = new TimeSpan((long) reader.ReadDouble());
			var errors = reader.ReadInt32();
			for (int i = 0; i < errors; i++)
			{
				var message = new BuildMessage();
				message.SetDataFrom(reader);
				_errors.Add(message);
			}
			var warnings = reader.ReadInt32();
			for (int i = 0; i < warnings; i++)
			{
				var message = new BuildMessage();
				message.SetDataFrom(reader);
				_warnings.Add(message);
			}
		}
		#endregion
    }
}

