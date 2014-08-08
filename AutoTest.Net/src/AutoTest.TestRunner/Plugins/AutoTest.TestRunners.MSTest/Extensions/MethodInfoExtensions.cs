using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AutoTest.TestRunners.MSTest.Extensions
{
    public static class MethodInfoExtensions
    {
        public static bool IsTest(this MethodInfo method)
        {
            return method.GetCustomAttributes(true).Any(x => isTest(x));
        }

        public static bool IsTest(this MethodInfo method, string[] ignoreCategories)
        {
            if (!IsTest(method))
                return false;
            if (method.GetCustomAttributes(true).Any(x => isIgnoreAttribute(x)))
                return false;
            if (ignoreCategories.Length == 0)
                return true;;
            var categories = method.GetCustomAttributes(true)
                .Where(x => isTestCategoryAttribute(x))
                .Select(x => getTestCategories(x)).ToList();
            if (categories.Count == 0)
                return true;
            return categories.Any(x => doesNotContainIgnoreCategory(x, ignoreCategories));
        }

        private static bool isTest(object x)
        {
            return x.GetType().FullName.Equals("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute");
        }

        private static bool isTestCategoryAttribute(object x)
        {
            return x.GetType().FullName.Equals("Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute");
        }

        private static bool isIgnoreAttribute(object x)
        {
            return x.GetType().FullName.Equals("Microsoft.VisualStudio.TestTools.UnitTesting.IgnoreAttribute");
        }

        private static bool doesNotContainIgnoreCategory(string[] x, string[] ignoreCategories)
        {
            return !x.Any(category => ignoreCategories.Contains(category));
        }

        private static string[] getTestCategories(object x)
        {
            return ((IEnumerable<string>)x.GetType().InvokeMember("TestCategories", BindingFlags.GetProperty, null, x, null)).ToArray();
        }
    }
}
