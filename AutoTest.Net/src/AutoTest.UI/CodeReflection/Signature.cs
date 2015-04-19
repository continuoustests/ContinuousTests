namespace AutoTest.UI.CodeReflection
{
    public class Signature
    {
        public SignatureTypes Type { get; private set; }
        public string Name { get; private set; }

        public Signature(SignatureTypes type, string signature)
        {
            Type = type;
            Name = signature;
        }
    }
}