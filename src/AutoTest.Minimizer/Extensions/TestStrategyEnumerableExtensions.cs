using System.Collections.Generic;
using System.Linq;
using AutoTest.Graphs;
using Mono.Cecil;

namespace AutoTest.Minimizer.Extensions
{
    public static class TestStrategyEnumerableExtensions
    {
        public static IEnumerable<string> TestRunnersFor(this IEnumerable<ITestIdentifier> list, MethodDefinition target)
        {
            return from cur in list where cur.IsTest(target) select cur.Tester;
        }

        public static IEnumerable<TestDescriptor> GetDescriptorsFor(this IEnumerable<ITestIdentifier> list, MemberReference definition)
        {
            foreach(var cur in list)
            {
                var desc = cur.GetTestDescriptorFor(definition);
                if(desc!=null)
                {
                    yield return desc;
                }
            }
        }

        public static bool IsProfilerSetup(this IEnumerable<ITestIdentifier> list, MethodReference target)
        {
            return list.Any(ider => ider.IsProfilerSetup(target));
        }


        public static bool IsProfilerTeardown(this IEnumerable<ITestIdentifier> list, MethodReference target)
        {
            return list.Any(ider => ider.IsProfilerTeardown(target));
        }

        public static bool IsProfilerTest(this IEnumerable<ITestIdentifier> list, MemberReference target)
        {
            return list.Any(ider => ider.IsProfilerTest(target));
        }

        public static bool IsSetup(this IEnumerable<ITestIdentifier> list, MethodReference target)
        {
            return list.Any(ider => ider.IsSetup(target));
        }


        public static bool IsTeardown(this IEnumerable<ITestIdentifier> list, MethodReference target)
        {
            return list.Any(ider => ider.IsTeardown(target));
        }

        public static bool IsTest(this IEnumerable<ITestIdentifier> list, MemberReference target)
        {
            return list.Any(ider => ider.IsTest(target));
        }


        public static string GetSpecificallyMangledName(this IEnumerable<ITestIdentifier> list, MethodReference target)
        {
            return list.Select(l => l.GetSpecificallyMangledName(target)).FirstOrDefault(ret => ret != null);
        }

        public static bool IsInFixture(this IEnumerable<ITestIdentifier> list, MethodReference target)
        {
            return list.Any(ider => ider.IsInFixture(target));
        }
    }
}