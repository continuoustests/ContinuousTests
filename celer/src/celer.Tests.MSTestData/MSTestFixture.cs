using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace celer.Tests.MSTestData
{
    [DeploymentItem("TestResourceFolder\\FixtureResource.txt")]
    [TestClass]
    public class MSTestFixture
    {
        [TestMethod]
        public void Simple_passing_test()
        {
            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void Simple_test_throwing_exception()
        {
            throw new Exception("Some exception");
        }

        [TestMethod]
        public void Simple_failing_test()
        {
            Assert.AreEqual(1, 10);
        }

        [TestMethod]
        public void Simple_inconclusive_test()
        {
            Assert.Inconclusive("inconclusive test");
        }
        
        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void Test_excpecting_exception()
        {
            var a = 10;
            var b = 0;
            Assert.AreEqual(1, a / b);
        }

        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void Test_excpecting_exception_but_not_getting_one()
        {
            var a = 10;
            var b = 0;
            Assert.AreEqual(1, a + b - 9);
        }

        [TestMethod]
        [DeploymentItem("TestResourceFolder\\SomeFile.txt")]
        public void Test_with_deployment_item()
        {
            Assert.IsTrue(File.Exists("SomeFile.txt"));
        }

        [TestMethod]
        [DeploymentItem("TestResourceFolder\\SomeFile.txt", "CustomOutput")]
        public void Test_with_deployment_item_with_custom_output()
        {
            Assert.IsTrue(File.Exists(Path.Combine("CustomOutput", "SomeFile.txt")));
        }
    }
}
