using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MMProfiler.Tests
{
    [TestClass]
    public class ClassTests : ProfilerTestBase
    {
        [TestInitialize]
        override public void TestInitialize()
        {
            base.TestInitialize();
            IncludeFilter = "Test.Class.Sample";
        }

        [TestMethod]
        public void ClassWithBase_Ctor_Signature()
        {
            TestTarget = "Test.Target.ClassTests.ClassWithBase_Ctor_Test";

            ExecuteTestRunner();

            Assert.AreEqual(2, ResultsInfos.Count);
            Assert.AreEqual("Test.Class.Sample.ClassWithBase::.ctor", ResultsInfos[0].RuntimeData);
            Assert.AreEqual("System.Void Test.Class.Sample.ClassWithBase::.ctor()", ResultsInfos[0].MetaData);
            Assert.AreEqual("Test.Class.Sample.BaseClass::.ctor", ResultsInfos[1].RuntimeData);
            Assert.AreEqual("System.Void Test.Class.Sample.BaseClass::.ctor()", ResultsInfos[1].MetaData);
        }

        [TestMethod]
        public void ClassWithBase_CallBaseMethod_Signature()
        {
            TestTarget = "Test.Target.ClassTests.ClassWithBase_CallBaseMethod_Test";

            ExecuteTestRunner();

            Assert.AreEqual(3, ResultsInfos.Count);
            Assert.AreEqual("Test.Class.Sample.BaseClass::MethodInBase", ResultsInfos[2].RuntimeData);
            Assert.AreEqual("System.Void Test.Class.Sample.BaseClass::MethodInBase()", ResultsInfos[2].MetaData);
        }

        [TestMethod]
        public void ClassWithAbstractBase_Ctor_Signature()
        {
            TestTarget = "Test.Target.ClassTests.ClassWithAbstractBase_Ctor_Test";

            ExecuteTestRunner();

            Assert.AreEqual(2, ResultsInfos.Count);
            Assert.AreEqual("Test.Class.Sample.ClassWithAbstractBase::.ctor", ResultsInfos[0].RuntimeData);
            Assert.AreEqual("System.Void Test.Class.Sample.ClassWithAbstractBase::.ctor()", ResultsInfos[0].MetaData);
            Assert.AreEqual("Test.Class.Sample.AbstractClass::.ctor", ResultsInfos[1].RuntimeData);
            Assert.AreEqual("System.Void Test.Class.Sample.AbstractClass::.ctor()", ResultsInfos[1].MetaData);
        }

        [TestMethod]
        public void ClassWithAbstractBase_CallBaseMethod_Signature()
        {
            TestTarget = "Test.Target.ClassTests.ClassWithAbstractBase_CallBaseMethod_Test";

            ExecuteTestRunner();

            Assert.AreEqual(3, ResultsInfos.Count);
            Assert.AreEqual("Test.Class.Sample.AbstractClass::MethodInAbstract", ResultsInfos[2].RuntimeData);
            Assert.AreEqual("System.Void Test.Class.Sample.AbstractClass::MethodInAbstract()", ResultsInfos[2].MetaData);
        }
    
    }
}