using AutoTest.VM.Messages;

namespace AutoTest.Client.SequenceDiagramGenerators
{
    public interface ISequenceDiagramVisualization
    {
        void GenerateAndShowDiagramFor(TestInformationGeneratedMessage message);
        string GetCurrentSignature();
        bool WantsRefresh();
    }
}
