using System;
using System.Collections.Generic;
using System.Linq;
using celer.Core.TestRunners;
using System.Reflection;

namespace celer.Core.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsTestFixture(this Type type)
        {
            return type.GetCustomAttributes(true).Any(a => a.GetType().ToString() == "NUnit.Framework.TestFixtureAttribute");
        }

        public static List<DeploymentItem> GetDeploymentItems(this Type type)
        {
            return type.GetCustomAttributes(true)
                .Where(a => a.GetType().ToString() == "Microsoft.VisualStudio.TestTools.UnitTesting.DeploymentItemAttribute")
                .Select(x => getDeploymentItem(x)).ToList();
        }

        private static DeploymentItem getDeploymentItem(object x)
        {
            return new DeploymentItem()
            {
                Path = ((string)x.GetType().InvokeMember("Path", BindingFlags.GetProperty, null, x, null)),
                OutputDirectory = ((string)x.GetType().InvokeMember("OutputDirectory", BindingFlags.GetProperty, null, x, null))
            };
        }
    }
}