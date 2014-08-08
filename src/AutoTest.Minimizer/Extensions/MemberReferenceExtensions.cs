using System;
using Mono.Cecil;

namespace AutoTest.Minimizer.Extensions
{
    public static class MemberReferenceExtensions
    {
        public static string GetCacheName(this MemberReference memberReference)
        {
            if (memberReference is MethodReference) {
                if (memberReference.DeclaringType != null && (memberReference.DeclaringType.IsGenericInstance))
				{
					var elementMethod = GetMatchingElementMethod(memberReference);
					if (elementMethod != null)
                    	return elementMethod.GetCacheName().Replace("/", "+");
				}
                var a = memberReference as MethodSpecification;
                if (a != null) return a.ElementMethod.FullName.Replace("/", "+");
            }
            try
            {
                var fieldReference = memberReference as FieldReference;
                if (fieldReference != null)
                {
                    if (fieldReference.DeclaringType != null && fieldReference.DeclaringType.IsGenericInstance)
                    {
                        return GetMatchingMember(fieldReference).GetCacheName().Replace("/", "+");
                    }
                }
            }catch(Exception ex) {}

            return memberReference.FullName;
        }

        private static string GetOpenTypeName(TypeReference typeReference)
        {
            return ((GenericInstanceType) typeReference).ElementType.FullName;
        }

        private static MemberReference GetMatchingMember(FieldReference fieldReference){
            var declaringType = (GenericInstanceType) fieldReference.DeclaringType;
            var def = declaringType.ElementType.ThreadSafeResolve();
            if (def == null) throw new ArgumentException("field not found on type");
            foreach (var current in def.Fields)
            {
                if (current.Name == fieldReference.Name)
                {
                    return current;
                }
            }
            throw new ArgumentException("field not found on type");
        }

        private static MemberReference GetMatchingElementMethod(MemberReference memberReference)
        {
            var methodReference = memberReference as MethodReference;
            var declaringType = (GenericInstanceType) memberReference.DeclaringType;
            var elementType = declaringType.ElementType.ThreadSafeResolve();
			if (elementType == null)
				return null;
            foreach (var current in elementType.Methods)
            {
                if(current.Name == memberReference.Name &&
                   current.Parameters.Count == methodReference.Parameters.Count)
                {
                    //TODO need to check out params in cases where there are many matches
                    //for (int index = 0; index < methodReference.Parameters.Count; index++)
                    //{
                    //    var method = methodReference.Parameters[index];
                    //    var element = current.Parameters[index];
                    //    var elementtype = element.GetTypeWithGenericResolve();
                    //    var methodtype = method.GetTypeWithGenericResolve();
                    //    if (elementtype == methodtype)
                    //    {
                    //        return current;
                    //    }
                    //}
                    return current;
                }
            }
            return null;
        }
    }
}