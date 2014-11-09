using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MMProfiler.Tests
{
    [TestClass]
    public class BasicTests : ProfilerTestBase
    {
        [TestInitialize]
        override public void TestInitialize()
        {
            base.TestInitialize();
            IncludeFilter = "Test.Basic.Sample";
        }

        [TestMethod]
        public void Ctor_Signature()
        {
            TestTarget = "Test.Target.BasicTests.CtorTest";

            ExecuteTestRunner();

            Assert.AreEqual(1, ResultsInfos.Count);
            Assert.AreEqual("Test.Basic.Sample.Target::.ctor", ResultsInfos[0].RuntimeData);
            Assert.AreEqual("System.Void Test.Basic.Sample.Target::.ctor()", ResultsInfos[0].MetaData);
        }

        [TestMethod]
        public void Ctor_Signature_With_Parameters()
        {
            TestTarget = "Test.Target.BasicTests.CtorWithParamsTest";

            ExecuteTestRunner();

            Assert.AreEqual(1, ResultsInfos.Count);
            Assert.AreEqual("Test.Basic.Sample.Target::.ctor", ResultsInfos[0].RuntimeData);
            Assert.AreEqual("System.Void Test.Basic.Sample.Target::.ctor(System.String, System.Int32, Test.Basic.Sample.Target[])", ResultsInfos[0].MetaData);
        }

        [TestMethod]
        public void DataProperty_Signature()
        {
            TestTarget = "Test.Target.BasicTests.PropertyTest";

            ExecuteTestRunner();

            Assert.AreEqual(3, ResultsInfos.Count);
            Assert.AreEqual("Test.Basic.Sample.Target::set_Data", ResultsInfos[1].RuntimeData);
            Assert.AreEqual("System.Void Test.Basic.Sample.Target::set_Data(System.String)", ResultsInfos[1].MetaData);
            Assert.AreEqual("Test.Basic.Sample.Target::get_Data", ResultsInfos[2].RuntimeData);
            Assert.AreEqual("System.String Test.Basic.Sample.Target::get_Data()", ResultsInfos[2].MetaData);
        }

        [TestMethod]
        public void StaticMethod_Signature()
        {
            TestTarget = "Test.Target.BasicTests.StaticMethodTest";

            ExecuteTestRunner();

            Assert.AreEqual(1, ResultsInfos.Count);
            Assert.AreEqual("Test.Basic.Sample.Target::StaticMethod", ResultsInfos[0].RuntimeData);
            Assert.AreEqual("System.Void Test.Basic.Sample.Target::StaticMethod()", ResultsInfos[0].MetaData);
        }

        [TestMethod]
        public void StaticMethod_WithParams_Signature()
        {
            TestTarget = "Test.Target.BasicTests.StaticMethodWithParamsTest";

            ExecuteTestRunner();

            Assert.AreEqual(1, ResultsInfos.Count);
            Assert.AreEqual("Test.Basic.Sample.Target::StaticMethod", ResultsInfos[0].RuntimeData);
            Assert.AreEqual("Test.Basic.Sample.Target Test.Basic.Sample.Target::StaticMethod(Test.Basic.Sample.Target)", ResultsInfos[0].MetaData);
        }

        [TestMethod]
        public void Method_Signature()
        {
            TestTarget = "Test.Target.BasicTests.MethodTest";

            ExecuteTestRunner();

            Assert.AreEqual(2, ResultsInfos.Count);
            Assert.AreEqual("Test.Basic.Sample.Target::Method", ResultsInfos[1].RuntimeData);
            Assert.AreEqual("System.Void Test.Basic.Sample.Target::Method()", ResultsInfos[1].MetaData);
        }

        [TestMethod]
        public void Method_WithParams_Signature()
        {
            TestTarget = "Test.Target.BasicTests.MethodWithParamsTest";

            ExecuteTestRunner();

            Assert.AreEqual(2, ResultsInfos.Count);
            Assert.AreEqual("Test.Basic.Sample.Target::Method", ResultsInfos[1].RuntimeData);
            Assert.AreEqual("System.Void Test.Basic.Sample.Target::Method(System.Double)", ResultsInfos[1].MetaData);
        }

        [TestMethod]
        public void ExtensionMethod_Signature()
        {
            TestTarget = "Test.Target.BasicTests.ExtensionTest";

            ExecuteTestRunner();

            Assert.AreEqual(2, ResultsInfos.Count);
            Assert.AreEqual("Test.Basic.Sample.TargetExtensions::Extension", ResultsInfos[1].RuntimeData);
            Assert.AreEqual("System.Void Test.Basic.Sample.TargetExtensions::Extension(Test.Basic.Sample.Target)", ResultsInfos[1].MetaData);
        }

        [TestMethod]
        public void ExtensionMethodWithParams_Signature()
        {
            TestTarget = "Test.Target.BasicTests.ExtensionWithParamsTest";

            ExecuteTestRunner();

            Assert.AreEqual(2, ResultsInfos.Count);
            Assert.AreEqual("Test.Basic.Sample.TargetExtensions::Extension", ResultsInfos[1].RuntimeData);
            Assert.AreEqual("System.Void Test.Basic.Sample.TargetExtensions::Extension(Test.Basic.Sample.Target, System.String)", ResultsInfos[1].MetaData);
        }

        [TestMethod]
        public void StaticMethod_WithArrayParam_Signature()
        {
            TestTarget = "Test.Target.BasicTests.StaticMethodWithArrayParamTest";

            ExecuteTestRunner();

            Assert.AreEqual(1, ResultsInfos.Count);
            Assert.AreEqual("Test.Basic.Sample.Target::StaticMethod", ResultsInfos[0].RuntimeData);
            Assert.AreEqual("System.Int32[] Test.Basic.Sample.Target::StaticMethod(System.String[])", ResultsInfos[0].MetaData);
        }

        [TestMethod]
        public void StaticMethod_WithRefParam_Signature()
        {
            TestTarget = "Test.Target.BasicTests.StaticMethodWithRefParamTest";

            ExecuteTestRunner();

            Assert.AreEqual(1, ResultsInfos.Count);
            Assert.AreEqual("Test.Basic.Sample.Target::StaticMethod", ResultsInfos[0].RuntimeData);
            Assert.AreEqual("System.String Test.Basic.Sample.Target::StaticMethod(System.Int32&)", ResultsInfos[0].MetaData);
        }

        [TestMethod]
        public void StaticMethod_WithOutParam_Signature()
        {
            TestTarget = "Test.Target.BasicTests.StaticMethodWithOutParamTest";

            ExecuteTestRunner();

            Assert.AreEqual(1, ResultsInfos.Count);
            Assert.AreEqual("Test.Basic.Sample.Target::StaticMethod", ResultsInfos[0].RuntimeData);
            Assert.AreEqual("System.Int32 Test.Basic.Sample.Target::StaticMethod(System.String&)", ResultsInfos[0].MetaData);
        }

    }
}
