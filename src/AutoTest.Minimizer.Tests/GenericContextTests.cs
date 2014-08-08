using System;
using System.Collections.Generic;
using AutoTest.Minimizer;
using Mono.Cecil;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests
{
    //TODO: GREG REVIEW
    [TestFixture]
    public class when_seeing_if_transition_is_allowed : AssemblyTestFixture
    {
        //[Test]
        //public void can_transfer_to_a_context_that_opens_a_parameter()
        //{
        //    var context1 = new Dictionary<string, GenericEntry> 
        //        { { "TBar", new GenericEntry(assembly.GetTypeDefinition<TestData>(), false, "") } };
        //    var context2 = new Dictionary<string, GenericEntry> { { "TBar", new GenericEntry(null, true, "TFoo") } };
        //    var context = new GenericContext(context1);
        //    Assert.IsTrue(context.CanTransitionTo(context2));
        //}
       
        //[Test]
        //public void can_not_transfer_to_a_context_that_is_directly_unreachable()
        //{
        //    var context1 = new Dictionary<string, GenericEntry> { { "TBar", new GenericEntry(assembly.GetTypeDefinition<TestData>(), false, "") } };
        //    var context2 = new Dictionary<string, GenericEntry> { { "TBar", new GenericEntry(assembly.GetTypeDefinition<Ab>(), false, "") } };
        //    var context = new GenericContext(context1);
        //    Assert.IsFalse(context.CanTransitionTo(context2));
        //}

        //[Test]
        //public void can_not_transfer_to_an_icorrect_context_that_is_remembered_indirectly()
        //{
        //    var context1 = new Dictionary<string, GenericEntry> { { "TBar", new GenericEntry(assembly.GetTypeDefinition<TestData>(), false, "") } };
        //    var context2 = new Dictionary<string, GenericEntry> { { "TFoo", new GenericEntry(null, true, "TBar") } };
        //    var context3 = new Dictionary<string, GenericEntry> { { "TFoo", new GenericEntry(assembly.GetTypeDefinition<Ab>(), false, "") } };
        //    var context = new GenericContext(context1);
        //    context = context.TransitionTo(context2);
        //    Assert.IsFalse(context.CanTransitionTo(context3));
        //}

        //[Test]
        //public void can_transfer_to_a_correct_context_that_is_remembered_indirectly()
        //{
        //    var context1 = new Dictionary<string, GenericEntry> { { "TBar", new GenericEntry(assembly.GetTypeDefinition<TestData>(), false, "") } };
        //    var context2 = new Dictionary<string, GenericEntry> { { "TFoo", new GenericEntry(null, true, "TBar") } };
        //    var context3 = new Dictionary<string, GenericEntry> { { "TFoo", new GenericEntry(assembly.GetTypeDefinition<TestData>(), false, "") } };
        //    var context = new GenericContext(context1);
        //    context = context.TransitionTo(context2);
        //    Assert.IsTrue(context.CanTransitionTo(context3));
        //}
    }
    [TestFixture]
    public class when_transitioning_generic_context : AssemblyTestFixture
    {
        [Test]
        public void does_not_copy_over_arguments_that_disappear()
        {
            var context1 = new Dictionary<string, GenericEntry> 
                { { "TFoo", new GenericEntry(assembly.GetTypeDefinition<TestData>(), false, "") },
                  { "TBar", new GenericEntry(assembly.GetTypeDefinition<TestData>(), false, "") } };
            var context2 = new Dictionary<string, GenericEntry> { { "TBar", new GenericEntry(null, true, "TFoo") } };
            var context = new GenericContext(context1);
            var newContext = context.TransitionTo(context2);
            Assert.AreEqual(1, newContext.LimitationCount);
        }
        //TODO GREG REVIEW
        //[Test]
        //public void brings_over_generic_arguments_with_correct_name()
        //{
        //    var context1 = new Dictionary<string, GenericEntry> 
        //        { { "TFoo", new GenericEntry(assembly.GetTypeDefinition<TestData>(), true, "T") }};
        //    var context2 = new Dictionary<string, GenericEntry> { { "TBar", new GenericEntry(null, true, "TFoo") } };
        //    var context = new GenericContext(context1);
        //    var newContext = context.TransitionTo(context2);
        //    Assert.IsTrue(newContext["TBar"].IsGeneric);
        //    Assert.AreEqual("T", newContext["TBar"].Name);
        //}
        //TODO GREG REVIEW
        //[Test]
        //public void remembers_previous_type_limitations()
        //{
        //    var context1 = new Dictionary<string, GenericEntry>
        //                       {{"TFoo", new GenericEntry(assembly.GetTypeDefinition<TestData>(), false, "TFoo")}};
        //    var context2 = new Dictionary<string, GenericEntry>
        //                       {{"TBar", new GenericEntry(null, true, "TFoo")}};
        //    var context = new GenericContext(context1);
        //    var newContext = context.TransitionTo(context2);
        //    Assert.IsFalse(newContext["TBar"].IsGeneric);
        //    Assert.IsTrue(newContext["TBar"].Type.Name == "TestData");
        //}
    }

    [TestFixture]
    public class when_reading_generic_context_information : AssemblyTestFixture
    {
        private readonly GenericContext context = new GenericContext();

        [Test]
        public void closed_generic_method_parameters_are_read()
        {
            var method = assembly.GetMethodDefinition<TestData>("ClosedGenericParametersOnMethodCall");
            var result = TypeScanner.ScanMethod(method);
            var testReference = (GenericInstanceMethod)result.MemberAccesses[0].ActualMethodDefinition;
            var genericContext = context.GetGenericContextOf(testReference);
            Assert.AreEqual(2, genericContext.Count);
            Assert.AreEqual("System.String", genericContext["T"].Type.FullName);
            Assert.AreEqual("System.Int32", genericContext["V"].Type.FullName);
        }

        [Test]
        public void open_generic_method_parameters_are_read()
        {
            var method = assembly.GetMethodDefinition<TestData>("OpenGenericParametersOnMethodCall");
            var result = TypeScanner.ScanMethod(method);
            var testReference = (GenericInstanceMethod)result.MemberAccesses[0].ActualMethodDefinition;
            var genericContext = context.GetGenericContextOf(testReference);
            Assert.AreEqual(2, genericContext.Count);
            Assert.AreEqual("TFoo", genericContext["T"].Name);
            Assert.AreEqual("TBar", genericContext["V"].Name);            
        }

        [Test]
        public void closed_generic_class_arguments_are_read_from_method()
        {
            var method = assembly.GetMethodDefinition<TestData>("ClosedClassArgumentsOnMethodCall");
            var result = TypeScanner.ScanMethod(method);
            var testReference = result.MemberAccesses[0].ActualMethodDefinition;
            var genericContext = context.GetGenericContextOf(testReference);
            Assert.AreEqual(2, genericContext.Count);
            Assert.AreEqual("System.String", genericContext["T"].Type.FullName);
            Assert.AreEqual("System.Int32", genericContext["V"].Type.FullName);
        }

        [Test]
        public void open_generic_class_arguments_are_read_from_method()
        {
            var method = assembly.GetMethodDefinition(typeof(GenericTestCaller<>), "OpenGenericClassArgumentsCall");
            var result = TypeScanner.ScanMethod(method);
            var testReference = result.MemberAccesses[0].ActualMethodDefinition;
            var genericContext = context.GetGenericContextOf(testReference);
            Assert.AreEqual(2, genericContext.Count);
            Assert.AreEqual("TBar", genericContext["T"].Name);
            Assert.AreEqual("System.String", genericContext["V"].Type.FullName);
        }
    }

    public class GenericTestCaller<TBar>
    {
        public static void OpenGenericClassArgumentsCall()
        {
            TestData.GenericParameterCall<TBar, string>();
        }
    }

    public class GenericTestData<T,V>
    {
        public static void Foo(){}
    }

    class TestData
    {
        public void ClosedGenericParametersOnMethodCall()
        {
            GenericParameterCall<string, int>();
        }

        public void ClosedClassArgumentsOnMethodCall()
        {
            GenericTestData<string, int>.Foo();
        }

        private void OpenGenericParametersOnMethodCall<TFoo, TBar>()
        {
            GenericParameterCall<TFoo, TBar>();
        }

        public static void GenericParameterCall<T, V>()
        {
            throw new NotImplementedException();
        }
    }
    class Ab {}
    class Ad : Ab {}
}
