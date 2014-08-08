using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using System.Threading;

namespace AutoTest.TestRunners.MbUnitTests.Tests.TestResource
{
    public class ClassContainingTests
    {
        [Test]
        public void A_passing_test()
        {
            Thread.Sleep(10);
            Console.WriteLine("Not using assert as it fails in mono");
        }

        [Test]
        public void A_failing_test()
        {
            Console.WriteLine("Not using assert as it fails in mono");
            throw new Exception("failing test");
        }

        [Test]
        public void An_inconclusive_test()
        {
            Assert.Inconclusive();
        }
    }
}
