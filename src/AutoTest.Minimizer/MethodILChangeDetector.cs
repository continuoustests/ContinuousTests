using System;
using System.Collections.Generic;
using System.Linq;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;

namespace AutoTest.Minimizer
{
    public class MethodILChangeDetector
    {
        public bool AreDifferentIL(MethodDefinition originalMethodDefinition, MethodDefinition newMethodDefinition)
        {
            if (!originalMethodDefinition.HasBody && !newMethodDefinition.HasBody) return false;
            if (!originalMethodDefinition.HasBody && newMethodDefinition.HasBody) return true;
            if (originalMethodDefinition.HasBody && !newMethodDefinition.HasBody) return true;
            if(AttributesAreDifferent(originalMethodDefinition, newMethodDefinition))
            {
                return true;
            }
            var originalBody = originalMethodDefinition.Body.Instructions;
            var newBody = newMethodDefinition.Body.Instructions;
            if (originalBody.Count != newBody.Count) return true;
            for(var i=0;i<originalBody.Count;i++)
            {
                if ((originalBody[i].GetSize() != newBody[i].GetSize() || !originalBody[i].ToString().Equals(newBody[i].ToString())) && !originalBody[i].ToString().Contains("<PrivateImplementationDetails>"))
                {
                    return true;
                }
            }
            return false;
        }
         
        private static bool AttributesAreDifferent(MethodDefinition originalMethodDefinition, MethodDefinition newMethodDefinition)
        {
            if(CheckProviderAttributes(originalMethodDefinition.DeclaringType.GetEffectiveAttributes(), newMethodDefinition.DeclaringType.GetEffectiveAttributes()))
            {
                return true;
            }
            if(CheckProviderAttributes(originalMethodDefinition.GetEffectiveAttributes(), newMethodDefinition.GetEffectiveAttributes()))
            {
                return true;
            }
            if (CheckProviderAttributes(originalMethodDefinition.MethodReturnType.GetEffectiveAttributes(), newMethodDefinition.MethodReturnType.GetEffectiveAttributes()))
            {
                return true;
            }
            if(originalMethodDefinition.Parameters.Count != newMethodDefinition.Parameters.Count) return true;
            for (int i = 0; i < originalMethodDefinition.Parameters.Count;i++ )
            {
                if (CheckProviderAttributes(originalMethodDefinition.Parameters[i].GetEffectiveAttributes(), newMethodDefinition.Parameters[i].GetEffectiveAttributes()))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CheckProviderAttributes(IEnumerable<CustomAttribute> originalMethodDefinition, IEnumerable<CustomAttribute> newMethodDefinition)
        {
            var originalSet = GetParameterMap(originalMethodDefinition);
            var newSet = GetParameterMap(newMethodDefinition);
            var changes = MapKeyDifferenceFinder.GetChangesBetween(originalSet, newSet);
            return changes.Count > 0;
        }

        private static Dictionary<string, string> GetParameterMap(IEnumerable<CustomAttribute> attributes)
        {
            var ret = new Dictionary<string, string>();
            foreach(var attribute in attributes)
            {
                var key = BuildKeyFor(attribute);
                var newkey = key;
                int count = 1;
                while(ret.ContainsKey(newkey))
                {
                    count++;
                    newkey = key + count;
                }
                ret.Add(newkey, attribute.AttributeType.FullName);
            }
            return ret;
        }

        private static string BuildKeyFor(CustomAttribute attribute)
        {
            if(attribute.AttributeType == null) throw new Exception("attribute type null");
            var ret = attribute.AttributeType.FullName;
            var args = "";
            if(!attribute.HasConstructorArguments)
            {
                args = "(";
            } else
            {
                args = attribute.ConstructorArguments.Aggregate("(",
                                                         (current, arg) =>
                                                         current + (arg.Value == null ? "NULL" : arg.Value.ToString()) +
                                                         ",");
            }
            if (args.Length > 1)
            {
                args = args.Substring(0, args.Length - 1);
            }
            return ret + args;
        }
    }
}