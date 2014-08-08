using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests
{
    [TestFixture]
    public class IEnumerableResolverTests
    {
        [Test]
        public void can_parse_non_generated_name()
        {
            var name = "SomeClass::SomeMethod";
            var ret = IEnumerableResolver.GetOtherMethodNameFrom(name);
            Assert.AreEqual(name, ret);
        }

        [Test]
        public void can_find_original_method()
        {
            var name = "Test/<GetCounter>d__0";
            var ret = IEnumerableResolver.GetOtherMethodNameFrom(name);
            Assert.AreEqual("GetCounter", ret);
        }
    }
}
