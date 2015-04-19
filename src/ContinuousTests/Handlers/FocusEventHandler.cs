using System;
using AutoTest.Messages;
using BellyRub.UI;

namespace ContinuousTests.Handlers
{
	class FocusEventHandler : Handler
	{
        private Browser _browser;

        public FocusEventHandler(Browser browser) {
            _browser = browser;
        }

        public override void OnMessage(object message) {
            if (isType<ExternalCommandMessage>(message)) {
                var commandMessage = (ExternalCommandMessage)message;
                if (commandMessage.Sender == "EditorEngine") {
                    var msg = EditorEngineMessage.New(commandMessage.Sender + " " + commandMessage.Command);
                    if (msg.Arguments.Count == 2 &&
                        msg.Arguments[0].ToLower() == "autotest.net" &&
                        msg.Arguments[1].ToLower() == "setfocus")
                    {
                        _browser.BringToFront();
                    }
                }
            }
        }
	}
}
