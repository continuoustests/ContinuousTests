using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AutoTest.TestRunners.MSTest.Extensions
{
    public static class AssemblyExtensions
    {
        public static List<Type> GetFixtures(this Assembly assembly)
        {
            return assembly.GetTypes().Where(type => type.IsTestFixture()).ToList();
        }
    }
}
