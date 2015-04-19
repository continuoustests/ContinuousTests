using System;
using System.IO;
using System.Collections.Generic;

namespace AutoTest.Messages
{
	public class TestResult : ICustomBinarySerializable
	{
        private TestRunner _runner;
		private TestRunStatus _status;
        private string _name = "";
        private string _displayName = null;
        private string _message = "";
        private IStackLine[] _stackTrace;
        private static readonly TestResult _passResult;

        public TimeSpan TimeSpent = new TimeSpan();

        /// <summary>
        /// Factory method to return passed message
        /// </summary>
        /// <returns>Passed test result with no message</returns>
        public static TestResult Pass()
        {
            return _passResult;
        }

        /// <summary>
        /// Factory method to return failed result
        /// </summary>
        /// <param name="message">The failure message</param>
        /// <returns>A failed result</returns>
        public static TestResult Fail(string message)
        {
            return new TestResult(TestRunner.Any, TestRunStatus.Failed, message);
        }

        public TestResult(TestRunner runner, TestRunStatus status, string name)
        {
            _runner = runner;
            _name = name;
            _status = status;
            _stackTrace = new IStackLine[] { };
        }

        public TestResult(TestRunner runner, TestRunStatus status, string name, string message)
        {
            _runner = runner;
            _status = status;
            _name = name;
            _message = message;
            _stackTrace = new IStackLine[] { };
        }

        public TestResult(TestRunner runner, TestRunStatus status, string name, string message, IStackLine[] stackTrace)
        {
            _runner = runner;
            _status = status;
            _name = name;
            _message = message;
            _stackTrace = stackTrace;
        }

        public TestResult(TestRunner runner, TestRunStatus status, string name, string message, IStackLine[] stackTrace, double milliseconds)
        {
            _runner = runner;
            _status = status;
            _name = name;
            _message = message;
            _stackTrace = stackTrace;
            TimeSpent = TimeSpan.FromMilliseconds(milliseconds);
        }

        static TestResult()
        {
            _passResult = new TestResult(TestRunner.Any, TestRunStatus.Passed, string.Empty);
        }

        public TestRunner Runner
        {
            get { return _runner; }
        }

        public TestRunStatus Status
        {
            get { return _status; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string DisplayName
        {
            get
            {
                if (_displayName == null)
                    return _name;
                return _displayName;
            }
        }

        public string Message
        {
            get { return _message; } set { _message = value; }
        }

        public IStackLine[] StackTrace
        {
            get { return _stackTrace; } set { _stackTrace = value; }
        }

        public TestResult SetDisplayName(string name)
        {
            _displayName = name;
            return this;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(TestResult))
                return false;
            var other = (TestResult) obj;
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
        {
            // Overflow is fine, just wrap
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + _runner.GetHashCode();
                hash = hash * 23 + (_name == null ? 0 : _name.GetHashCode());
                hash = hash * 23 + (_message == null ? 0 : _message.GetHashCode());
                foreach (var line in _stackTrace)
                {
                    hash = hash * 23 + (line.File == null ? 0 : line.File.GetHashCode());
                    hash = hash * 23 + (line.Method == null ? 0 : line.Method.GetHashCode());
                    hash = hash * 23 + line.LineNumber.GetHashCode();
                }
                return hash;
            }
        }

		public void WriteDataTo(BinaryWriter writer)
		{
            writer.Write((int)_runner);
			writer.Write((int) _status);
            
            if (_name == null)
                writer.Write("");
            else
                writer.Write(_name);

            if (_displayName == null)
                writer.Write("");
            else
                writer.Write(_displayName);

            if (_message == null)
                writer.Write("");
            else
			    writer.Write((string) _message);

            writer.Write((double)TimeSpent.Ticks);
            writer.Write((int)_stackTrace.Length);
			foreach (var line in _stackTrace)
			{
                if (line.Method == null)
                    writer.Write("");
                else
				    writer.Write((string) line.Method);

                if (line.File == null)
                    writer.Write("");
                else
				    writer.Write((string) line.File);

				writer.Write((int) line.LineNumber);
			}
		}

		public void SetDataFrom(BinaryReader reader)
		{
			var stackTrace = new List<IStackLine>();
            _runner = (TestRunner)reader.ReadInt32();
			_status = (TestRunStatus) reader.ReadInt32();
			_name = reader.ReadString();
            _displayName = reader.ReadString();
            if (_displayName == "")
                _displayName = null;
			_message = reader.ReadString();
            TimeSpent = new TimeSpan((long)reader.ReadDouble());
			var count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				var method = reader.ReadString();
				var file = reader.ReadString();
				var lineNumber = reader.ReadInt32();
				var line = new StackLineMessage(method, file, lineNumber);
				stackTrace.Add(line);
			}
			_stackTrace = stackTrace.ToArray();
		}
    }
}

