using System.Collections.Generic;
using AutoTest.Messages;
using System.IO;
using System;
using System.Text;

namespace AutoTest.Messages
{
    public class CacheMessages : IMessage
    {
        private List<CacheBuildMessage> _errorsToAdd = new List<CacheBuildMessage>();
        private List<CacheBuildMessage> _errorsToRemove = new List<CacheBuildMessage>();
        private List<CacheBuildMessage> _warningsToAdd = new List<CacheBuildMessage>();
        private List<CacheBuildMessage> _warningsToRemove = new List<CacheBuildMessage>();
        private List<CacheTestMessage> _failedToAdd = new List<CacheTestMessage>();
        private List<CacheTestMessage> _ignoredToAdd = new List<CacheTestMessage>();
        private List<CacheTestMessage> _testsToRemove = new List<CacheTestMessage>();

        public CacheBuildMessage[] ErrorsToAdd { get { return _errorsToAdd.ToArray(); } }
        public CacheBuildMessage[] ErrorsToRemove { get { return _errorsToRemove.ToArray(); } }
        
        public CacheBuildMessage[] WarningsToAdd { get { return _warningsToAdd.ToArray(); } }
        public CacheBuildMessage[] WarningsToRemove { get { return _warningsToRemove.ToArray(); } }
        
        public CacheTestMessage[] FailedToAdd { get { return _failedToAdd.ToArray(); } }
        public CacheTestMessage[] IgnoredToAdd { get { return _ignoredToAdd.ToArray(); } }
        public CacheTestMessage[] TestsToRemove { get { return _testsToRemove.ToArray(); } }

        public void AddError(CacheBuildMessage error) { _errorsToAdd.Add(error); }
        public void RemoveError(CacheBuildMessage error) { _errorsToRemove.Add(error); }
        public void AddWarning(CacheBuildMessage warning) { _warningsToAdd.Add(warning); }
        public void RemoveWarning(CacheBuildMessage warning) { _warningsToRemove.Add(warning); }
        public void AddFailed(CacheTestMessage failed) { _failedToAdd.Add(failed); }
        public void AddIgnored(CacheTestMessage ignored) { _ignoredToAdd.Add(ignored); }
        public void RemoveTest(CacheTestMessage ignored) { _testsToRemove.Add(ignored); }  
            
        public void SetDataFrom(BinaryReader reader)
        {
            _errorsToAdd = new List<CacheBuildMessage>();
            _errorsToRemove = new List<CacheBuildMessage>();
            _warningsToAdd = new List<CacheBuildMessage>();
            _warningsToRemove = new List<CacheBuildMessage>();
            _failedToAdd = new List<CacheTestMessage>();
            _ignoredToAdd = new List<CacheTestMessage>();
            _testsToRemove = new List<CacheTestMessage>();

            var errors = reader.ReadInt32();
            for (int i = 0; i < errors; i++)
            {
                var message = new CacheBuildMessage("", null);
                message.SetDataFrom(reader);
                _errorsToAdd.Add(message);
            }
            errors = reader.ReadInt32();
            for (int i = 0; i < errors; i++)
            {
                var message = new CacheBuildMessage("", null);
                message.SetDataFrom(reader);
                _errorsToRemove.Add(message);
            }

            var warnings = reader.ReadInt32();
            for (var i = 0; i < warnings; i++)
            {
                var message = new CacheBuildMessage("", null);
                message.SetDataFrom(reader);
                _warningsToAdd.Add(message);
            }
            warnings = reader.ReadInt32();
            for (var i = 0; i < warnings; i++)
            {
                var message = new CacheBuildMessage("", null);
                message.SetDataFrom(reader);
                _warningsToRemove.Add(message);
            }

            var failed = reader.ReadInt32();
            for (var i = 0; i < failed; i++)
            {
                var result = new CacheTestMessage("", null);
                result.SetDataFrom(reader);
                _failedToAdd.Add(result);
            }

            var ignored = reader.ReadInt32();
            for (var i = 0; i < ignored; i++)
            {
                var result = new CacheTestMessage("", null);
                result.SetDataFrom(reader);
                _ignoredToAdd.Add(result);
            }
            ignored = reader.ReadInt32();
            for (var i = 0; i < ignored; i++)
            {
                var result = new CacheTestMessage("", null);
                result.SetDataFrom(reader);
                _testsToRemove.Add(result);
            }
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(_errorsToAdd.Count);
            foreach (var message in _errorsToAdd)
                message.WriteDataTo(writer);
            writer.Write(_errorsToRemove.Count);
            foreach (var message in _errorsToRemove)
                message.WriteDataTo(writer);

            writer.Write(_warningsToAdd.Count);
            foreach (var message in _warningsToAdd)
                message.WriteDataTo(writer);
            writer.Write(_warningsToRemove.Count);
            foreach (var message in _warningsToRemove)
                message.WriteDataTo(writer);
            
            writer.Write(_failedToAdd.Count);
            foreach (var result in _failedToAdd)
                result.WriteDataTo(writer);
            writer.Write(_ignoredToAdd.Count);
            foreach (var result in _ignoredToAdd)
                result.WriteDataTo(writer);

            writer.Write(_testsToRemove.Count);
            foreach (var result in _testsToRemove)
                result.WriteDataTo(writer);
        }

        public override int GetHashCode()
        {
            // Overflow is fine, just wrap
            unchecked
            {
                int hash = 17;
                foreach (var message in _errorsToAdd)
                    hash = hash * 23 + message.GetHashCode();

                foreach (var message in _errorsToRemove)
                    hash = hash * 23 + message.GetHashCode();

                foreach (var message in _warningsToAdd)
                    hash = hash * 23 + message.GetHashCode();

                foreach (var message in _warningsToRemove)
                    hash = hash * 23 + message.GetHashCode();

                foreach (var message in _failedToAdd)
                    hash = hash * 23 + message.GetHashCode();

                foreach (var message in _ignoredToAdd)
                    hash = hash * 23 + message.GetHashCode();

                foreach (var message in _testsToRemove)
                    hash = hash * 23 + message.GetHashCode();

                return hash;
            }
        }
    }
}
