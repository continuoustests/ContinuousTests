using AutoTest.Minimizer.Extensions;
using Mono.Cecil;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests
{
    [TestFixture]
    public class when_translating_parameters : AssemblyTestFixture
    {
        [Test]
        public void non_generic_parameter_translates_to_itself()
        {
            var method = assembly.GetMethodDefinition<ParameterTestMethods>("NonGenericCall");
            var result = TypeScanner.ScanMethod(method);
            var type = result.MemberAccesses[0].ActualMethodDefinition.Parameters[0].GetTypeWithGenericResolve();
            Assert.That(type == result.MemberAccesses[0].ActualMethodDefinition.Parameters[0].ParameterType);
        }

        [Test]
        public void closed_generic_parameter_translates_to_generic_argument_on_static()
        {
            var method = assembly.GetMethodDefinition<ParameterTestMethods>("ClosedStaticGenericCall");
            var result = TypeScanner.ScanMethod(method);
            var type = result.MemberAccesses[0].ActualMethodDefinition.Parameters[0].GetTypeWithGenericResolve();
            Assert.That(type.FullName == "System.Int32");
        }
        

        [Test]
        public void closed_generic_parameter_translates_to_generic_argument_on_instance()
        {
            var method = assembly.GetMethodDefinition<ParameterTestMethods>("ClosedInstanceGenericCall");
            var result = TypeScanner.ScanMethod(method);
            var type = result.MemberAccesses[1].ActualMethodDefinition.Parameters[0].GetTypeWithGenericResolve();
            Assert.AreEqual(type.FullName,"System.String");            
        }

        [Test]
        public void generic_parameter_on_static_method_can_be_translated()
        {
            var method = assembly.GetMethodDefinition<ParameterTestMethods>("StaticWithGenericParameter");
            var result = TypeScanner.ScanMethod(method);
            var testReference = (GenericInstanceMethod)result.MemberAccesses[0].ActualMethodDefinition;
            var type = testReference.Parameters[0].GetTypeWithGenericResolve(testReference);
            Assert.That(type.FullName == "System.String");               
        }

        [Test]
        public void generic_argument_and_generic_parameter_on_static()
        {
            var method = assembly.GetMethodDefinition<ParameterTestMethods>("ClosedStaticWithGenericParameter");
            var result = TypeScanner.ScanMethod(method);
            var testReference = (GenericInstanceMethod) result.MemberAccesses[0].ActualMethodDefinition;
            var type = testReference.Parameters[0].GetTypeWithGenericResolve( testReference);
            Assert.That(type.FullName == "System.String");
            type = testReference.Parameters[1].GetTypeWithGenericResolve(testReference);
            Assert.That(type.FullName == "System.Int32");
        }

        [Test]
        public void can_read_array_type_argument()
        {
            var method = assembly.GetMethodDefinition<ParameterTestMethods>("ClosedInstanceTypeSpecGenericCall");
            var result = TypeScanner.ScanMethod(method);
            var type = result.MemberAccesses[1].ActualMethodDefinition.Parameters[0].GetTypeWithGenericResolve();
            Assert.AreEqual(type.FullName, "System.String[]");                        
        }

        [Test]
        public void can_read_array_type_parameter()
        {
            var method = assembly.GetMethodDefinition<ParameterTestMethods>("StaticWithTypeSpecGenericParameter");
            var result = TypeScanner.ScanMethod(method);
            var testReference = (GenericInstanceMethod)result.MemberAccesses[1].ActualMethodDefinition;
            var type = testReference.Parameters[0].GetTypeWithGenericResolve(testReference);
            Assert.AreEqual(type.FullName,"System.String[]");
        }

        [Test]
        public void can_read_open_generic_type_argument_with_translation()
        {
            var method = assembly.GetMethodDefinition(typeof(OpenParameterTestMethods<>), "OpenInstanceGenericCall");
            var result = TypeScanner.ScanMethod(method);
            var type = result.MemberAccesses[2].ActualMethodDefinition.Parameters[0].GetTypeWithGenericResolve();
            Assert.AreEqual(type.FullName, "TFoo");            
        }

        [Test]
        public void can_read_open_generic_parameter_with_translation()
        {
            var method = assembly.GetMethodDefinition(typeof(OpenParameterTestMethods<>), "OpenParameterGenericCall");
            var result = TypeScanner.ScanMethod(method);
            var type = result.MemberAccesses[1].ActualMethodDefinition.Parameters[0].GetTypeWithGenericResolve();
            Assert.AreEqual(type.FullName, "TFoo");
        }
    }
    
    public class ParameterTestMethods
    {
        private static void Call(int x)
        {
            
        }

        private static void Whatever<T>(T val) {}

        public void StaticWithGenericParameter()
        {
            Whatever<string>("hello");
        }

        public void StaticWithTypeSpecGenericParameter()
        {
            Whatever<string[]>(new[] {"hello"});
        }

        public void ClosedStaticWithGenericParameter()
        {
            GenericStatic<int>.WithParam<string>("hello", 42);
        }

        public void ClosedInstanceTypeSpecGenericCall()
        {
            GenericStatic<string[]>.Something(new []{"hello"});
        }

        public void ClosedInstanceGenericCall()
        {
            new InstanceGenericClass<int, string>().Something("test");
        }
        public void ClosedStaticGenericCall()
        {
            GenericStatic<int>.Something(5);
        }
        public void NonGenericCall()
        {
            Call(42);
        }
    }

    class OpenParameterTestMethods<TFoo>
    {
        public void OpenInstanceGenericCall()
        {
            new SimpleGenericClass<TFoo>().Foo(default(TFoo));
        }

        public void OpenParameterGenericCall()
        {
            GenericStatic<TFoo>.Something(default(TFoo));
        }
    }

    static class GenericStatic<T>
    {
        public static void WithParam<V>(V val, T val2)
        {
            
        }
        public static void Something(T val)
        {

        }
    }

    class InstanceGenericClass<T,V>
    {
        public void Something(V val)
        {
            
        }
    }

}
