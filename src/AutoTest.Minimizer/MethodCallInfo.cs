using Mono.Cecil;

namespace AutoTest.Minimizer
{
    internal class MethodCallInfo
    {
        public bool IsSelfCall;
        public FieldReference FieldReference;
        public bool IsLocal;

        public MethodCallInfo(bool isSelfCall, FieldReference fieldReference, bool isLocal)
        {
            IsSelfCall = isSelfCall;
            FieldReference = fieldReference;
            IsLocal = isLocal;
        }
    }
}