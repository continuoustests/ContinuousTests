using System;
using System.Linq;
using AutoTest.Minimizer.TestIdentifiers;
using Mono.Cecil;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests.TestIdentifiers
{
    [TestFixture]
    class MSpecTranslatorTests : AssemblyTestFixture
    {
        [Test]
        public void null_method_receives_null_field()
        {
            Assert.IsNull(MSpecTranslator.TranslateGeneratedMethod(null));
        }

        [Test]
        public void can_translate_an_mspec_method_back_to_its_field()
        {
            var type = assembly.MainModule.Types.First(x => x.Name == "crapolalola");
            var method = type.Methods[1];
            var expectedField = type.Fields.First(f => f.Name == "it_should_foo");
            var actualField = MSpecTranslator.TranslateGeneratedMethod(method);
            Assert.AreEqual(actualField, expectedField);
        }
    }

    class crapolalola
    {
        public Action it_should_foo = () => Console.Write("");
        public Action it_should_foo2 = () => Console.Write("");
        public Action it_should_foo3 = () => Console.Write("");
        public Action it_should_foo4 = () => Console.Write("");
    }
}
