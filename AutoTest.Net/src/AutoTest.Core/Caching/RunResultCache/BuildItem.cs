using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.BuildRunners;
using System.IO;
using AutoTest.Core.Launchers;
using AutoTest.Core.Configuration;
using AutoTest.Messages;

namespace AutoTest.Core.Caching.RunResultCache
{
    public class BuildItem : IItem
    {
        public string Key { get; private set; }
        public BuildMessage Value { get; private set; }

        public BuildItem(string key, BuildMessage value)
        {
            Key = key;
            Value = value;
        }

        public override bool  Equals(object obj)
        {
            var other = (BuildItem) obj;
            return GetHashCode().Equals(other.GetHashCode());
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
            if (File.Exists(getFilePath()))
            {
                _toString = string.Format(
                    "Project: {0}{6}" +
                    "File: {4}{1}:line {2}{5}{6}" +
                    "Message:{6}{3}",
                    Key,
                    Value.File,
                    Value.LineNumber,
                    Value.ErrorMessage,
                    LinkParser.TAG_START,
                    LinkParser.TAG_END,
				    Environment.NewLine);
            }
            else
            {
                _toString = string.Format(
                    "Project: {0}{4}" +
                    "File: {1}:line {2}{4}" +
                    "Message:{4}{3}",
                    Key,
                    Value.File,
                    Value.LineNumber,
                    Value.ErrorMessage,
				    Environment.NewLine);
            }
            return _toString;
        }

        #region IItem Members


        public void HandleLink(string link)
        {
            var file = getFilePath();
            var launcher = BootStrapper.Services.Locate<IApplicatonLauncher>();
            launcher.LaunchEditor(file, Value.LineNumber, Value.LinePosition);
        }

        private string getFilePath()
        {
            return Path.Combine(Path.GetDirectoryName(Key), Value.File);
        }

        #endregion
    }
}
