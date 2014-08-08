using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace AutoTest.TestRunners.MbUnit.Tests.TestResource
{
    public class AnotherClassContainingTests
    {
        [Test]
        public void Some_passing_test()
        {
            Console.WriteLine("Not using assert as it fails in mono");
        }
    }
}
