using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace celer.Tests.MSTestData
{
    [TestClass]
    public class MSTestFixtureWithClassSetupAndTearDown
    {
        private static int _i = 0;
        private int _u = 0;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            _i += 10;
        }

        [ClassCleanup]
        public static void Teardown()
        {
            _i += 10;
        }

        [TestInitialize]
        public void Test_Setup()
        {
            _u += 20;
        }

        [TestCleanup]
        public void Test_Teardown()
        {
            _u += 20;
        }

        [TestMethod]
        public void class_init_teardown_test1()
        {
            Assert.AreEqual(10, _i);
        }

        [TestMethod]
        public void test_setup_teardown()
        {
            Assert.AreEqual(20, _u);
        }
    }

    [TestClass]
    public class MSTestFixtureWithFailingClassSetupAndTearDown
    {

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
        }

        [ClassCleanup]
        public static void Teardown()
        {
        }

        [TestInitialize]
        public void Test_Setup()
        {
            throw new Exception("some error");
        }

        [TestCleanup]
        public void Test_Teardown()
        {
        }

        [TestMethod]
        public void class_init_teardown_test1()
        {
            Assert.AreEqual(10, 10);
        }
    }
}
