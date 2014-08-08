using System;

namespace AutoTest.VM.Messages.Communication
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public readonly object Message;
        public MessageReceivedEventArgs(object message)
        {
            Message = message;
        }
    }
}
