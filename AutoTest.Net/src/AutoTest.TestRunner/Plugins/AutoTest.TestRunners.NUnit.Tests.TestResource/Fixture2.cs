using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AutoTest.TestRunners.NUnit.Tests.TestResource
{
    [TestFixture]
    public class Fixture2
    {
        [Test]
		[Category("notworking")]
        public void Should_also_pass()
        {
        }

        [Test]
		[Category("notworking")]
        public void Should_also_pass_again()
        {
        }
    }
}
