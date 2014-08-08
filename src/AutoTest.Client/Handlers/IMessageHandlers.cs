using AutoTest.Messages;
using AutoTest.UI;

namespace AutoTest.Client.Handlers
{
    public interface IMessageHandlers
    {
        void Consume(object message);
    }
}
