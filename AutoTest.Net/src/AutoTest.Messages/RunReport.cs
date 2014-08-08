using System;
using System.Collections.Generic;
using System.IO;
namespace AutoTest.Messages
{
	public class RunReport : ICustomBinarySerializable
	{
        private TimeSpan _timeSpent = new TimeSpan();
		private int _numberOfBuildsSucceeded = 0;
        private int _numberOfBuildsFailed = 0;
        private int _numberOfTestsPassed = 0;
        private int _numberOfTestsFailed = 0;
        private int _numberOfTestsIgnored = 0;
        private List<RunAction> _runActions = new List<RunAction>();

        public TimeSpan RealTimeSpent { get { return _timeSpent; } }
        public int NumberOfBuildsSucceeded { get { return _numberOfBuildsSucceeded; } }
        public int NumberOfBuildsFailed { get { return _numberOfBuildsFailed; } }
        public int NumberOfProjectsBuilt { get { return _numberOfBuildsSucceeded + _numberOfBuildsFailed; } }
        public int NumberOfTestsPassed { get { return _numberOfTestsPassed; } }
        public int NumberOfTestsFailed { get { return _numberOfTestsFailed; } }
        public int NumberOfTestsIgnored { get { return _numberOfTestsIgnored; } }
        public int NumberOfTestsRan { get { return _numberOfTestsPassed + _numberOfTestsFailed + _numberOfTestsIgnored; } }
        public bool Aborted { get; private set; }

        public RunAction[] RunActions { get { return _runActions.ToArray(); } }

        public void AddBuild(string project, TimeSpan timeSpent, bool succeeded)
        {
            _runActions.Add(new RunAction(InformationType.Build, project, timeSpent, succeeded));
            if (succeeded)
                _numberOfBuildsSucceeded++;
            else
                _numberOfBuildsFailed++;
        }

        public void AddTestRun(string project, string assembly, TimeSpan timeSpent, int passed, int ignored, int failed)
        {
            var succeeded = ignored == 0 && failed == 0;
            _runActions.Add(new RunAction(InformationType.TestRun, project, assembly, timeSpent, succeeded));
            _numberOfTestsPassed += passed;
            _numberOfTestsIgnored += ignored;
            _numberOfTestsFailed += failed;
        }

        public void SetTimeSpent(TimeSpan time)
        {
            _timeSpent = time;
        }

		#region ICustomBinarySerializable implementation
		public void WriteDataTo(BinaryWriter writer)
		{
            writer.Write(_timeSpent.TotalMilliseconds);
            writer.Write(Aborted);
			writer.Write((int) _numberOfBuildsSucceeded);
			writer.Write((int) _numberOfBuildsFailed);
			writer.Write((int) _numberOfTestsPassed);
			writer.Write((int) _numberOfTestsIgnored);
			writer.Write((int) _numberOfTestsFailed);
			writer.Write((int) _runActions.Count);
			foreach (var action in _runActions)
				action.WriteDataTo(writer);
		}

		public void SetDataFrom(BinaryReader reader)
		{
            _timeSpent = TimeSpan.FromMilliseconds(reader.ReadDouble());
            Aborted = reader.ReadBoolean();
			_numberOfBuildsSucceeded = reader.ReadInt32();
			_numberOfBuildsFailed = reader.ReadInt32();
			_numberOfTestsPassed = reader.ReadInt32();
			_numberOfTestsIgnored = reader.ReadInt32();
			_numberOfTestsFailed = reader.ReadInt32();
			var count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				var action = new RunAction(InformationType.TestRun, "", new TimeSpan(0), false);
				action.SetDataFrom(reader);
				_runActions.Add(action);
			}
		}
		#endregion

        public void WasAborted()
        {
            Aborted = true;
        }
    }
}

