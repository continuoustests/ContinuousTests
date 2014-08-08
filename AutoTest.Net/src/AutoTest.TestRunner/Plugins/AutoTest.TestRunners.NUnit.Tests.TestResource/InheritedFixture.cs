using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AutoTest.TestRunners.NUnit.Tests.TestResource
{
    public class InheritedFixture : Fixture2
    {
        [Test]
        public void Should_pass()
        {
            Assert.IsTrue(true);
        }
    }
}
