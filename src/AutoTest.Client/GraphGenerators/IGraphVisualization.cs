using AutoTest.VM.Messages;

namespace AutoTest.Client.GraphGenerators
{
    public interface IGraphVisualization
    {
        void GenerateAndShowGraphFor(VisualGraphGeneratedMessage message);
        bool WantsRefresh();
        string GetCurrentSignature();
    }
}