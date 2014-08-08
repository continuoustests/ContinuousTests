using System;
using System.Collections.Generic;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;

namespace AutoTest.Minimizer
{
    public class SynonymFinder
    {
        //TODO this should return MethodDefinition
        public static IList<MethodReference> FindSynonymsFor(MethodDefinition methodDefinition)
        {
            if(methodDefinition == null) throw new ArgumentNullException("methodDefinition");
            var baseSynonyms = GetBaseSynonyms(methodDefinition);
            baseSynonyms.AddRange(methodDefinition.Overrides);
            return baseSynonyms;
        }

        private static List<MethodReference> GetBaseSynonyms(MethodDefinition methodDefinition)
        {
            var type = methodDefinition.DeclaringType;
            var ret = new List<MethodReference>();
           
            var interfaceSynonym = GetBaseMethodInInterfaceHierarchy(type, methodDefinition);
            if (interfaceSynonym != null) ret.Add(interfaceSynonym);
            if (methodDefinition.IsNewSlot) return ret;
            var baseType = GetBaseType(type);
            while (baseType != null)
            {
                var synonym = baseType.TryMatchMethod(methodDefinition);
                if(synonym != null) ret.Add(synonym);
                interfaceSynonym = GetBaseMethodInInterfaceHierarchy(baseType, methodDefinition);
                if (interfaceSynonym != null) ret.Add(interfaceSynonym);
                baseType = GetBaseType(baseType);
            }
            return ret;
        }


        static MethodDefinition GetBaseMethodInInterfaceHierarchy(TypeDefinition type, MethodDefinition method)
        {
            if (!type.HasInterfaces)
                return null;

            foreach (var interfaceRef in type.Interfaces)
            {
                var @interface = interfaceRef.ThreadSafeResolve();
                if (@interface == null)
                    continue;

                var baseMethod = @interface.TryMatchMethod(method);
                if (baseMethod != null)
                    return baseMethod.ThreadSafeResolve();

                baseMethod = GetBaseMethodInInterfaceHierarchy(@interface, method);
                if (baseMethod != null)
                    return baseMethod.ThreadSafeResolve();
            }

            return null;
        }

        static TypeDefinition GetBaseType(TypeDefinition type)
        {
            if (type == null || type.BaseType == null)
                return null;

            return type.BaseType.ThreadSafeResolve();
        }
    }
}