namespace AutoTest.Core.Messaging.MessageConsumers
{
    public interface IGenerateBuildList
    {
        string[] Generate(string[] keys);
    }
}