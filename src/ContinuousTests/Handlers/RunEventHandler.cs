using System;
using System.Dynamic;
using System.Collections.Generic;
using AutoTest.Messages;
using AutoTest.Core.Messaging;
using AutoTest.Core.Messaging.MessageConsumers;

namespace ContinuousTests.Handlers
{
	class RunEventHandler : Handler
	{
        public override void OnMessage(object message) {
            if (isType<RunStartedMessage>(message))
                _dispatch("run-started", new { files = ((RunStartedMessage)message).Files });
            if (isType<RunFinishedMessage>(message))
                _dispatch("run-finished", null);
        }
	}
}
