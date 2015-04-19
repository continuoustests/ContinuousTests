using System;
using AutoTest.Messages;

namespace ContinuousTests.Handlers
{
    class InformationEventsHandler : Handler
    {
        public override void OnMessage(object message) {
            if (isType<InformationMessage>(message)) {
                _dispatch("text-info", new { text = ((InformationMessage)message).Message });
            }
            if (isType<WarningMessage>(message)) {
                _dispatch("text-warning", new { text = ((WarningMessage)message).Warning });
            }
            if (isType<ErrorMessage>(message)) {
                _dispatch("text-error", new { text = ((ErrorMessage)message).Error });
            }
        }
    }
}
