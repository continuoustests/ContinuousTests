using System;
using AssemblyChangeDetector;
using NUnit.Framework;

namespace Coupling.Tests
{
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
    }

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
    }
}