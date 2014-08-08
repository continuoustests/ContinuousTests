using System;
using Mono.Cecil;

namespace AutoTest.Minimizer.Extensions
{
    public static class TypeReferenceExtensions
    {
        public static TypeReference MakeGenericTypeInstance(this TypeReference type, params TypeReference[] arguments)
        {
            if (type.GenericParameters.Count != arguments.Length)
                throw new ArgumentException();

            var instance = new GenericInstanceType(type);
            foreach (var argument in arguments)
                instance.GenericArguments.Add(argument);

            return instance;
        }

        public static TypeDefinition ThreadSafeResolve(this TypeReference reference)
        {
            var def = reference as TypeDefinition;
            if (def != null) return def;
            if (reference == null) return null;
            lock(reference.Module)
            {
                try
                {
                    return reference.Resolve();
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
