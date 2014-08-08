using System;
using celer.Core;

namespace celer
{
    class Program
    {
        static void Main(string[] args)
        {
            StopWatch.PrintRunningTime(() =>
                                         {
                                             var runner = new CelerRunner();
                                             runner.RunTests(args);
                                         }
                );
        }
    }

    internal static class StopWatch
    {
        public static void PrintRunningTime(Action a)
        {
            var start = DateTime.Now;
            a();
            var end = DateTime.Now;
            Console.WriteLine("operation took: " + (end - start));
        }
    }
}
