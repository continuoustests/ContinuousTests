using System;
using AutoTest.Messages;

namespace ContinuousTests.Handlers
{
	class ShutdownEventHandler : Handler
    {
        public  override void OnMessage(object message) {
            if (isType<ExternalCommandMessage>(message)) {
                var commandMessage = (ExternalCommandMessage)message;
                if (commandMessage.Sender == "EditorEngine") {
                    var msg = EditorEngineMessage.New(commandMessage.Sender + " " + commandMessage.Command);
                    if (msg.Arguments.Count == 1 && msg.Arguments[0].ToLower() == "shutdown")
                        _dispatch("shutdown", null);
                }
            }
        }
	}
}
