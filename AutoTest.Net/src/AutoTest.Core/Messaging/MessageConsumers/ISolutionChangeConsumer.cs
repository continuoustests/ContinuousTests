using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    public interface ISolutionChangeConsumer
    {
        void Consume(ChangedFile file);
    }
}
