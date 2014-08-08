using System.Collections.Generic;

namespace AutoTest.Minimizer.Extensions
{
    public static class ListExtensions
    {
        public static void AddNotExist<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
        
        public static void AddNotExistRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var type in items)
            {
                AddNotExist(list, type);
            }
        }
    }
}