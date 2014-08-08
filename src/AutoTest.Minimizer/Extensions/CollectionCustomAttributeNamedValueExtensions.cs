using Mono.Cecil;
using Mono.Collections.Generic;

namespace AutoTest.Minimizer.Extensions
{
    public static class CollectionCustomAttributeNamedValueExtensions
    {
        public static T FindByName<T>(this Collection<CustomAttributeNamedArgument> collection, string name)
        {
            foreach (var cur in collection)
            {
                if (cur.Name == name)
                {
                    return (T)cur.Argument.Value;
                }
            }
            return default(T);
        }
    }
}