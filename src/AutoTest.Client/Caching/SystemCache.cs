using System.Collections.Generic;
using AutoTest.Messages;

namespace AutoTest.Client.Caching
{
    class SystemCache
    {
        private List<IMessage> _messages;

        public IEnumerable<IMessage> Messages { get { return _messages; } }

        public SystemCache()
        {
            _messages = new List<IMessage>();
        }

        public void Add(IMessage message)
        {
            _messages.Add(message);
        }
    }
}
