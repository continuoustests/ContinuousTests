using System;
using System.Collections.Generic;

namespace AutoTest.VM
{
    public static class IEnumerableExtensions
    {
        public static Dictionary<V, T> ToSafeDictionary<T, V>(this IEnumerable<T> list, Func<T, V> key)
        {
            var ret = new Dictionary<V, T>();
            foreach (var item in list)
            {
                var k = key(item);
                if (!ret.ContainsKey(k))
                    ret.Add(k, item);
            }
            return ret;
        }
    }
}