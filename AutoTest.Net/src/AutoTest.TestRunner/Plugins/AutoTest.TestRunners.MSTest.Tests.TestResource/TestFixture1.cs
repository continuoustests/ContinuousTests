using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace AutoTest.TestRunners.MSTest.Tests.TestResource
{
    [TestClass]
    public class TestFixture1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void Passing_test()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Failing_test()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void Inconclusive_test()
        {
            Thread.Sleep(10);
            Assert.Inconclusive("test was inconclusive");
        }

        [Ignore]
        [TestMethod]
        public void Ignore_Attrib_test()
        {
            Assert.Fail();
        }
    }
}
