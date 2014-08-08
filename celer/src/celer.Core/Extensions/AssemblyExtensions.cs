using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace celer.Core.Extensions
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<ITestFixture> GetTestFixtures(this Assembly assembly)
        {
            return assembly.GetTypes().Where(current => current.IsTestFixture()).Select(current => new NUnitTestFixture(current)).Cast<ITestFixture>();
        }
    }
}