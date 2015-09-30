using EnvDTE;

namespace AutoTest.VS.RiskClassifier
{
    public class Signature
    {
        public int LineNumber;
        public readonly string StringSignature;
        public readonly CodeElement Element;

        public Signature(int lineNumber, string stringSignature, CodeElement element)
        {
            StringSignature = stringSignature;
            Element = element;
            LineNumber = lineNumber;
        }
    }
}