using System;

namespace AutoTest.Minimizer.Extensions
{
    public static class ObjectExtensions
    {
        public static T IfNotNull<T, U>(this U item, Func<U, T> lambda) where U : class
        {
            if (item == null)
            {
                return default(T);
            }
            return lambda(item);
        }
    }

}
