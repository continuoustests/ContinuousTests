using Mono.Cecil;

namespace AutoTest.Minimizer.Extensions
{
    public static class CustomAttributeExtensions
    {
        public static bool IsInheritable(this CustomAttribute attribute)
        {
            var attrType = attribute.Constructor.DeclaringType.ThreadSafeResolve();
            return attrType == null ? false : attrType.IsInheritableAttribute();
        }
    }
}