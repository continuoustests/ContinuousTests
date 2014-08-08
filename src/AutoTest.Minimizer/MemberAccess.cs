using Mono.Cecil;

namespace AutoTest.Minimizer
{
    public class MemberAccess
    {
        public readonly MemberReference MemberReference;
        public readonly bool IsReadOnly;
        public readonly bool IsSelfCall;
        public readonly FieldReference OnField;
        public readonly MethodReference ActualMethodDefinition;
        public readonly bool IsLocalVariable;

        public MemberAccess(MemberReference memberReference, bool isReadOnly, bool isSelfCall, FieldReference onField, MethodReference actualMethodDefinition, bool isLocalVariable)
        {
            MemberReference = memberReference;
            IsLocalVariable = isLocalVariable;
            OnField = onField;
            IsSelfCall = isSelfCall;
            IsReadOnly = isReadOnly;
            ActualMethodDefinition = actualMethodDefinition;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (MemberAccess)) return false;
            return Equals((MemberAccess) obj);
        }

        public bool Equals(MemberAccess other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.MemberReference, MemberReference) && other.IsReadOnly.Equals(IsReadOnly) && other.IsSelfCall.Equals(IsSelfCall) && Equals(other.OnField, OnField);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (MemberReference != null ? MemberReference.GetHashCode() : 0);
                result = (result*397) ^ IsReadOnly.GetHashCode();
                result = (result*397) ^ IsSelfCall.GetHashCode();
                result = (result*397) ^ (OnField != null ? OnField.GetHashCode() : 0);
                return result;
            }
        }
    }
}