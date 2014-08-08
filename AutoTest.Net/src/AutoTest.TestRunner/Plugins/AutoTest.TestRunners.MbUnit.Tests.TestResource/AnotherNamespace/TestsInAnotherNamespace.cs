using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace AutoTest.TestRunners.MbUnit.Tests.TestResource.AnotherNamespace
{
    public class TestsInAnotherNamespace
    {
        [Test]
        public void Even_another_test()
        {
            Console.WriteLine("Not using assert as it fails in mono");
        }
    }
}
