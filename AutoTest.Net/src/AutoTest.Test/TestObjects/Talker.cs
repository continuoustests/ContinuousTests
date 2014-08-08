using AutoTest.Core.Messaging;

namespace AutoTest.Test.TestObjects
{
    internal class Talker
    {
        private readonly IMessageBus _bus;

        public Talker(IMessageBus bus)
        {
            _bus = bus;
        }

        public void Say(string msg)
        {
            _bus.Publish(new Message(msg));
        }
    }
}