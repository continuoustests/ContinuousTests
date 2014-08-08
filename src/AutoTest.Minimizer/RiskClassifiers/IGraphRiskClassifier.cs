using AutoTest.Graphs;

namespace AutoTest.Minimizer.RiskClassifiers
{
    public interface IGraphRiskClassifier
    {
        int CalculateRiskFor(AffectedGraph graph);
    }
}