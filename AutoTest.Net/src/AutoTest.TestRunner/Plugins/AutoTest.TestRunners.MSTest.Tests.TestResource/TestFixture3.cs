using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoTest.TestRunners.MSTest.Tests.TestResource
{
    [Ignore]
    [TestClass]
    public class TestFixture3
    {
        [TestMethod]
        public void SomeTest()
        {
            Assert.Fail();
        }
    }
}
