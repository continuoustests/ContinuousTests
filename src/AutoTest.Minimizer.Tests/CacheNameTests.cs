using System;
using System.Linq;
using System.Collections.Generic;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests
{
    [TestFixture]
    public class when_generating_cache_name_for_method : AssemblyTestFixture
    {

        [Test]
        public void normal_instance_method_returns_simple_cache_name()
        {
            var memberReference = GetMethodReferenceFrom(assembly.GetMethodDefinition<Methods>("CallsNormalMethod"));
            Assert.AreEqual("System.Void AutoTest.Minimizer.Tests.Instance::Foo()", memberReference.GetCacheName());
        }

        
        [Test]
        public void static_method_returns_simple_cache_name()
        {
            var memberReference = GetMethodReferenceFrom(assembly.GetMethodDefinition<Methods>("CallsStaticMethod"));
            Assert.AreEqual("System.Void AutoTest.Minimizer.Tests.Methods::StaticMethodReference()", memberReference.GetCacheName());            
        }

        [Test]
        public void generic_method_returns_element_name_of_method()
        {
            var memberReference = GetMethodReferenceFrom(assembly.GetMethodDefinition<Methods>("CallsGenericMethod"));
            Assert.AreEqual("System.Void AutoTest.Minimizer.Tests.Methods::GenericMethodReference(T)", memberReference.GetCacheName());                        
        }

        [Test]
        public void generic_class_method_returns_open_definition_of_generic()
        {
            var memberReference = GetMethodReferenceFrom(assembly.GetMethodDefinition<Methods>("CallsGenericClass"));
            Assert.AreEqual("System.Void AutoTest.Minimizer.Tests.GenericClass`2::Foo()", memberReference.GetCacheName());
        }

        [Test]
        public void generic_class_method_returns_open_definition_of_generic_with_parameters()
        {
            var memberReference = GetMethodReferenceFrom(assembly.GetMethodDefinition<Methods>("CallsGenericClassWithParams"));
            Assert.AreEqual("System.Void AutoTest.Minimizer.Tests.GenericClass`2::WithParam(T)", memberReference.GetCacheName());
        }

        [Test]
        public void generic_method_on_generic_class_returns_open_definition_of_type_and_method()
        {
            var memberReference = GetMethodReferenceFrom(assembly.GetMethodDefinition<Methods>("CallsGenericMethodOnGenericClass"));
            Assert.AreEqual("System.Void AutoTest.Minimizer.Tests.GenericClass`2::GenericMethod(T,R)", memberReference.GetCacheName());            
        }


        private static MemberReference GetMethodReferenceFrom(MethodDefinition methodDefinition)
        {
            int x = 52324;
            foreach (var m in
                TypeScanner.ScanMethod(methodDefinition).MemberAccesses.Where(m => m.MemberReference is MethodReference))
            {
                return m.MemberReference;
            }
            return null;
        }
    }

    [TestFixture]
    public class when_generating_cache_name_for_member : AssemblyTestFixture
    {
        [Test]
        public void normal_instance_member_returns_simple_cache_name()
        {
            var memberReference = GetMemberReferenceFrom(assembly.GetMethodDefinition<Methods>("AccessesVariable"));
            Assert.AreEqual("System.Int32 AutoTest.Minimizer.Tests.Methods::x", memberReference.GetCacheName());
        }
        [Test]
        public void generic_instance_member_returns_open_cache_name()
        {
            var memberReference = GetMemberReferenceFrom(assembly.GetMethodDefinition<Methods>("Usesfoo"));
            Assert.AreEqual("T AutoTest.Minimizer.Tests.GenericClass`2::foo", memberReference.GetCacheName());
        }
        private static MemberReference GetMemberReferenceFrom(MethodDefinition methodDefinition)
        {
            return TypeScanner.ScanMethod(methodDefinition).MemberAccesses[0].MemberReference;
        }
    }

    

    public class Methods
    {
        private Instance i = new Instance();
        private GenericClass<int, string> j = new GenericClass<int, string>();
        private GenericClass<List<int>, int> q = new GenericClass<List<int>, int>();
        private int x;

        public void Usesfoo(GenericClass<int, string> value)
        {
            value.foo = 5;
        }

        public void AccessesVariable()
        {
            x = 72;
        }

        public void CallsNormalMethod()
        {
            i.Foo();
        }

        private static void StaticMethodReference()
        {

        }

        public static void GenericMethodReference<T>(T item)
        {
            
        }

        public static void CallsGenericMethod()
        {
            GenericMethodReference(5);
        }

        public static void CallsStaticMethod()
        {
            StaticMethodReference();
        }

        public void CallsGenericClass()
        {
            j.Foo();
        }

        public void CallsGenericClassWithParams()
        {
            j.WithParam(5);
        }

        public void CallsGenericMethodOnGenericClass()
        {
            j.GenericMethod<int>(5,5);
        }

        private static List<int> list;
        public void CallsGenericMethodOnGenericClassWithGenericParam()
        {
            q.GenericMethod<int>(list, 5);
        }
    }

    public class GenericClass<T,V>
    {
        public T foo;
        public void Foo() {}
        public void WithParam(T param1)
        {
                    
        }

        
        public void GenericMethod<R>(T param1, R param2)
        {
            
        }

    }

    internal class Instance
    {
        
        public void Foo()
        {
        }
    }
}
