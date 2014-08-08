using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AutoTest.TestRunners.NUnit.Tests.TestResource
{
    [TestFixture]
    public class Fixture1
    {
        [Test]
		[Category("notworking")]
        public void Should_pass()
        {
            Assert.AreEqual(1, 1);
        }

        [TestCase]
		[Category("notworking")]
        public void Should_fail()
        {
            Assert.Fail("failing test");
        }

        [Test]
		[Category("notworking")]
        public void Should_ignore()
        {
            Assert.Ignore("ignored test");
        }

        [TestFixture]
        public class NestedFixture
        {
            [Test]
            public void Nested_test()
            {
                Assert.AreEqual(true, true);
            }
        }
    }
}
