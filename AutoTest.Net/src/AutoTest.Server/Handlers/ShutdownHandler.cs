using System;
using AutoTest.Messages;

namespace AutoTest.Server.Handlers
{
	class ShutdownHandler: IHandler, IInternalMessageHandler
    {
        private Action<string, object> _dispatch;

        public void DispatchThrough(Action<string, object> dispatcher) {
            _dispatch = dispatcher;
        }

        public void OnInternalMessage(object message) {
            if (message.Is<ExternalCommandMessage>()) {
                var commandMessage = (ExternalCommandMessage)message;
                if (commandMessage.Sender == "EditorEngine")
                {
                    var msg = EditorEngineMessage.New(commandMessage.Sender + " " + commandMessage.Command);
                    if (msg.Arguments.Count == 1 && msg.Arguments[0].ToLower() == "shutdown")
                        _dispatch("shutdown", null);
                }
            }
        }
	}
}