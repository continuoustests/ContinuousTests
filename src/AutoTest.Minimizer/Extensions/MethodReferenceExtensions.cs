using System;
using System.Linq;
using Mono.Cecil;

namespace AutoTest.Minimizer.Extensions
{
    public static class MethodReferenceExtensions
    {
        public static MethodDefinition ThreadSafeResolve(this MethodReference reference)
        {
            var def = reference as MethodDefinition;
            if (def != null) return def;
            lock (reference.Module)
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
        public static MethodReference MakeReference(this MethodReference method, TypeReference declaringType)
        {
            var reference = new MethodReference(method.Name, method.ReturnType,declaringType)
                                {
                                    HasThis = method.HasThis,
                                    ExplicitThis = method.ExplicitThis,
                                    CallingConvention = MethodCallingConvention.Generic
                                };
            foreach (var parameter in method.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

            return reference;
        }

        public static bool HasAttribute(this MethodReference methodReference, string name)
        {
            var md = methodReference as MethodDefinition;
            if (md == null)
                return false;
            return
                md.CustomAttributes.Any(
                    attr => attr.Constructor.DeclaringType.ThreadSafeResolve().IfNotNull((x) => x.TryTypeNamed(name + "Attribute")));
        }

        public static string GetUniqueName(this MethodReference reference)
        {
            return reference.Module.Assembly.FullName + " " + reference.FullName;
        }
    }
}