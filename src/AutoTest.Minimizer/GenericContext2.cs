using System;
using System.Collections.Generic;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;

namespace AutoTest.Minimizer
{
    public class GenericContext2
    {
        public Dictionary<string, GenericEntry> GetGenericContextOf(MemberReference memberReference)
        {
            var ret = new Dictionary<string, GenericEntry>();
            GetMethodGenericArgumentsOn(memberReference, ret);
            GetClassGenericArgumentsOn(memberReference, ret);
            return ret;
        }

        private void GetClassGenericArgumentsOn(MemberReference memberReference, Dictionary<string, GenericEntry> ret)
        {
                if (memberReference == null) return;
                var instance = memberReference.DeclaringType as GenericInstanceType;
                if (instance != null)
                {
                    var resolved = instance.ThreadSafeResolve();
                    for (int i = 0; i < resolved.GenericParameters.Count; i++)
                    {
                        var current = resolved.GenericParameters[i];
                        var instanceParam = instance.GenericArguments[i];
                        if(instanceParam.IsGenericParameter)
                        {
                            ret.Add(current.Name, new GenericEntry(null, true, instanceParam.Name));
                        } else
                        {
                            ret.Add(current.Name, new GenericEntry(instanceParam.ThreadSafeResolve(), false, ""));
                        }
                    }
                }

        }

        public void GetMethodGenericArgumentsOn(MemberReference memberReference, Dictionary<string, GenericEntry> ret)
        {
            var genericInstanceMethod = memberReference as GenericInstanceMethod;
            
            if (genericInstanceMethod != null)
            {
                var elementMethod = genericInstanceMethod.ElementMethod;
                for (int i = 0; i < elementMethod.GenericParameters.Count; i++)
                {
                    var param = elementMethod.GenericParameters[i];
                    if (!genericInstanceMethod.GenericArguments[i].IsGenericParameter)
                    {
                        ret.Add(param.Name, new GenericEntry(genericInstanceMethod.GenericArguments[i].ThreadSafeResolve(), false, ""));
                    } else
                    {
                        ret.Add(param.Name, new GenericEntry(null, true, genericInstanceMethod.GenericArguments[i].Name));
                    }
                }
            }
        }
    }

    public class GenericEntry
    {
        public readonly TypeDefinition Type;
        public readonly bool IsGeneric;
        public readonly string Name;

        public GenericEntry(TypeDefinition type, bool isGeneric, string name)
        {
            Type = type;
            Name = name;
            IsGeneric = isGeneric;
        }
    }
}