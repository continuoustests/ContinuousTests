using System.Reflection;

namespace AutoTest.TestRunners.MSpec
{
    static class ObjectExtensions
    {
        public static T Run<T>(this object me, string methodName, object[] args)
        {
            return (T) me.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, me, args);
        }

        public static void Run(this object me, string methodName, object[] args)
        {
            me.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, me, args);
        }

        public static T Get<T>(this object me, string propertyName)
        {
            return (T) me.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, me, new object[] {});
        }
    }
}
