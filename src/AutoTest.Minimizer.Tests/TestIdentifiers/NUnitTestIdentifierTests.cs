using System;
using AutoTest.Minimizer.TestIdentifiers;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests.TestIdentifiers
{
    [TestFixture]
    public class when_identifying_nunit_tests : AssemblyTestFixture
    {
        private readonly NUnitTestIdentifier identifier = new NUnitTestIdentifier();

        [Test]
        public void can_identify_a_nunit_test()
        {
            var method = assembly.GetMethodDefinition<NUnitTestFixture>("a_test");
            Assert.IsTrue(identifier.IsTest(method));
        }
        [Test]
        public void does_not_identify_a_nunit_test_with_explicit()
        {
            var method = assembly.GetMethodDefinition<NUnitTestFixture>("a_test_with_explicit");
            Assert.IsFalse(identifier.IsTest(method));
        }

        [Test]
        public void does_not_identify_a_nunit_test_case_with_explicit()
        {
            var method = assembly.GetMethodDefinition<NUnitTestFixture>("a_test_case_with_explicit");
            Assert.IsFalse(identifier.IsTest(method));
        }
        [Test]
        public void can_identify_a_nunit_param_test()
        {
            var method = assembly.GetMethodDefinition<NUnitTestFixture>("a_test_case");
            Assert.IsTrue(identifier.IsTest(method));
        }

        [Test]
        public void can_identify_a_test_fixture_setup()
        {
            var method = assembly.GetMethodDefinition<NUnitTestFixtureSetupTeardown>("test_fixture_setup");
            Assert.IsTrue(identifier.IsSetup(method));
        }

        [Test]
        public void can_identify_a_test_fixture_teardown()
        {
            var method = assembly.GetMethodDefinition<NUnitTestFixtureSetupTeardown>("test_fixture_teardown");
            Assert.IsTrue(identifier.IsTeardown(method));
        }

        [Test]
        public void does_not_identify_a_non_test()
        {
            var method = assembly.GetMethodDefinition<NUnitTestFixture>("not_a_test");
            Assert.IsFalse(identifier.IsTest(method));            
        }

        [Test]
        public void does_not_identify_an_abstract_test()
        {
            var method = assembly.GetMethodDefinition<AbstractNUnitTestFixture>("abstract_test");
            Assert.IsFalse(identifier.IsTest(method));
        }

        [Test]
        public void does_not_identify_a_test_inabstract_class()
        {
            var method = assembly.GetMethodDefinition<AbstractNUnitTestFixture>("a_test");
            Assert.IsFalse(identifier.IsTest(method));
        }

        [Test]
        public void does_not_identify_setup_on_non_test()
        {
            var method = assembly.GetMethodDefinition<NUnitTestFixture>("not_a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.Count == 0);
        }

        [Test]
        public void identifies_setup_on_same_class_for_test()
        {
            var method = assembly.GetMethodDefinition<NUnitTestFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(NUnitTestFixture), "Foo"));
        }

        [Test]
        public void identifies_teardown_on_same_class_for_test()
        {
            var method = assembly.GetMethodDefinition<NUnitTestFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(NUnitTestFixture), "TearDown"));
        }


        [Test]
        public void identifies_derivedtestfixxture_setup_on_base_class_for_test()
        {
            var method = assembly.GetMethodDefinition<DerivedTestFixtureWithDerivedFixtureSetupTearDown>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(BaseTestFixtureWithDerivedFixtureSetupTearDown), "Foo"));
        }

        [Test]
        public void identifies_derivedtestfixture_teardown_on_base_class_for_test()
        {
            var method = assembly.GetMethodDefinition<DerivedTestFixtureWithFixtureSetupTearDown>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(BaseTestFixtureWithFixtureSetupTearDown), "TearDown"));
        }

        [Test]
        public void identifies_testfixxture_setup_on_base_class_for_test()
        {
            var method = assembly.GetMethodDefinition<DerivedTestFixtureWithFixtureSetupTearDown>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(BaseTestFixtureWithFixtureSetupTearDown), "Foo"));
        }

        [Test]
        public void identifies_testfixture_teardown_on_base_class_for_test()
        {
            var method = assembly.GetMethodDefinition<DerivedTestFixtureWithFixtureSetupTearDown>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(BaseTestFixtureWithFixtureSetupTearDown), "TearDown"));
        }


        [Test]
        public void identifies_testfixture_setup_for_test()
        {
            var method = assembly.GetMethodDefinition<TestFixtureWithFixtureSetupTearDown>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(TestFixtureWithFixtureSetupTearDown), "Foo"));
        }

        [Test]
        public void identifies_testfixture_teardown_for_test()
        {
            var method = assembly.GetMethodDefinition<TestFixtureWithFixtureSetupTearDown>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(TestFixtureWithFixtureSetupTearDown), "TearDown"));
        }

        [Test]
        public void identifies_derivedtestfixture_setup_for_test()
        {
            var method = assembly.GetMethodDefinition<TestFixtureWithDerivedFixtureSetupTearDown>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(TestFixtureWithDerivedFixtureSetupTearDown), "Foo"));
        }

        [Test]
        public void identifies_derivedtestfixture_teardown_for_test()
        {
            var method = assembly.GetMethodDefinition<TestFixtureWithDerivedFixtureSetupTearDown>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(TestFixtureWithDerivedFixtureSetupTearDown), "TearDown"));
        }

        [Test]
        public void identifies_testfixture_setup_on_base_class_for_test()
        {
            var method = assembly.GetMethodDefinition<DerivedTestFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(BaseTestFixture), "Foo"));
        }

        [Test]
        public void identifies_multiple_setups_on_base_class_for_test()
        {
            var method = assembly.GetMethodDefinition<Derived>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.Count == 4);
            Assert.IsTrue(references.HasMember(typeof(Base1), "SetUp1"));
            Assert.IsTrue(references.HasMember(typeof(Base2), "SetUp2"));
        }


        [Test]
        public void identifies_multiple_teardowns_on_base_class_for_test()
        {
            var method = assembly.GetMethodDefinition<Derived>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.Count == 4);
            Assert.IsTrue(references.HasMember(typeof(Base1), "Teardown1"));
            Assert.IsTrue(references.HasMember(typeof(Base2), "Teardown2"));
        }

        [Test]
        public void identifies_teardown_on_base_class_for_test()
        {
            var method = assembly.GetMethodDefinition<DerivedTestFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(BaseTestFixture), "TearDown"));
        }

        [Test]
        public void returns_generic_instance_for_setup_on_generic_base()
        {
            var method = assembly.GetMethodDefinition<GenericDerivedFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(GenericBaseFixture<Foo>), "SetUp"));
        }

        [Test]
        public void returns_generic_instance_for_teardown_on_generic_base()
        {
            var method = assembly.GetMethodDefinition<GenericDerivedFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(GenericBaseFixture<Foo>), "TearDown"));
        }



        [Test]
        public void returns_generic_instance_for_setup_on_multi_level_generic_base()
        {
            var method = assembly.GetMethodDefinition<GenericMultiDerivedFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(GenericBaseFixture<Foo>), "SetUp"));
        }

        [Test]
        public void returns_generic_instance_for_teardown_on_multi_level_generic_base()
        {
            var method = assembly.GetMethodDefinition<GenericMultiDerivedFixture>("a_test");
            var references = identifier.GetHiddenDependenciesForTest(method);
            Assert.IsTrue(references.HasMember(typeof(GenericBaseFixture<Foo>), "TearDown"));
        }

        [Test]
        public void can_identify_a_setup()
        {
            var method = assembly.GetMethodDefinition<NUnitTestFixture>("Foo");
            Assert.IsTrue(identifier.IsSetup(method));
        }

        [Test]
        public void can_identify_a_teardown()
        {
            var method = assembly.GetMethodDefinition<NUnitTestFixture>("TearDown");
            Assert.IsTrue(identifier.IsTeardown(method));
        }

        [Test]
        public void can_identify_a_setup_with_derived_attribute()
        {
            var method = assembly.GetMethodDefinition<DerivedAttrTestFixture>("Foo");
            Assert.IsTrue(identifier.IsSetup(method));
        }

        [Test]
        public void can_identify_a_teardown_with_derived_attribute()
        {
            var method = assembly.GetMethodDefinition<DerivedAttrTestFixture>("TearDown");
            Assert.IsTrue(identifier.IsTeardown(method));
        }


        [Test]
        public void does_not_identify_a_setup_without_attribute()
        {
            var method = assembly.GetMethodDefinition<NotATestFixture>("SetUp");
            Assert.IsFalse(identifier.IsSetup(method));
        }

        [Test]
        public void does_not_identify_a_teardown_without_attribute()
        {
            var method = assembly.GetMethodDefinition<NotATestFixture>("TearDown");
            Assert.IsFalse(identifier.IsTeardown(method));
        }

        [Test]
        public void can_identify_test_fixture_with_attribute_from_base()
        {
            var method = assembly.GetMethodDefinition<DerivedTestFixture>("a_test");
            Assert.IsTrue(identifier.IsInFixture(method));
        }

        [Test]
        public void can_identify_test_fixture_with_attribute_on_same()
        {
            var method = assembly.GetMethodDefinition<BaseTestFixture>("TearDown");
            Assert.IsTrue(identifier.IsInFixture(method));
        }


        [Test]
        public void can_identify_test_fixture_with_derived_attribute_on_same()
        {
            var method = assembly.GetMethodDefinition<DerivedAttrTestFixture>("Foo");
            Assert.IsTrue(identifier.IsInFixture(method));
        }

        [Test]
        public void can_identify_test_fixture_with_derived_attribute_on_base()
        {
            var method = assembly.GetMethodDefinition<DerivedTestFixture>("a_test");
            Assert.IsTrue(identifier.IsInFixture(method));
        }

        [Test]
        public void does_not_identify_test_fixture_without_attribute()
        {
            var method = assembly.GetMethodDefinition<NotATestFixture>("SetUp");
            Assert.IsFalse(identifier.IsInFixture(method));
        }
    }

    public class DerivedTestFixtureWithFixtureSetupTearDown : BaseTestFixtureWithFixtureSetupTearDown
    {
        [Test]
        public void a_test()
        {

        }
    }

    [TestFixture]
    public class TestFixtureWithFixtureSetupTearDown
    {
        [TestFixtureSetUp]
        public void Foo()
        {

        }

        [Test]
        public void a_test()
        {
            
        }

        [TestFixtureTearDown]
        public void TearDown()
        {

        }
    }

    [TestFixture]
    public class BaseTestFixtureWithFixtureSetupTearDown
    {
        [TestFixtureSetUp]
        public void Foo()
        {

        }

        [TestFixtureTearDown]
        public void TearDown()
        {

        }
    }

    public class DerivedTestFixtureWithDerivedFixtureSetupTearDown : BaseTestFixtureWithDerivedFixtureSetupTearDown
    {
        [Test]
        public void a_test()
        {

        }
    }

    [TestFixture]
    public class TestFixtureWithDerivedFixtureSetupTearDown
    {
        [DerivedFixtureSetUpAttribute]
        public void Foo()
        {

        }

        [Test]
        public void a_test()
        {

        }

        [DerivedFixtureTearDownAttribute]
        public void TearDown()
        {

        }
    }
    [TestFixture]
    public class BaseTestFixtureWithDerivedFixtureSetupTearDown
    {
        [DerivedFixtureSetUpAttribute]
        public void Foo()
        {

        }

        [DerivedFixtureTearDownAttribute]
        public void TearDown()
        {

        }
    }

    public abstract class AbstractNUnitTestFixture
    {
        [Test]
        public abstract void a_test();
        [Test]
        public void abstract_test()
        {
            
        }
    }

    public class NotATestFixture
    {
        [Test]
        public void SetUp() {}
        [Test]
        public void TearDown() {}
    }

    [DerivedFixture]
    public class DerivedAttrTestFixture
    {
        [DerivedSetUp]
        public void Foo()
        {
            
        }

        [DerivedTearDown]
        public void TearDown()
        {
            
        }
    }

    public class NUnitTestFixtureSetupTeardown
    {
        [TestFixtureSetUp]
        public void test_fixture_setup()
        {
            
        }

        [TestFixtureTearDown]
        public void test_fixture_teardown()
        {

        }
    }

    public class DerivedFixture : TestFixtureAttribute {}
    public class DerivedFixtureSetUpAttribute : TestFixtureSetUpAttribute{}
    public class DerivedFixtureTearDownAttribute : TestFixtureTearDownAttribute {}

    public class DerivedTearDownAttribute : TearDownAttribute
    {
    }

    public class DerivedSetUpAttribute : SetUpAttribute
    {
    }

    public class GenericMultiDerivedFixture : FirstDerivedFixture<Foo>
    {
        [Test]
        public void a_test() { }
    }

    public class FirstDerivedFixture<T> : GenericBaseFixture<T>
    {
    }

    public class GenericDerivedFixture : GenericBaseFixture<Foo>
    {
        [Test]
        public void a_test() {}
    }
    [DerivedFixture]
    public class GenericBaseFixture<T>
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }

    public class Foo {}

    [TestFixture]
    public class NUnitTestFixture
    {
        [SetUp]
        public void Foo()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public void a_test()
        {
            
        }

        [TestCase(1,2)]
        public void a_test_case(int x, int y)
        {

        }

        [Test, Explicit]
        public void a_test_with_explicit(int x, int y)
        {

        }

        [TestCase(1,2), Explicit]
        public void a_test_case_with_explicit(int x, int y)
        {
            
        }

        public void not_a_test()
        {
            
        }
    }

    public class DerivedTestFixture : BaseTestFixture
    {
        [Test]
        public void a_test()
        {
            
        }
    }

    [TestFixture]
    public class BaseTestFixture
    {
        [SetUp]
        public void Foo()
        {

        }

        [TearDown]
        public void TearDown()
        {

        }
    }

    public class Base1
    {
        [SetUp]
        public void SetUp1()
        {
            
        }

        [TearDown]
        public void Teardown1() {}
    }

    public class Base2 : Base1
    {
        [SetUp]
        public void SetUp2()
        {
            
        }
        [TearDown]
        public void Teardown2 () {}
    }

    
    public class Derived : Base2
    {
        [Test]
        public void a_test()
        {
            
        }
    }

}
