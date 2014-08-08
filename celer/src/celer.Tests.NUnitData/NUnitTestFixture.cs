using System;
using NUnit.Framework;

namespace celer.Tests.NUnitData
{
    [MightyMooseIgnore]
    [TestFixture]
    public class NUnitTestFixture
    {
        [Test]
        public void test_with_no_assert() {}

        [Test]
        public void test_with_passing_assert()
        {
            Assert.That(1 == 1);
        }
        
        [ExpectedException(typeof(ArgumentException))]
        [Test]
        public void test_with_expected_exception()
        {
            throw new ArgumentException("test");
        }

        [Test]
        public void test_with_failing_assert()
        {
            Assert.That(1 == 0);
        }

        [Test]
        public void test_that_throws_unexpected_exception()
        {
            throw new ArgumentException("fubar");
        }
    }
}