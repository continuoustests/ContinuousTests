using System;
using Mono.Cecil;
using NUnit.Framework;
using Xunit;
using Assert = NUnit.Framework.Assert;

namespace AutoTest.Minimizer.TestIdentifiers.XUnit2.Tests
{
    static class AssemblyReferenceExtension
    {
        public static TypeDefinition GetTypeDefinition<T>(this AssemblyDefinition assembly)
        {
            var type = assembly.MainModule.GetType(typeof(T).FullName);
            Assert.IsNotNull(type, "type " + typeof(T).FullName + " was null");
            return type;
        }

        public static TypeDefinition GetTypeDefinition(this AssemblyDefinition assembly, Type t)
        {
            var type = assembly.MainModule.GetType(t.FullName);
            Assert.IsNotNull(type, "type " + t.FullName + " was null");
            return type;
        }


        public static MethodDefinition GetMethodDefinition(this AssemblyDefinition assembly, Type t, string name)
        {
            var type = assembly.GetTypeDefinition(t);
            foreach (var method in type.Methods)
            {
                if (method.Name == name)
                    return method;
            }
            throw new MissingMethodException("Unable to find method: " + name);
        }

        public static MethodDefinition GetMethodDefinition<T>(this AssemblyDefinition assembly, string name)
        {
            var type = assembly.GetTypeDefinition<T>();
            foreach (var method in type.Methods)
            {
                if (method.Name == name)
                    return method;
            }
            throw new MissingMethodException("Unable to find method: " + name);
        }
    }
    
    public class AssemblyTestFixture
    {
        protected AssemblyDefinition assembly;

        [SetUp]
        public virtual void SetUp()
        {
            var mdr = new DefaultAssemblyResolver();
            assembly = mdr.Resolve("AutoTest.Minimizer.TestIdentifiers.XUnit2.Tests");
            Assert.IsNotNull(assembly, "assembly was null");
        }
    }

    [TestFixture]
    public class xUnit2TestIdentifierTests : AssemblyTestFixture
    {
        private readonly XUnit2TestIdentifier identifier = new XUnit2TestIdentifier();

        [Test]
        public void can_identify_a_xunit_test()
        {
            var method = assembly.GetMethodDefinition<XUnit2TestFixture>("a_test");
            Assert.IsTrue(identifier.IsTest(method));
        }


        [Test]
        public void does_not_identify_an_abstract_test()
        {
            var method = assembly.GetMethodDefinition<AbstractXUnit2TestFixture>("abstract_test");
            Assert.IsFalse(identifier.IsTest(method));
        }

        [Test]
        public void does_not_identify_a_test_inabstract_class()
        {
            var method = assembly.GetMethodDefinition<AbstractXUnit2TestFixture>("a_test");
            Assert.IsFalse(identifier.IsTest(method));
        }

        [Test]
        public void does_not_identify_non_test_as_test()
        {
            var method = assembly.GetMethodDefinition<XUnit2TestFixture>("not_a_test");
            Assert.IsFalse(identifier.IsTest(method));
        }

        [Test]
        public void can_identify_constructor_as_setup()
        {
            var method = assembly.GetMethodDefinition<XUnit2TestFixture>(".ctor");
            Assert.IsTrue(identifier.IsSetup(method));
        }

        [Test]
        public void can_identify_finalizer_as_teardown()
        {
            var method = assembly.GetMethodDefinition<XUnit2TestFixture>("Finalize");
            Assert.IsTrue(identifier.IsTeardown(method));
        }

        [Test]
        public void can_identify_constructor_on_base_as_hidden_dependency()
        {
            var method = assembly.GetMethodDefinition<XUnitDerivedTestFixture>("test");
            Assert.IsTrue(identifier.GetHiddenDependenciesForTest(method).Count == 1);
        }
    }

    public abstract class AbstractXUnit2TestFixture
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

    public class XUnit2TestFixture
    {
        public XUnit2TestFixture()
        {
            
        }

        ~XUnit2TestFixture()
        {
            
        }

        [Fact]
        public void a_test()
        {
            
        }

        public void not_a_test()
        {
            
        }

        [Xunit.Theory, InlineData(5)]
        public void data_driven_test(int x)
        {

        }
    }

}