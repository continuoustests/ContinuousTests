using System;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests
{
    [TestFixture]
    public class when_looking_for_dependencies : AssemblyTestFixture
    {
        [Test]
        public void can_find_simple_method_call()
        {
            var method = assembly.GetMethodDefinition<StaticMethods>("CallsStatic");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses.HasMember(typeof(Console), "WriteLine"));
        }

        [Test]
        public void returns_method_definition()
        {
            var method = assembly.GetMethodDefinition<StaticMethods>("CallsStatic");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsNotNull(result.Definition);
            Assert.AreEqual("System.Void AutoTest.Minimizer.Tests.StaticMethods::CallsStatic()", result.Definition.GetCacheName());
        }

        [Test]
        public void empty_method_has_complexity_of_zero()
        {
            var method = assembly.GetMethodDefinition<StaticMethods>("CallsStatic");
            var result = TypeScanner.ScanMethod(method);
            Assert.AreEqual(0, result.Complexity);
        }


        [Test]
        public void simple_if_results_in_complexity_of_one()
        {
            var method = assembly.GetMethodDefinition<Ifs>("SimpleIf");
            var result = TypeScanner.ScanMethod(method);
            Assert.AreEqual(1, result.Complexity);
        }

        [Test]
        public void if_else_results_in_complexity_of_two()
        {
            var method = assembly.GetMethodDefinition<Ifs>("SimpleIfElse");
            var result = TypeScanner.ScanMethod(method);
            Assert.AreEqual(2, result.Complexity);
        }

        [Test]
        public void can_find_static_variable_read()
        {
            var method = assembly.GetMethodDefinition<StaticMethods>("ReadsClassStaticVariable");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses.HasMember<StaticMethods>("Foo", true));
        }

        [Test]
        public void can_find_static_variable_write()
        {
            var method = assembly.GetMethodDefinition<StaticMethods>("WritesClassStaticVariable");
            var resule = TypeScanner.ScanMethod(method);
            Assert.IsTrue(resule.MemberAccesses.HasMember<StaticMethods>("Foo", false));
            Assert.IsFalse(resule.MemberAccesses[0].IsReadOnly);
        }

        [Test]
        public void can_find_other_class_static_variable_read()
        {
            var method = assembly.GetMethodDefinition<StaticMethods>("ReadsOtherClassStaticVariable");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses.HasMember<Bar>("Baz", true));
        }

        [Test]
        public void can_find_other_class_static_variable_write()
        {
            var method = assembly.GetMethodDefinition<StaticMethods>("WritesOtherClassStaticVariable");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses.HasMember<Bar>("Baz", false));
        }

        [Test]
        public void can_find_same_object_instance_method_call()
        {
            var method = assembly.GetMethodDefinition<InstanceMethods>("CallsInstanceMethod");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses.HasMember<InstanceMethods>("InstanceMethod"));
        }

        [Test]
        public void can_find_read_of_instance_variable()
        {
            var method = assembly.GetMethodDefinition<InstanceMethods>("ReadsInstanceVariable");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses.HasMember<InstanceMethods>("_instanceVariable", true));
        }

        [Test]
        public void can_find_write_of_instance_variable()
        {
            var method = assembly.GetMethodDefinition<InstanceMethods>("WritesInstanceVariable");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses.HasMember<InstanceMethods>("_instanceVariable", false));
        }

        [Test]
        public void can_find_instance_call_to_another_object()
        {
            var method = assembly.GetMethodDefinition<InstanceMethods>("CallsOther");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses.HasMember<Other>("Foo"));           
        }

        [Test]
        public void can_find_instance_call_to_base_class()
        {
            var method = assembly.GetMethodDefinition<InstanceMethods>("CallsBase");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses.HasMember<A>("Foo"));                      
        }

        [Test]
        public void can_find_instance_call_to_interface()
        {
            var method = assembly.GetMethodDefinition<InstanceMethods>("CallsInterface");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses.HasMember<IFoo>("Foo"));
        }

        [Test]
        public void can_remove_bogus_constructor_reference()
        {
            var method = assembly.GetMethodDefinition<SimpleWithConstructor>("Foo");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses.Count == 0);            
        }

        [Test]
        public void can_read_generic_method()
        {
            var method = assembly.GetMethodDefinition<StaticMethods>("CallsGeneric");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses.HasMember<StaticMethods>("WithGeneric"));
            string s = result.MemberAccesses[0].MemberReference.GetCacheName();
        }

        [Test]
        public void can_read_generic_class_method()
        {
            var method = assembly.GetMethodDefinition<StaticMethods>("CallsGenericClass");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses.HasMember<SimpleGenericClass<int>>("Foo"));
        }

        [Test]
        public void can_find_simple_self_instance_method_call()
        {
            var method = assembly.GetMethodDefinition<InstanceMethods>("CallsInstanceMethod");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses[0].IsSelfCall);
        }

        [Test]
        public void can_find_self_instance_method_call_with_parameters()
        {
            var method = assembly.GetMethodDefinition<InstanceMethods>("CallsInstanceMethodWithParameters");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses[1].IsSelfCall); //new before it
        }

        [Test]
        public void can_find_self_instance_method_call_with_parameters_on_base()
        {
            var method = assembly.GetMethodDefinition<B>("CallsBase");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses[1].IsSelfCall); //new before it
        }

        [Test]
        public void can_find_field_reference_for_call()
        {
            var method = assembly.GetMethodDefinition<InstanceMethods>("CallsField");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsNotNull(result.MemberAccesses[1].MemberReference); //new before it
            Assert.IsTrue(((FieldReference) result.MemberAccesses[0].MemberReference).Name == "s");
        }

        [Test]
        public void can_find_self_instance_when_calling_abstract_from_base()
        {
            var method = assembly.GetMethodDefinition<Base>("CallsDerived");
            var result = TypeScanner.ScanMethod(method);
            Assert.IsTrue(result.MemberAccesses[0].IsSelfCall); //new before it
        }

        //TODO GREG REVIEW
        //[Test] 
        //public void finds_when_there_is_a_local_variable_call()
        //{
        //    var method = assembly.GetMethodDefinition<StaticMethods>("CallsLocal");
        //    var references = TypeScanner.ScanMethod(method);
        //    Assert.IsTrue(references[0].IsLocalVariable); //new before it            
        //}
    }

    public abstract class Base
    {
        public abstract void Foo();
        public void CallsDerived()
        {
            Foo();
        }
    }

    public class SimpleWithConstructor
    {
        private int x;
        private SimpleWithConstructor s;
        private int j;
        void Bar(int x)
        {
            var q = s;
            int i = 12*5;
            Console.WriteLine("foo");
            q.Bar(22);
            s.Bar(j);
        }
        public SimpleWithConstructor()
        {
            x = 5;
        }

        public void Foo()
        {
            int y = 17*22;
        }
    }

    public interface IFoo
    {
        void Foo();
    }

    class A
    {
        protected void Bar(int x, string y, A z)
        {
            
        }
        public virtual void Foo()
        {
            
        }
    }
    class B:A
    {
        public override void Foo()
        {
            base.Foo();
        }

        public void CallsBase()
        {
            this.Bar(5, "hello", new B());
        }
    }

    class Other
    {
        public void Foo()
        {
            
        }
    }

    class InstanceMethods
    {
        private int _instanceVariable;
        private void InstanceMethod()
        {
            
        }

        public void foo(int x, string y, InstanceMethods q)
        {
            
        }

        public void CallsInstanceMethodWithParameters()
        {
            this.foo(4, "test", new InstanceMethods());
        }

        public void CallsInstanceMethod()
        {
            this.InstanceMethod();
        }

        public void ReadsInstanceVariable()
        {
            var local = _instanceVariable;
        }

        public void WritesInstanceVariable()
        {
            _instanceVariable = 2743;
        }

        public void CallsOther(Other other)
        {
            other.Foo();
        }

        public void CallsBase(A a)
        {
            a.Foo();
        }

        public void CallsInterface(IFoo foo)
        {
            foo.Foo();
        }

        private string s;
        public void CallsField()
        {
            s.Replace(' ', ' ');
        }
    }

    class Bar
    {
        public static int Baz;
    }

    public class StaticMethods
    {
        private static int Foo;
        public static void CallsStatic()
        {
            Console.WriteLine("5");
        }

        public static void CallsLocal()
        {
            var s = new string(' ', 4);
        }

        public static void ReadsClassStaticVariable()
        {
            var local = Foo;
        }

        public static void WritesClassStaticVariable()
        {
            Foo = 22;
        }

        public static void ReadsOtherClassStaticVariable()
        {
            var local = Bar.Baz;
        }

        public static void WritesOtherClassStaticVariable()
        {
            Bar.Baz = 22;
        }

        public static void CallsGeneric()
        {
            WithGeneric<int>(5);
        }

        private static void WithGeneric<T>(T param)
        {
            
        }

        public static void CallsGenericClass()
        {
            var f = new SimpleGenericClass<int>();
            f.Foo(32);
        }
    }

    public class Ifs
    {
        public int x;
        public void SimpleIf()
        {
            if (x > 2)
            {
                Console.Write("x");
            }
        }
        public void SimpleIfElse()
        {
            if (x > 2)
            {
                Console.Write("x");
            }
            else
            {
                Console.Write("y");
            }
        }
    }

    public class SimpleGenericClass<T>
    {
        public void Foo(T t)
        {
            
        }
    }
}
