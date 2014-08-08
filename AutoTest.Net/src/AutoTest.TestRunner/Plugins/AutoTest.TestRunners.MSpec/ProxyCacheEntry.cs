using System;
using System.Collections.Generic;

namespace LinFu.DynamicProxy
{
    public struct ProxyCacheEntry
    {
        private readonly int hashCode;
        public Type BaseType;
        public Type[] Interfaces;

        public ProxyCacheEntry(Type baseType, Type[] interfaces)
        {
            if (baseType == null)
            {
                throw new ArgumentNullException("baseType");
            }
            BaseType = baseType;
            Interfaces = interfaces;

            if (interfaces == null || interfaces.Length == 0)
            {
                hashCode = baseType.GetHashCode();
                return;
            }

            // duplicated type exclusion
            Dictionary<Type, object> set = new Dictionary<Type, object>(interfaces.Length + 1);
            set[baseType] = null;
            foreach (Type type in interfaces)
            {
                if (type != null)
                    set[type] = null;
            }

            hashCode = 0;
            foreach (Type type in set.Keys)
            {
                hashCode ^= type.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ProxyCacheEntry))
                return false;

            ProxyCacheEntry other = (ProxyCacheEntry)obj;
            return hashCode == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
