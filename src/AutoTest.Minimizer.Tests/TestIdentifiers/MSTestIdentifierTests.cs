using System;
using AutoTest.Minimizer.TestIdentifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace AutoTest.Minimizer.Tests.TestIdentifiers
{
    [TestFixture]
    public class when_identifying_msunit_tests : AssemblyTestFixture
    {  
        private readonly MSTestTestIdentifier identifier = new MSTestTestIdentifier();
        
        [Test]
        public void can_identify_a_mstestunit_test()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixture>("a_test");
            Assert.IsTrue(identifier.IsTest(method));
        }

        [Test]
        public void does_not_identify_an_abstract_test()
        {
            var method = assembly.GetMethodDefinition<AbstractMSTestTestFixture>("abstract_test");
            Assert.IsFalse(identifier.IsTest(method));
        }

        [Test]
        public void does_not_identify_a_test_inabstract_class()
        {
            var method = assembly.GetMethodDefinition<AbstractMSTestTestFixture>("a_test");
            Assert.IsFalse(identifier.IsTest(method));
        }

        [Test]
        public void does_not_identify_a_non_test()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixture>("not_a_test");
            Assert.IsFalse(identifier.IsTest(method));
        }

        [Test]
        public void does_not_identify_setup_on_non_test()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixture>("not_a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.Count == 0);
        }


        [Test]
        public void identifies_class_initialize_on_same_class_for_test()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixtureWithClassInitializeCleanup>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(MSTestTestFixtureWithClassInitializeCleanup), "Foo"));
        }

        [Test]
        public void identifies_class_cleanup_on_same_class_for_test()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixtureWithClassInitializeCleanup>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(MSTestTestFixtureWithClassInitializeCleanup), "TearDown"));
        }


        [Test]
        public void identifies_setup_on_same_class_for_test()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(MSTestTestFixture), "Foo"));
        }

        [Test]
        public void identifies_teardown_on_same_class_for_test()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(MSTestTestFixture), "TearDown"));
        }

        [Test]
        public void identifies_setup_on_base_class_for_test()
        {
            var method = assembly.GetMethodDefinition<MSTestDerivedTestFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(MSTestBaseTestFixture), "Foo"));
        }


        [Test]
        public void identifies_teardown_on_base_class_for_test()
        {
            var method = assembly.GetMethodDefinition<MSTestDerivedTestFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(MSTestBaseTestFixture), "TearDown"));
        }


        [Test]
        public void identifies_class_initialize_on_base_class_for_test()
        {
            var method = assembly.GetMethodDefinition<MSTestDerivedTestFixtureWithInitializeAndCleanUp>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(MSTestBaseTestFixtureWithInitializeAndCleanUp), "Foo"));
        }

        [Test]
        public void identifies_class_cleanup_on_base_class_for_test()
        {
            int x = 6;
            var method = assembly.GetMethodDefinition<MSTestDerivedTestFixtureWithInitializeAndCleanUp>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(MSTestBaseTestFixtureWithInitializeAndCleanUp), "TearDown"));
        }

        [Test]
        public void identifies_multiple_setups_on_base_class_for_test()
        {
            var method = assembly.GetMethodDefinition<MSDerived>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.Count == 4);
            Assert.IsTrue(references.HasMember(typeof(MSBase1), "SetUp1"));
            Assert.IsTrue(references.HasMember(typeof(MSBase2), "SetUp2"));
        }


        [Test]
        public void identifies_multiple_teardowns_on_base_class_for_test()
        {
            var method = assembly.GetMethodDefinition<MSDerived>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.Count == 4);
            Assert.IsTrue(references.HasMember(typeof(MSBase1), "Teardown1"));
            Assert.IsTrue(references.HasMember(typeof(MSBase2), "Teardown2"));
        }


        [Test]
        public void returns_generic_instance_for_setup_on_generic_base()
        {
            var method = assembly.GetMethodDefinition<MSTestGenericDerivedFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(MSTestGenericBaseFixture<Foo>), "SetUp"));
        }

        [Test]
        public void returns_generic_instance_for_teardown_on_generic_base()
        {
            var method = assembly.GetMethodDefinition<MSTestGenericDerivedFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(MSTestGenericBaseFixture<Foo>), "TearDown"));
        }

        [Test]
        public void returns_generic_instance_for_setup_on_multi_level_generic_base()
        {
            var method = assembly.GetMethodDefinition<MSTestGenericMultiDerivedFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(MSTestGenericBaseFixture<Foo>), "SetUp"));
        }

        [Test]
        public void returns_generic_instance_for_teardown_on_multi_level_generic_base()
        {
            var method = assembly.GetMethodDefinition<MSTestGenericMultiDerivedFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(MSTestGenericBaseFixture<Foo>), "TearDown"));
        }

        [Test]
        public void can_identify_setup()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixture>("Foo");
            Assert.IsTrue(identifier.IsSetup(method));
        }

        [Test]
        public void can_identify_classcleanup()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixtureWithClassInitializeCleanup>("TearDown");
            Assert.IsTrue(identifier.IsTeardown(method));
        }

        [Test]
        public void does_not_identify_class_init_on_non_setup()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixtureWithClassInitializeCleanup>("TearDown");
            Assert.IsFalse(identifier.IsSetup(method));
        }

        [Test]
        public void does_not_identify_cleanup_on_non_cleanup()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixtureWithClassInitializeCleanup>("Foo");
            Assert.IsFalse(identifier.IsTeardown(method));
        }

        [Test]
        public void can_identify_classinitializer()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixtureWithClassInitializeCleanup>("Foo");
            Assert.IsTrue(identifier.IsSetup(method));
        }

        [Test]
        public void can_identify_teardown()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixture>("TearDown");
            Assert.IsTrue(identifier.IsTeardown(method));
        }

        [Test]
        public void does_not_identify_setup_on_non_setup()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixture>("TearDown");
            Assert.IsFalse(identifier.IsSetup(method));
        }

        [Test]
        public void does_not_identify_teardown_on_non_teardown()
        {
            var method = assembly.GetMethodDefinition<MSTestTestFixture>("Foo");
            Assert.IsFalse(identifier.IsTeardown(method));
        }
        //no derived attributes so no tests
    }

    public abstract class AbstractMSTestTestFixture
    {
        [TestMethod]
        public abstract void abstract_test();
        [TestMethod]
        public void a_test()
        {
            
        }
    }


    public class MSTestGenericMultiDerivedFixture : MSTestFirstDerivedFixture<Foo>
    {
        [TestMethod]
        public void a_test() { }
    }

    public class MSTestFirstDerivedFixture<T> : MSTestGenericBaseFixture<T>
    {
    }

    public class MSTestGenericDerivedFixture : MSTestGenericBaseFixture<Foo>
    {
        [TestMethod]
        public void a_test() { }
    }

    public class MSTestGenericBaseFixture<T>
    {
        [TestInitialize]
        public void SetUp()
        {

        }

        [TestCleanup]
        public void TearDown()
        {

        }
    }

    [TestClass]
    public class MSTestTestFixture
    {
        [TestInitialize]
        public void Foo()
        {

        }

        [TestCleanup]
        public void TearDown()
        {

        }

        [TestMethod]
        public void a_test()
        {

        }

        public void not_a_test()
        {

        }
    }


    [TestClass]
    public class MSTestTestFixtureWithClassInitializeCleanup
    {
        [ClassInitialize]
        public static void Foo(TestContext testContext)
        {

        }

        [ClassCleanup]
        public static void TearDown()
        {

        }

        [MightyMooseIgnore]
        [TestMethod]
        public void a_test()
        {

        }

        public void not_a_test()
        {

        }
    }

    public class MSTestDerivedTestFixture : MSTestBaseTestFixture
    {
        [TestMethod]
        public void a_test()
        {

        }
    }

    public class MSTestBaseTestFixture
    {
        [TestInitialize]
        public void Foo()
        {

        }

        [TestCleanup]
        public void TearDown()
        {

        }
    }

    public class MSTestDerivedTestFixtureWithInitializeAndCleanUp : MSTestBaseTestFixtureWithInitializeAndCleanUp
    {
        [TestMethod]
        public void a_test()
        {

        }
    }

    public class MSTestBaseTestFixtureWithInitializeAndCleanUp
    {
        [TestInitialize]
        public void Foo()
        {

        }

        [TestCleanup]
        public void TearDown()
        {

        }
    }

    public class MSBase1
    {
        [TestInitialize]
        public void SetUp1()
        {

        }

        [TestCleanup]
        public void Teardown1() { }
    }

    public class MSBase2 : MSBase1
    {
        [TestInitialize]
        public void SetUp2()
        {

        }
        [TestCleanup]
        public void Teardown2() { }
    }


    public class MSDerived : MSBase2
    {
        [TestMethod]
        public void a_test()
        {

        }
    }
}
