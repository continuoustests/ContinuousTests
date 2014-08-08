using AutoTest.VS.Util;
using NUnit.Framework;

namespace AutoTest.VS.Util.Tests
{
    [TestFixture]
    public class GenericNameManglerTests
    {
        [Test]
        public void CanMangleSingleGeneric()
        {
            Assert.AreEqual("Foo`1<T>", GenericNameMangler.MangleParameterName("Foo<T>"));
        }

        [Test]
        public void CanMangleFullyQualifiedGeneric()
        {   
            Assert.AreEqual("System.Collections.Generic.IEnumerable`1<AutoTest.TestRunners.Shared.IAutoTestNetTestRunner>", GenericNameMangler.MangleParameterName("System.Collections.Generic.IEnumerable<AutoTest.TestRunners.Shared.IAutoTestNetTestRunner>"));
        }

        [Test]
        public void CanMangleMultipleGenerics()
        {
            Assert.AreEqual("Foo`2<T,V>", GenericNameMangler.MangleParameterName("Foo<T,V>"));
        }

        [Test]
        public void DoesNotMangleNameWithoutGenerics()
        {
            Assert.AreEqual("Foo", GenericNameMangler.MangleParameterName("Foo"));
        }

        [Test]
        public void CanHandleNestedGenerics()
        {
            Assert.AreEqual("Foo`2<Bar`1<T>,V>", GenericNameMangler.MangleParameterName("Foo<Bar<T>,V>"));
        }

        [Test]
        public void CanReturnWithoutParameters()
        {
            Assert.AreEqual("Foo`2", GenericNameMangler.MangleTypeName("Foo<Bar<T>,V>"));
        }

        [Test]
        public void CanMangleRemovingGenerics()
        {
            Assert.AreEqual("Foo", GenericNameMangler.MangleMethodName("Foo<Bar<T>,V>"));
        }

        [Test]
        public void CanMangleWithoutGenerics()
        {
            Assert.AreEqual("Foo", GenericNameMangler.MangleMethodName("Foo"));
        }
    }
}
