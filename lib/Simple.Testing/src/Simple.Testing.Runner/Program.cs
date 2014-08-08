using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Simple.Testing.Framework;

namespace Simple.Testing.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            args.ForEach(x => new PrintFailuresOutputter().Output(SimpleRunner.RunAllInAssembly(x)));
        }

        
    }

    internal class PrintFailuresOutputter
    {
        public void Output(IEnumerable<RunResult> results)
        {
            int totalCount = 0;
            int totalAsserts = 0;
            int fail = 0;
            int failAsserts = 0;
            foreach (var result in results)
            {
                Console.WriteLine(Format(result));
                if (!result.Passed)
                {
                    failAsserts += result.Expectations.Where(x => x.Passed == false).Count();
                    fail++;
                }
                totalAsserts += result.Expectations.Count;
                totalCount++;
            }
            Console.WriteLine("\nRan {0} specifications {1} failures. {2} total assertions {3} failures.", totalCount, fail, totalAsserts, failAsserts);
        }

        private static string Format(RunResult result)
        {
            if (result.Passed) return "";
            var ret = (result.SpecificationName ?? result.FoundOnMemberInfo.Name) + " ";
            ret += (result.Passed ? "PASSED" : "FAILED") + " " + result.Message + "\n\n";
            if (result.Thrown != null)
                ret += result.Thrown + "\n\n";
            foreach (var exp in result.Expectations)
            {
                ret += "\t" + exp.Text + " " + (exp.Passed ? "PASSED" : "FAILED") + "\n";
                if (!exp.Passed)
                {
                    ret += exp.Exception.Message + "\n\n";
                }
            }
            ret += "\n---------------------------------------------------------------------------\n";
            return ret;
        }
    }
}
