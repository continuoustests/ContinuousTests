using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.TestRunners;
using System.IO;
using AutoTest.Core.Launchers;
using AutoTest.Core.Configuration;
using AutoTest.Messages;

namespace AutoTest.Core.Caching.RunResultCache
{
    public class TestItem : IItem
    {
        public string Key { get; private set; }
        public string Project { get; private set; }
        public TestResult Value { get; private set; }

        public TestItem(string key, string project, TestResult value)
        {
            Key = key;
            Project = project;
            Value = value;
        }

        public override bool  Equals(object obj)
        {
            return GetHashCode().Equals(obj.GetHashCode());
        }

        private int _hashCode = 0;
        public override int GetHashCode()
        {
            if (_hashCode != 0) return _hashCode;
            // Overflow is fine, just wrap
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Key == null ? 0 : Key.GetHashCode());
                hash = hash * 23 + (Value == null ? 0 : Value.GetHashCode());
                _hashCode = hash;
                return _hashCode;
            }
        }

        private string _toString = null;
        public override string ToString()
        {
            if (_toString != null) return _toString;
            var stackTrace = new StringBuilder();
            foreach (var line in Value.StackTrace)
            {
				DebugLog.Debug.WriteDetail("Stack line message " + line.File);
                if (File.Exists(line.File))
                {
                    stackTrace.AppendLine(string.Format("at {0} in {1}{2}:line {3}{4}",
                                                        line.Method,
                                                        LinkParser.TAG_START,
                                                        line.File,
                                                        line.LineNumber,
                                                        LinkParser.TAG_END));
                }
                else
                {
                    stackTrace.AppendLine(line.ToString());
                }

            }
            _toString = string.Format(
                "Assembly: {0}{4}" +
                "Test: {1}{4}" +
                "Message:{4}{2}{4}" +
                "Stack trace{4}{3}",
                Key,
                Value.DisplayName,
                Value.Message,
                stackTrace.ToString(),
			    Environment.NewLine);
            return _toString;
        }

        public bool IsTheSameTestAs(TestItem item)
        {
            if (item.Value == null)
                return false; // WTF!!?? == Fail
            return Key.Equals(item.Key) && Value.Runner.Equals(item.Value.Runner) && Value.Name.Equals(item.Value.Name) && Value.DisplayName.Equals(item.Value.DisplayName);
        }

        #region IItem Members


        public void HandleLink(string link)
        {
            var file = link.Substring(0, link.IndexOf(":line"));
            var lineNumber = getLineNumber(link);
            var launcher = BootStrapper.Services.Locate<IApplicatonLauncher>();
            launcher.LaunchEditor(file, lineNumber, 0);
        }

        private int getLineNumber(string link)
        {
            var start = link.IndexOf(":line");
            if (start < 0)
                return 0;
            start += ":line".Length;
            return int.Parse(link.Substring(start, link.Length - start));
        }

        #endregion
    }
}
