using System;
using System.Collections.Generic;
using AutoTest.Minimizer;

namespace TestHarness
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var files = new List<string>();
            files.AddRange(args);
            var x = new TestMinimizer(true, Environment.ProcessorCount);
            x.MinimizerMessage += x_MinimizerMessage;
            x.LoadOldCachedFiles(files);
            var tests = x.GetTestsFor(files);
            foreach (var y in tests)
            {
                Console.WriteLine("found test " + y.TestClass + "::" + y.TestName + " in assembly " + y.TestAssembly);
            }
            Console.WriteLine(tests.Count + " total tests found.");
            
            var graph = x.GetGraphFor("System.Void Fohjin.DDD.CommandHandlers.AssignNewBankCardCommandHandler::Execute(Fohjin.DDD.Commands.AssignNewBankCardCommand)", false);
        }

        private static void x_MinimizerMessage(object sender, MessageArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

