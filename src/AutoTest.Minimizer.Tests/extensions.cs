using System;
using System.Collections.Generic;
using System.Linq;
using AutoTest.Minimizer;
using Mono.Cecil;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests
{
    static class AssemblyReferenceExtension
    {
        public static TypeDefinition GetTypeDefinition<T>(this AssemblyDefinition assembly)
        {
            var type = assembly.MainModule.GetType(typeof(T).FullName);
            Assert.IsNotNull(type, "type " + typeof(T).FullName + " was null");
            return type;
        }

        public static TypeDefinition GetTypeDefinition(this AssemblyDefinition assembly, Type t)
        {
            var type = assembly.MainModule.GetType(t.FullName);
            Assert.IsNotNull(type, "type " + t.FullName + " was null");
            return type;
        }


        public static MethodDefinition GetMethodDefinition(this AssemblyDefinition assembly, Type t, string name)
        {
            var type = assembly.GetTypeDefinition(t);
            foreach (var method in type.Methods)
            {
                if (method.Name == name)
                    return method;
            }
            throw new MissingMethodException("Unable to find method: " + name);
        }

        public static MethodDefinition GetMethodDefinition<T>(this AssemblyDefinition assembly, string name)
        {
            var type = assembly.GetTypeDefinition<T>();
            foreach (var method in type.Methods)
            {
                if (method.Name == name)
                    return method;
            }
            throw new MissingMethodException("Unable to find method: " + name);
        }
    }

    public static class ListExtenstions
    {

        public static bool OnlyHasMember(this IList<MemberAccess> list, Type type, string method)
        {
            if (list.Count != 1) return false;
            return HasMember(list,type, method, null);
        }


        public static bool OnlyHasMember<T>(this IList<MemberAccess> list, string method)
        {
            if (list.Count != 1) return false;
            return HasMember<T>(list, method);
        }

        public static bool HasMember<T>(this IList<MemberAccess> list, string method)
        {
            return HasMember(list, typeof(T), method, null);
        }

        public static bool HasMember<T>(this IList<MemberAccess> list, string method, bool isReadonly)
        {
            return HasMember(list, typeof(T), method, isReadonly);
        }
        public static bool HasMember(this IList<MemberAccess> list, Type t, string method)
        {
            return HasMember(list, t, method, null);
        }

        public static bool HasMember(this IList<MemberAccess> list, Type t, string method, bool? isReadonly)
        {
            foreach (var m in list)
            {
                if (m.MemberReference.Name == method && m.MemberReference.DeclaringType.Name == t.Name && (!isReadonly.HasValue || m.IsReadOnly == (bool)isReadonly))
                {
                    var args = t.GetGenericArguments();
                    var cecilargs = m.MemberReference.DeclaringType.GenericParameters;
                    //TODO compare generic arguments
                    return true;
                }
            }
            return false;
        }

        public static bool OnlyHasMember(this IList<MethodReference> list, Type type, string method)
        {
            if (list.Count != 1) return false;
            return HasMember(list, type, method);
        }


        public static bool OnlyHasMember<T>(this IList<MethodReference> list, string method)
        {
            if (list.Count != 1) return false;
            return HasMember<T>(list, method);
        }

        public static bool HasMember<T>(this IList<MethodReference> list, string method)
        {
            return HasMember(list, typeof(T), method);
        }
        public static bool HasMember(this IList<MethodReference> list, Type t, string method)
        {
            foreach (var m in list)
            {
                if (m.Name == method && m.DeclaringType.FullName == t.FullName) return true;
            }
            return false;
        }

    }
    
    static class EnumerableExtensions
    {

		private static bool Has(IEnumerable<TypeReference> list, Type t)
		{
		    return list.Any(current => current.FullName == t.FullName);
		}

	    public static bool Has<T>(this IList<TypeReference> list)
        {
			return Has(list, typeof(T));
        }
    }
}

