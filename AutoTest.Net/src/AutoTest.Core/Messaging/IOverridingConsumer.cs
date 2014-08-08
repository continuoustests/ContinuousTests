using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Messaging
{
    public interface IOverridingConsumer<TMessage> : IMessageConsumer
    {
        bool IsRunning { get; }
        void Consume(TMessage message);
        void Terminate();
    }
}
