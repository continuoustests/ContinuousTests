using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using System.IO;
using System.Reflection;

namespace AutoTest.TestRunners.Tests.AssemblyAnalysis
{
    [TestFixture]
    public class TypeLocatorTests : BaseClass
    {
        [Test]
        public void Should_find_me()
        {
            var locator = new SystemReflectionProvider(Assembly.GetExecutingAssembly().Location);
            var method = locator.LocateMethod("AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests.Should_find_me");
            Assert.AreEqual("AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests.Should_find_me", method.Fullname);
            Assert.AreEqual("NUnit.Framework.TestAttribute", method.Attributes.ElementAt(0));
        }

        [Test]
        public void Should_find_my_parent()
        {
            var locator = new SystemReflectionProvider(Assembly.GetExecutingAssembly().Location);
            var cls = locator.LocateClass(locator.GetParentType("AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests.Should_find_me"));
            Assert.AreEqual("AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests", cls.Fullname);
            Assert.AreEqual("NUnit.Framework.TestFixtureAttribute", cls.Attributes.ElementAt(0));
            Assert.AreEqual(10, cls.Methods.Count());
        }

        [Test]
        public void Should_find_inherited_attributes()
        {
            var locator = new SystemReflectionProvider(Assembly.GetExecutingAssembly().Location);
            var cls = locator.LocateClass("AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests");
            Assert.AreEqual("AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests", cls.Fullname);
            Assert.AreEqual("AutoTest.TestRunners.Tests.AssemblyAnalysis.MyAttribute", cls.Attributes.ElementAt(3));
        }

        [Test]
        public void Should_find_inherited_attributes_in_methods()
        {
            var locator = new SystemReflectionProvider(Assembly.GetExecutingAssembly().Location);
            var method = locator.LocateMethod("AutoTest.TestRunners.Tests.AssemblyAnalysis.BaseClass.Blargh");
            Assert.AreEqual("AutoTest.TestRunners.Tests.AssemblyAnalysis.BaseClass.Blargh", method.Fullname);
            Assert.AreEqual(4, method.Attributes.Count());
        }

        [Test]
        public void Should_find_nested_classes()
        {
            var locator = new SystemReflectionProvider(Assembly.GetExecutingAssembly().Location);
            var method = locator.LocateMethod("AutoTest.TestRunners.Tests.AssemblyAnalysis.BaseClass+MyNestedClass+MyNestedNestedClass.SomeMethod");
            Assert.AreEqual("AutoTest.TestRunners.Tests.AssemblyAnalysis.BaseClass+MyNestedClass+MyNestedNestedClass.SomeMethod", method.Fullname);
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
