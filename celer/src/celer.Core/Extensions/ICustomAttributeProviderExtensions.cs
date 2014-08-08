using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace celer.Core.Extensions
{
    public static class ICustomAttributeProviderExtensions
    {
        public static bool ContainsAttribute(this ICustomAttributeProvider provider, string name)
        {
            var attributes = provider.GetCustomAttributes(true);
            return attributes.Any(t => t.GetType().FullName == name);
        }
    }
}
