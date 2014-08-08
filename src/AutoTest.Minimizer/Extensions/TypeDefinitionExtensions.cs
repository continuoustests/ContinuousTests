using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace AutoTest.Minimizer.Extensions
{
    public static class TypeDefinitionExtensions
    {
        public static void GetPublicsAndProtectedsFromType(this TypeDefinition type, List<MemberReference> ret)
        {
            foreach (var method in type.GetMethods())
            {
                if (method.IsPrivate) continue;
                ret.Add(method);
            }
            foreach (var field in type.Fields)
            {
                if (field.IsPrivate) continue;
                ret.Add(field);
            }

            foreach (var nested in type.NestedTypes)
            {
                GetPublicsAndProtectedsFromType(nested, ret);
            }
        }

        public static IEnumerable<MethodDefinition> GetMethods(this TypeDefinition type)
        {
            foreach (var method in type.Methods)
            {
                yield return method;
            }
        }

        public static bool ContainsEffectiveAttribute(this TypeDefinition definition, string name)
        {
            foreach(var attr in definition.GetEffectiveAttributes())
            {
                var def = attr.AttributeType.ThreadSafeResolve();
                if (def == null) continue;
                foreach(var attrtype in def.WalkInheritance())
                {
                    if (attrtype.Name == name) return true;
                }
            }
            return false;
        }

        public static IEnumerable<CustomAttribute> GetEffectiveAttributes(this TypeDefinition definition)
        {
            bool first = true;
            foreach(var t in definition.WalkInheritance())
            {
                var def = t.ThreadSafeResolve();
                if(def == null) continue;
                foreach(var a in def.CustomAttributes)
                {
                    if (first || a.IsInheritable())
                    {
                        yield return a;
                    }
                }
                first = false;
            }
        }
        
        public static bool IsInheritableAttribute(this TypeDefinition type)
        {
            foreach(var attr in type.CustomAttributes)
            {
                if(attr.Constructor.DeclaringType.FullName == "System.AttributeUsageAttribute" && 
                   attr.Properties.FindByName<bool>("Inherited"))
                {
                    return true;
                }
            }
            return false;
        }

        public static MethodDefinition GetMethodDefinitionMatching(this TypeDefinition type, MethodDefinition toMatch, bool allowNonVirtual)
        {
            foreach (var m in type.Methods)
            {
                if (m.MethodMatch(toMatch, allowNonVirtual)) return m;
            }
            return null;
        }

        public static TypeReference GetInInheritance(this TypeDefinition type, TypeReference baseType)
        {
            foreach(var x in WalkInheritance(type))
            {
                if(x.Name == baseType.Name)
                {
                    return x;
                }
            }
            throw new TypeNotFoundException("could not locate " + baseType.FullName + " in inheritance chain of " + type.FullName);
        }

        public static IEnumerable<MethodDefinition> GetGeneratedMethods(this TypeDefinition type)
        {
            foreach(var m in type.Methods)
            {
                if(m.IsGeneratedMethod())
                {
                    yield return m;
                }
            }
        }



        public static bool IsBaseTypeOf(this TypeDefinition self, TypeDefinition other)
        {
            if (self == null || other == null) return true;
            if (self.FullName == other.FullName) return true;
            foreach (var x in WalkInheritance(other))
            {
                var def = x.ThreadSafeResolve();
                if (def != null && def.FullName == self.FullName)
                {
                    return true;
                }
            }
            return false;
        }

        internal static IEnumerable<TypeReference> WalkInheritance(this TypeDefinition self)
        {
            TypeReference current = self;
            while (current != null)
            {
                yield return current;
                var def = current.ThreadSafeResolve();
                current = def == null ? null : def.BaseType;
            }
        }
        public static MethodDefinition TryMatchMethod(this TypeDefinition type, MethodDefinition method)
        {
            if (!type.HasMethods)
                return null;

            return type.Methods.FirstOrDefault(candidate => candidate.MethodMatch(method));
        }

        public static bool TryTypeNamed(this TypeDefinition attr, string s)
        {
            if (attr == null || attr.BaseType == null) return false;
            return attr.Name.EndsWith(s) || TryTypeNamed(attr.BaseType.ThreadSafeResolve(), s);
        }

        public static IEnumerable<MethodDefinition> GetMethodsInHierarchyWithAttribute(this TypeDefinition t, string attributeName)
        {
            var current = t;
            while (current != null)
            {
                foreach (var m in current.Methods)
                {
                    foreach (var a in m.CustomAttributes)
                    {
                        if (TryTypeNamed(a.Constructor.DeclaringType.ThreadSafeResolve(), attributeName))
                            yield return m;
                    }
                }
                current = current.BaseType != null ? current.BaseType.ThreadSafeResolve() : null;
            }
        } 

    }
}