using Microsoft.VisualStudio.Text.Editor;

namespace AutoTest.VS.RiskClassifier
{
    public class RiskTag : IGlyphTag
    {
        public readonly Signature Signature;

        public RiskTag(Signature signature)
        {
            Signature = signature;
        }
    }
}