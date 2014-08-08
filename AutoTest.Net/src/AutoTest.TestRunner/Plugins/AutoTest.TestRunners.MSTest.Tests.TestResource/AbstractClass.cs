using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoTest.TestRunners.MSTest.Tests.TestResource
{
    public class InheritingFromAbstractClass : AbstractClass
    {
    }

    [TestClass]
    public abstract class AbstractClass
    {
        [TestMethod]
        public void Test_on_abstract_class()
        {
            Assert.IsTrue(true);
        }
    }
}
