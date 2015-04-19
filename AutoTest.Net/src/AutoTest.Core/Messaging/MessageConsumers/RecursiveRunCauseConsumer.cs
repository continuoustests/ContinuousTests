using System;
using System.Collections.Generic;
using AutoTest.Messages;

namespace AutoTest.Core.Messaging.MessageConsumers
{
	public class RecursiveRunCauseConsumer : IConsumerOf<FileChangeMessage>
    {
        private readonly List<string> _changedFiles = new List<string>();

        public string[] Files { get { return _changedFiles.ToArray(); } }

        public void Consume(FileChangeMessage message)
        {
            foreach (var file in message.Files)
                _changedFiles.Add(file.FullName);
        }
	}
}