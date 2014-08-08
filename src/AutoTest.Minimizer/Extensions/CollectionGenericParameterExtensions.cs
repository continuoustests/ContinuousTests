using Mono.Cecil;
using Mono.Collections.Generic;

namespace AutoTest.Minimizer.Extensions
{
    public static class CollectionGenericParameterExtensions
    {
        public static int GetIndexByName(this Collection<GenericParameter> collection, string name)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].FullName == name) return i;
            }
            return -1;
        }

    }
}