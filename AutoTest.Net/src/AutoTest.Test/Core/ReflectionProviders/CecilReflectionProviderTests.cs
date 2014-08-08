using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.ReflectionProviders;
using System.Reflection;

namespace AutoTest.Test.Core.ReflectionProviders
{
    [TestFixture]
    public class CecilReflectionProviderTests : BaseClass
    {
        [Test]
        public void Should_find_me()
        {
            var locator = new CecilReflectionProvider(Assembly.GetExecutingAssembly().Location);
            var method = locator.LocateMethod("AutoTest.Test.Core.ReflectionProviders.CecilReflectionProviderTests.Should_find_me");
            Assert.AreEqual("AutoTest.Test.Core.ReflectionProviders.CecilReflectionProviderTests.Should_find_me", method.Fullname);
            Assert.AreEqual("NUnit.Framework.TestAttribute", method.Attributes.ElementAt(0));
        }

        [Test]
        public void Should_find_my_parent()
        {
            var locator = new CecilReflectionProvider(Assembly.GetExecutingAssembly().Location);
            var cls = locator.LocateClass(locator.GetParentType("AutoTest.Test.Core.ReflectionProviders.CecilReflectionProviderTests.Should_find_me"));
            Assert.AreEqual("AutoTest.Test.Core.ReflectionProviders.CecilReflectionProviderTests", cls.Fullname);
            Assert.AreEqual("NUnit.Framework.TestFixtureAttribute", cls.Attributes.ElementAt(0));
        }

        [Test]
        public void Should_find_inherited_attributes()
        {
            var locator = new CecilReflectionProvider(Assembly.GetExecutingAssembly().Location);
            var cls = locator.LocateClass("AutoTest.Test.Core.ReflectionProviders.CecilReflectionProviderTests");
            Assert.AreEqual("AutoTest.Test.Core.ReflectionProviders.CecilReflectionProviderTests", cls.Fullname);
            Assert.AreEqual("AutoTest.Test.Core.ReflectionProviders.MyAttribute", cls.Attributes.ElementAt(1));
        }

        [Test]
        public void Should_find_inherited_attributes_in_methods()
        {
            var locator = new CecilReflectionProvider(Assembly.GetExecutingAssembly().Location);
            var method = locator.LocateMethod("AutoTest.Test.Core.ReflectionProviders.BaseClass.Blargh");
            Assert.AreEqual("AutoTest.Test.Core.ReflectionProviders.BaseClass.Blargh", method.Fullname);
            Assert.AreEqual(3, method.Attributes.Count());
        }

        [Test]
        public void Should_find_nested_classes()
        {
            var locator = new CecilReflectionProvider(Assembly.GetExecutingAssembly().Location);
            var method = locator.LocateMethod("AutoTest.Test.Core.ReflectionProviders.BaseClass+MyNestedClass+MyNestedNestedClass.SomeMethod");
            Assert.AreEqual("AutoTest.Test.Core.ReflectionProviders.BaseClass+MyNestedClass+MyNestedNestedClass.SomeMethod", method.Fullname);
        }

        [Test]
        public void Should_find_returns_types_as_nested_classes()
        {
            var locator = new CecilReflectionProvider(Assembly.GetExecutingAssembly().Location);
            var method = locator.LocateMethod("AutoTest.Test.Core.ReflectionProviders.BaseClass+MyNestedClass+MyNestedNestedClass.ReturningMethod");
            Assert.AreEqual("AutoTest.Test.Core.ReflectionProviders.BaseClass+MyNestedClass+MyNestedNestedClass.ReturningMethod", method.Fullname);
            Assert.AreEqual("AutoTest.Test.Core.ReflectionProviders.BaseClass+MyNestedClass", method.ReturnType);
        }
    }
    
    [MyAttribute]
    public class BaseClass
    {
        [MyOtherAttribute]
        public void Blargh()
        {
        }

        public class MyNestedClass
        {
            public class MyNestedNestedClass
            {
                public void SomeMethod()
                {
                }

                public MyNestedClass ReturningMethod()
                {
                    return null;
                }
            }
        }
    }

    public class MyAttribute : Attribute
    {
    }

    public class MyOtherAttribute : MyAttribute
    {
    }
}
