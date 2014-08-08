using System.Collections.Generic;
using System.Threading;
using Simple.Testing.ClientFramework;
using NUnit.Framework;
using AutoTest.Minimizer.TestIdentifiers;
using Simple.Testing.Framework;

namespace AutoTest.Minimizer.Tests.TestIdentifiers
{
    [TestFixture]
    public class SimpleTestingTestIdentifierTests : AssemblyTestFixture
    {

        private readonly SimpleTestingTestIdentifier identifier = new SimpleTestingTestIdentifier();

        [Test]
        public void can_identify_a_simpletesting_single_specification()
        {
            var method = assembly.GetMethodDefinition<SimpleTestingTestFixture>("SingleSpecification");
            Assert.IsTrue(identifier.IsTest(method));
        }

        [Test]
        public void can_identify_a_sipletesting_single_specification_with_derived_attribute()
        {
            var method = assembly.GetMethodDefinition<SimpleTestingTestFixture>("DerivedSingleSpecification");
            Assert.IsTrue(identifier.IsTest(method));
        }

        [Test]
        public void can_identify_a_simpletesting_single_specification_with_class_implementing_interface()
        {
            var method = assembly.GetMethodDefinition<SimpleTestingTestFixture>("DerivedClassSingleSpecification");
            Assert.IsTrue(identifier.IsTest(method));
        }

        [Test]
        public void can_identify_a_simpletesting_single_specification_with_derived_from_base_class_implementing_interface()
        {
            var method = assembly.GetMethodDefinition<SimpleTestingTestFixture>("DerivedClassFromBaseSingleSpecification");
            Assert.IsTrue(identifier.IsTest(method));
        }

        [Test]
        public void can_identify_a_simpletesting_multi_specification()
        {
            var method = assembly.GetMethodDefinition<SimpleTestingTestFixture>("MultiSpecification");
            Assert.IsTrue(identifier.IsTest(method));
        } 

        [Test]
        public void can_identify_internal_simple_testing_setup_for_profiling()
        {
            var method = assembly.GetMethodDefinition<SpecificationRunner>("RunSetup");
            Assert.IsTrue(identifier.IsProfilerSetup(method));
        }

        [Test]
        public void can_identify_internal_simple_testing_teardown_for_profiling()
        {
            var method = assembly.GetMethodDefinition<SpecificationRunner>("RunTeardowns");
            Assert.IsTrue(identifier.IsProfilerTeardown(method));
        }

        [Test]
        public void can_identify_internal_simple_testing_assertions_for_profiling()
        {
            var method = assembly.GetMethodDefinition<SpecificationRunner>("RunAssertions");
            Assert.IsTrue(identifier.IsProfilerTest(method));
        }
    }

    public interface DerivedSpecification : Specification { }
    public class ClassImplementingSpecification : Specification
    {
        public string GetName()
        {
            return "";
        }
    }

    public class DerivedClassImplementingSpecificationOnBase : ClassImplementingSpecification { }

    public class SimpleTestingTestFixture {
        public IEnumerable<Specification> MultiSpecification()
        {
            return null;
        }
        public Specification SingleSpecification() {
            return null;
        }

        public DerivedSpecification DerivedSingleSpecification()
        {
            return null;
        }

        public ClassImplementingSpecification DerivedClassSingleSpecification()
        {
            return null;
        }

        public DerivedClassImplementingSpecificationOnBase DerivedClassFromBaseSingleSpecification()
        {
            return null;
        }
    }
}

namespace Simple.Testing.Framework
{
    class SpecificationRunner
    {
        public bool RunSetup<T>()
        {
            return false;
        }

        public bool RunAssertions<T>()
        {
            return false;
        }

        public bool RunTeardowns<T>()
        {
            return false;
        }
    }
}
