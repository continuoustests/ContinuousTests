using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MMProfiler.Tests
{
    [TestClass]
    public class GenericTests : ProfilerTestBase
    {
        [TestInitialize]
        override public void TestInitialize()
        {
            base.TestInitialize();
            IncludeFilter = "Test.Generics.Sample";
        }

        [TestMethod]
        public void GenericTestsBaseClass_Ctor_Signature()
        {
            TestTarget = "Test.Target.GenericTests.BaseClassCtor_Test";

            ExecuteTestRunner();

            Assert.AreEqual(2, ResultsInfos.Count);
            Assert.AreEqual("Test.Generics.Sample.BaseClass`1<System.Int32>::.ctor", ResultsInfos[0].RuntimeData);
            Assert.AreEqual("System.Void Test.Generics.Sample.BaseClass`1<T>::.ctor()", ResultsInfos[0].MetaData);
            Assert.AreEqual("Test.Generics.Sample.BaseClass`1<System.Double>::.ctor", ResultsInfos[1].RuntimeData);
            Assert.AreEqual("System.Void Test.Generics.Sample.BaseClass`1<T>::.ctor()", ResultsInfos[1].MetaData);
        }

        [TestMethod]
        public void GenericTestsBaseClass_Ctor_Reference_Signature()
        {
            TestTarget = "Test.Target.GenericTests.BaseClassCtor_ReferenceTyped_Test";

            ExecuteTestRunner();

            Assert.AreEqual(2, ResultsInfos.Count);
            Assert.AreEqual("Test.Generics.Sample.BaseClass`1<System.String>::.ctor", ResultsInfos[0].RuntimeData);
            Assert.AreEqual("System.Void Test.Generics.Sample.BaseClass`1<T>::.ctor()", ResultsInfos[0].MetaData);
            Assert.AreEqual("Test.Generics.Sample.BaseClass`1<Test.Target.GenericTests>::.ctor", ResultsInfos[1].RuntimeData);
            Assert.AreEqual("System.Void Test.Generics.Sample.BaseClass`1<T>::.ctor()", ResultsInfos[1].MetaData);
        }

        [TestMethod]
        public void GenericTestsBaseClass_Ctor_TypedArray_Signature()
        {
            TestTarget = "Test.Target.GenericTests.BaseClassCtor_ArrayTyped_Test";

            ExecuteTestRunner();

            Assert.AreEqual(2, ResultsInfos.Count);
            Assert.AreEqual("Test.Generics.Sample.BaseClass`1<System.Double[]>::.ctor", ResultsInfos[0].RuntimeData);
            Assert.AreEqual("System.Void Test.Generics.Sample.BaseClass`1<T>::.ctor()", ResultsInfos[0].MetaData);
            Assert.AreEqual("Test.Generics.Sample.BaseClass`1<Test.Target.GenericTests[][][]>::.ctor", ResultsInfos[1].RuntimeData);
            Assert.AreEqual("System.Void Test.Generics.Sample.BaseClass`1<T>::.ctor()", ResultsInfos[1].MetaData);
        }
    }
}