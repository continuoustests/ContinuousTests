using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace AutoTest.Minimizer
{
    public class CouplingCacheNode
    {
        public readonly string From;
        public readonly List<Coupling> Couplings = new List<Coupling>();
        public readonly MemberReference MemberReference;
        public int Complexity;

        public CouplingCacheNode(string @from, MemberReference memberReference)
        {
            From = from;
            MemberReference = memberReference;
        }

        public void AddCoupling(IEnumerable<Coupling> to)
        {
            foreach(var x in to) AddCoupling(to);
        }

        public void AddCoupling(Coupling to)
        {
            if (Couplings.Any(x => x.To == to.To && ((x.FieldReference == null && to.FieldReference == null) || ((x.FieldReference != null && to.FieldReference != null) && (x.FieldReference.FullName == to.FieldReference.FullName)))))
            {
                return;
            }
            Couplings.Add(to);
        }
    }

    public class Coupling
    {
        public readonly string To;
        public readonly bool IsReadOnly;
        public readonly bool IgnoreWalk;
        public readonly bool IsSelfCall;
        public readonly FieldReference FieldReference;
        public readonly MemberReference ActualReference;

        public Coupling(string to, bool isReadOnly, bool ignoreWalk, bool isSelfCall, FieldReference fieldReference, MemberReference actualReference)
        {
            To = to;
            FieldReference = fieldReference;
            IsSelfCall = isSelfCall;
            IsReadOnly = isReadOnly;
            IgnoreWalk = ignoreWalk;
            ActualReference = actualReference;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Coupling)) return false;
            return Equals((Coupling) obj);
        }

        public bool Equals(Coupling other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.To, To) && Equals(other.ActualReference, ActualReference);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((To != null ? To.GetHashCode() : 0)*397) ^ (ActualReference != null ? ActualReference.GetHashCode() : 0);
            }
        }
    }
}