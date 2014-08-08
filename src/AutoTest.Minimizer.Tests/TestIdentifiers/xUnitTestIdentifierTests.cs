using System;
using AutoTest.Minimizer.TestIdentifiers;
using NUnit.Framework;
using Xunit;
using Xunit.Extensions;
using Assert = NUnit.Framework.Assert;

namespace AutoTest.Minimizer.Tests.TestIdentifiers
{
    [TestFixture]
    public class xUnitTestIdentifierTests : AssemblyTestFixture
    {
        private readonly XUnitTestIdentifier identifier = new XUnitTestIdentifier();

        [Test]
        public void can_identify_a_xunit_test()
        {
            var method = assembly.GetMethodDefinition<XUnitTestFixture>("a_test");
            Assert.IsTrue(identifier.IsTest(method));
        }


        [Test]
        public void does_not_identify_an_abstract_test()
        {
            var method = assembly.GetMethodDefinition<AbstractXUnitTestFixture>("abstract_test");
            Assert.IsFalse(identifier.IsTest(method));
        }

        [Test]
        public void does_not_identify_a_test_inabstract_class()
        {
            var method = assembly.GetMethodDefinition<AbstractXUnitTestFixture>("a_test");
            Assert.IsFalse(identifier.IsTest(method));
        }

        [Test]
        public void does_not_identify_non_test_as_test()
        {
            var method = assembly.GetMethodDefinition<XUnitTestFixture>("not_a_test");
            Assert.IsFalse(identifier.IsTest(method));
        }

        [Test]
        public void can_identify_constructor_as_setup()
        {
            var method = assembly.GetMethodDefinition<XUnitTestFixture>(".ctor");
            Assert.IsTrue(identifier.IsSetup(method));
        }

        [Test]
        public void can_identify_finalizer_as_teardown()
        {
            var method = assembly.GetMethodDefinition<XUnitTestFixture>("Finalize");
            Assert.IsTrue(identifier.IsTeardown(method));
        }

        [Test]
        public void can_identify_constructor_on_base_as_hidden_dependency()
        {
            var method = assembly.GetMethodDefinition<XUnitDerivedTestFixture>("test");
            Assert.IsTrue(identifier.GetHiddenDependenciesForTest(method).Count == 1);
        }
    }

    public abstract class AbstractXUnitTestFixture
    {
        [Fact]
        public abstract void abstract_test();
        [Fact]
        public void a_test(){}
    }

    public class XUnitDerivedTestFixture : XUnitBaseFixture
    {
        [Fact]
        public void test()
        {
            
        }
    }

    public class XUnitBaseFixture
    {
        public XUnitBaseFixture()
        {
            Console.Write("foo");
        }
    }

    public class XUnitTestFixture
    {
        public XUnitTestFixture()
        {
            
        }

        ~XUnitTestFixture()
        {
            
        }

        [Fact]
        public void a_test()
        {
            
        }

        public void not_a_test()
        {
            
        }

        [Xunit.Extensions.Theory, InlineData(5)]
        public void data_driven_test(int x)
        {

        }
    }

}