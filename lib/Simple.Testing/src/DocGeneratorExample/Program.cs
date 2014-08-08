using System;
using Simple.Testing.Framework;

namespace DocGeneratorExample
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleRunner.RunAllInAssembly(typeof(Program).Assembly).ForEach(PrintSpec);
        }

        private static void PrintSpec(RunResult result)
        {
            var passed = result.Passed ? "Passed" : "Failed";
            Console.WriteLine(result.Name.Replace('_', ' ') + " - " +passed);
            var on = result.GetOnResult();
                if(on != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("On:");
                    Console.WriteLine(on.ToString());
                    Console.WriteLine();
                }
                if (result.Result != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Results with:");
                    if(result.Result is Exception)
                        Console.WriteLine(result.Result.GetType() + "\n" + ((Exception) result.Result).Message );
                    else
                        Console.WriteLine(result.Result.ToString());
                    Console.WriteLine();
                }
            
            Console.WriteLine("Expectations:");
            foreach(var expecation in result.Expectations)
            {
                if(expecation.Passed)
                    Console.WriteLine("\t" + expecation.Text + " " + (expecation.Passed ? "Passed" : "Failed"));
                else
                    Console.WriteLine(expecation.Exception.Message);
            }
            if(result.Thrown != null)
            {
                Console.WriteLine("Specification failed: " + result.Message);
                Console.WriteLine();
                Console.WriteLine(result.Thrown);
            }
            Console.WriteLine(new string('-', 80));
            Console.WriteLine();
        }
    }
}
