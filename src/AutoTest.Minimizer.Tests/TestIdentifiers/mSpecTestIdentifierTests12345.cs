using System;
using System.Linq;
using AutoTest.Minimizer.Extensions;
using AutoTest.Minimizer.TestIdentifiers;
using Machine.Specifications;
using Mono.Cecil;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests.TestIdentifiers
{
        [TestFixture]
        public class mSpecTestIdentifierTests : AssemblyTestFixture
        {
            private readonly mSpecTestIdentifier identifier = new mSpecTestIdentifier();

            [Test]
            public void can_identify_test()
            {
                var definition = assembly.GetTypeDefinition<MSpecTestClass>();
                var methods = definition.GetGeneratedMethods().ToList();
                for (int i = 0; i < methods.Count;i++ )
                {
                    var ok = i == 3 || !identifier.IsTest(methods[i]);
                    Assert.IsTrue(ok);
                }
            }

            [Test]
            public void does_not_identify_non_mspec()
            {
                TypeDefinition definition = assembly.GetTypeDefinition<not_an_mspec_test>();
                var methods = definition.GetGeneratedMethods().ToList();
                foreach (var method in methods)
                {
                    Assert.IsFalse(identifier.IsTest(method));
                }
            }



            [Test]
            public void does_not_identify_a_test_inabstract_class()
            {
                TypeDefinition definition = assembly.GetTypeDefinition<not_an_mspec_test>();
                var methods = definition.GetGeneratedMethods().ToList();
                foreach (var method in methods)
                {
                    Assert.IsFalse(identifier.IsTest(method));
                }
            }

            [Test]
            public void does_not_identify_because_establish_as_a_test()
            {
                TypeDefinition definition = assembly.GetTypeDefinition<MSpecTestClass>();
                var methods = definition.GetGeneratedMethods().ToList();
                int count = methods.Count(method => identifier.IsTest(method));
                Assert.AreEqual(1, count);
            }

    }

    [Subject("Test Class For MSpec")]
    public class MSpecTestClass 
    {
        private static int x;

        Establish context = () =>
                                {
                                    x = 5;
                                };
        Because expression = () => x.ToString();


        Because of = () =>
                         {
                             x.ToString();
                         };

        It should_do_something = () =>
                                {
                                    Console.WriteLine(x);
                                };
    }


    [Subject("Test Class For MSpec")]
    public abstract class AbstractMSpecTestClass
    {
        private static int x;

        Establish context = () =>
        {
            x = 5;
        };
        Because expression = () => x.ToString();


        Because of = () =>
        {
            x.ToString();
        };

        It should_do_something = () =>
        {
            Console.WriteLine(x);
        };
    }

    public class not_an_mspec_test
    {
        private Action<object> x = y => Console.WriteLine(y);

        private Action<object> j = y => Console.WriteLine(y);
    }
}