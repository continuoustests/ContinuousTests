using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace celer.Core.Extensions
{
    public class DeploymentItem
    {
        public string Path { get; set; }
        public string OutputDirectory { get; set; }
    }

    public static class MethodInfoExtensions
    {
        public static List<DeploymentItem> GetDeploymentItems(this MethodInfo method)
        {
            return method.GetCustomAttributes(true)
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
