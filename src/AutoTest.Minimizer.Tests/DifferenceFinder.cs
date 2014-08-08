using System;
using AutoTest.Minimizer;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests
{

    [TestFixture]
    public class when_detecting_changes_in_method : AssemblyTestFixture
    {

        [Test]
        public void methods_that_are_different_show_up_as_different()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("Instance1"),
                                                   assembly.GetMethodDefinition<TestMethods>("Instance2")));
        }

        [Test]
        public void methods_that_are_the_same_are_not_different()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsFalse(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("Instance2"),
                                                   assembly.GetMethodDefinition<TestMethods>("SameAsInstance2")));
        }

        [Test]
        public void methods_that_only_have_different_string_data_are_different()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("Instance2"),
                                                   assembly.GetMethodDefinition<TestMethods>("StringDifferentAsInstance2")));
        }

        [Test]
        public void method_level_attribute_add_results_in_method_being_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("Instance2"),
                                                   assembly.GetMethodDefinition<TestMethods>("SameAsInstance2WithAttribute")));
            
        }

        [Test]
        public void method_level_attribute_remove_results_in_method_being_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("SameAsInstance2WithAttribute"),
                                                   assembly.GetMethodDefinition<TestMethods>("Instance2")));

        }

        [Test]
        public void method_level_attribute_on_both_results_in_not_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsFalse(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("SameAsInstance2WithAttribute"),
                                                   assembly.GetMethodDefinition<TestMethods>("SameAsInstance2WithAttribute")));

        }

        [Test]
        public void method_level_attribute_with_different_data_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("HasMethodAttributeWithData"),
                                                   assembly.GetMethodDefinition<TestMethods>("HasMethodAttributeWithDifferentData")));
        }

        [Test]
        public void method_level_attribute_without_different_data_results_in_not_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsFalse(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("HasMethodAttributeWithData"),
                                                   assembly.GetMethodDefinition<TestMethods>("HasMethodAttributeWithData")));
        }

        [Test]
        public void parameter_level_attribute_add_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("HasNoParameterAttribute"),
                                                   assembly.GetMethodDefinition<TestMethods>("HasParameterAttribute")));
        }

        [Test]
        public void parameter_level_attribute_remove_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("HasParameterAttribute"),
                                                   assembly.GetMethodDefinition<TestMethods>("HasNoParameterAttribute")));
        }


        [Test]
        public void parameter_level_attribute_on_both_results_in_not_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsFalse(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("HasParameterAttribute"),
                                                   assembly.GetMethodDefinition<TestMethods>("HasParameterAttribute")));
        }

        [Test]
        public void paramter_level_attribute_with_different_data_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();

            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("HasParameterAttributeWithData"),
                                                   assembly.GetMethodDefinition<TestMethods>("HasParameterAttributeWithDifferentData")));
        }

        [Test]
        public void parameter_level_attribute_without_different_data_results_in_not_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsFalse(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("HasParameterAttributeWithData"),
                                                   assembly.GetMethodDefinition<TestMethods>("HasParameterAttributeWithData")));
        }

        [Test]
        public void return_level_attribute_add_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("HasNoReturnAttribute"),
                                                   assembly.GetMethodDefinition<TestMethods>("HasReturnAttribute")));
        }

        [Test]
        public void return_level_attribute_remove_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("HasReturnAttribute"),
                                                   assembly.GetMethodDefinition<TestMethods>("HasNoReturnAttribute")));
        }
        [Test]
        public void return_level_attribute_on_both_results_in_not_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsFalse(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("HasReturnAttribute"),
                                                   assembly.GetMethodDefinition<TestMethods>("HasReturnAttribute")));
        }

        [Test]
        public void return_level_attribute_with_different_data_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("HasReturnAttributeWithData"),
                                                   assembly.GetMethodDefinition<TestMethods>("HasReturnAttributeWithDifferentData")));
        }

        [Test]
        public void return_level_attribute_without_different_data_results_in_not_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsFalse(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestMethods>("HasReturnAttributeWithData"),
                                                   assembly.GetMethodDefinition<TestMethods>("HasReturnAttributeWithData")));
        }

        [Test]
        public void type_level_attribute_add_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<Type2>("Foo"),
                                                   assembly.GetMethodDefinition<Type1>("Foo")));
        }

        [Test]
        public void type_level_attribute_remove_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<Type1>("Foo"),
                                                   assembly.GetMethodDefinition<Type2>("Foo")));
        }

        [Test]
        public void type_level_attribute_same_results_in_not_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<Type1>("Foo"),
                                                   assembly.GetMethodDefinition<Type2>("Foo")));
        }

        [Test]
        public void inherited_type_level_attribute_add_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<Type1>("Foo"),
                                                   assembly.GetMethodDefinition<Type1Derived>("Foo")));
        }

        [Test]
        public void inherited_type_level_attribute_remove_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<Type1Derived>("Foo"),
                                                   assembly.GetMethodDefinition<Type1>("Foo")));
        }

        [Test]
        public void inherited_method_level_attribute_remove_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestingDerived>("HasMethodAttribute"),
                                                   assembly.GetMethodDefinition<TestingDerived>("HasNoAttribute")));
        }

        [Test]
        public void inherited_method_level_attribute_add_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestingDerived>("HasMethodAttribute"),
                                                   assembly.GetMethodDefinition<TestingDerived>("HasNoAttribute")));
        }

        [Test]
        public void inherited_return_level_attribute_remove_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestingDerived>("HasNoAttribute"),
                                                   assembly.GetMethodDefinition<TestingDerived>("HasInheritedReturnAttribute")));
        }

        [Test]
        public void inherited_return_level_attribute_add_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestingDerived>("HasInheritedReturnAttribute"),
                                                   assembly.GetMethodDefinition<TestingDerived>("HasNoAttribute")));
        }

        [Test]
        public void inherited_param_level_attribute_remove_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestingDerived>("HasNoAttribute"),
                                                   assembly.GetMethodDefinition<TestingDerived>("HasInheritedParamAttribute")));
        }

        [Test]
        public void inherited_param_level_attribute_add_results_in_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsTrue(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestingDerived>("HasInheritedParamAttribute"),
                                                   assembly.GetMethodDefinition<TestingDerived>("HasNoAttribute")));
        }

        [Test]
        public void noninherited_param_level_attribute_compare_results_in_not_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsFalse(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestingDerived>("HasNonInheritableParamAttribute"),
                                                   assembly.GetMethodDefinition<TestingDerived>("HasNoAttribute")));
        }

        [Test]
        public void noninherited_method_level_attribute_compare_results_in_not_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsFalse(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestingDerived>("HasNonInheritableMethodAttribute"),
                                                   assembly.GetMethodDefinition<TestingDerived>("HasNoAttribute")));
        }

        [Test]
        public void noninherited_return_level_attribute_compare_results_in_not_marked_as_modified()
        {
            var dectector = new MethodILChangeDetector();
            Assert.IsFalse(dectector.AreDifferentIL(assembly.GetMethodDefinition<TestingDerived>("HasNonInhertibleReturnAttribute"),
                                                   assembly.GetMethodDefinition<TestingDerived>("HasNoAttribute")));
        }
    }

    public class TestMethods
    {
        public void Instance1()
        {
            var x = 7;
            Console.Write(x);
        }
        public void Instance2()
        {
            var x = "greg";
            Console.Write(x);
        }

        public void SameAsInstance2()
        {
            var x = "greg";
            Console.Write(x);
        }

        public void StringDifferentAsInstance2()
        {
            var x = "gregrocks";
            Console.Write(x);
        }

        [TestingAttribute]
        public void SameAsInstance2WithAttribute()
        {
            var x = "greg";
            Console.Write(x);
        }

        [TestingAttributeWithData(12, 15)]
        public void HasMethodAttributeWithData() { }

        [TestingAttributeWithData(13, 15)]
        public void HasMethodAttributeWithDifferentData() { }

        public void HasNoParameterAttribute(int x) { }
        public void HasParameterAttribute([TestingAttribute] int x) { }

        public void HasParameterAttributeWithData([TestingAttributeWithData(12, 15)] int x) { }
        public void HasParameterAttributeWithDifferentData([TestingAttributeWithData(12, 16)] int x) { }


        public int HasNoReturnAttribute(int x)
        {
            return 0;
        }
        [return: TestingAttribute]
        public int HasReturnAttribute(int x)
        {
            return 0;
        }
        [return: TestingAttributeWithData(12, 15)]
        public int HasReturnAttributeWithData(int x)
        {
            return 0;
        }
        [TestingAttributeWithData(12, 16)]
        public int HasReturnAttributeWithDifferentData(int x)
        {
            return 0;
        }
    }
    
    public class TestingBase
    {
        [return: Testing]
        public virtual int HasInheritedReturnAttribute(int x)
        {
            return 0;
        }

        [return: NonInheritable]
        public virtual int HasNonInhertibleReturnAttribute(int x)
        {
            return 0;
        }

        public virtual int HasInheritedParamAttribute([Testing] int x)
        {
            return 0;
        }

        public virtual int HasNonInheritableParamAttribute([NonInheritable] int x)
        {
            return 0;
        }

        [Testing]
        public virtual int HasMethodAttribute(int x)
        {
            return 0;
        }

        [NonInheritable]
        public virtual int HasNonInheritableMethodAttribute(int x)
        {
            return 0;
        }
    }

    public class TestingDerived : TestingBase
    {
        public override int HasInheritedReturnAttribute(int x)
        {
            return 0;
        }

        public override int HasMethodAttribute(int x)
        {
            return 0;
        }

        public override int HasInheritedParamAttribute(int x)
        {
            return 0;
        }

        public override int HasNonInheritableMethodAttribute(int x)
        {
            return 0;
        }

        public override int  HasNonInheritableParamAttribute(int x)
        {
            return 0;
        }

        public override int  HasNonInhertibleReturnAttribute(int x)
        {
            return 0;
        }

        public int HasNoAttribute(int x)
        {
            return 0;
        }
    }

    class Type1
    {
        public void Foo()
        {
            
        }
    }

    [TestingAttribute]
    class Type2
    {
        public void Foo()
        {
            
        }
    }
    
    [Testing]
    class Type1Base
    {
        public virtual void Foo()
        {
            
        }
    }

    class Type1Derived : Type1Base
    {
        public override void Foo()
        {
            
        }
    }

   [AttributeUsage(AttributeTargets.All,
                   AllowMultiple=false,Inherited=true)]
    public class TestingAttributeWithDataAttribute : Attribute
    {
        public int X { get; set; }
        public int Y { get; set; }

        public TestingAttributeWithDataAttribute(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
   [AttributeUsage(AttributeTargets.All,
                  AllowMultiple = false, Inherited = true)]
   public class TestingAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.All,
                   AllowMultiple = false, Inherited = false)]
    public class NonInheritableAttribute : Attribute
    {
        
    }
}