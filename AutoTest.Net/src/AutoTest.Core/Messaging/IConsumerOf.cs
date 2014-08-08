namespace AutoTest.Core.Messaging
{
    public interface IConsumerOf<TMesssage> : IMessageConsumer
    {
        void Consume(TMesssage message);
    }
}