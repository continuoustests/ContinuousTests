using System;

namespace AutoTest
{
    public static class ObjectExtensions
    {
        public static bool Is<T>(this object obj) {
            return obj.GetType() == typeof(T);
        }
    }
}