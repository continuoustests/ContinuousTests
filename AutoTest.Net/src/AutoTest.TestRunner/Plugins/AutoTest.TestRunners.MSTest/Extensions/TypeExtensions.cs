using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using AutoTest.TestRunners.Shared.Options;

namespace AutoTest.TestRunners.MSTest.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsTestFixture(this Type type)
        {
            if (type.IsAbstract)
                return false;
            if (type.GetCustomAttributes(true).Any(x => x.GetType().FullName.Equals("Microsoft.VisualStudio.TestTools.UnitTesting.IgnoreAttribute")))
                return false;
            return type.GetCustomAttributes(true).Any(x => x.GetType().FullName.Equals("Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute"));
        }

        public static IEnumerable<MethodInfo> GetTests(this Type type, string[] ignoreCategories)
        {
            if (type.IsAbstract)
                return new MethodInfo[] {};
            return type.GetMethods().Where(method => method.IsTest(ignoreCategories)).Select(test => test);
        }

        public static IEnumerable<MethodInfo> GetTestsMatching(this Type type, RunSettings settings)
        {
            if (type.IsAbstract)
                return new MethodInfo[] {};
            return type.GetMethods().Where(method => method.IsTest(settings.IgnoreCategories) && shouldRun(getTestName(type, method), settings)).Select(test => test);
        }

        private static string getTestName(Type type, MethodInfo method)
        {
            return type.FullName + "." + method.Name;
        }

        private static bool shouldRun(string methodName, RunSettings settings)
        {
            var assembly = settings.Assembly;
            if (assembly.Tests.Contains(methodName))
                return true;
            if (assembly.Members.Any(x => methodName.StartsWith(x)))
                return true;
            if (assembly.Namespaces.Any(x => methodName.StartsWith(x)))
                return true;
            return false;
        }
    }
}
