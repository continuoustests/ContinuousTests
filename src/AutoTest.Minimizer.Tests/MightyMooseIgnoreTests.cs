using System;
using System.Threading;
using AutoTest.Minimizer.Extensions;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests
{
    [TestFixture]
    public class MightyMooseIgnoreTests : AssemblyTestFixture {
     
        [Test]
        public void can_read_ignore_attribute_off_provider_when_attribute_exists()
        {
            var ignored = assembly.GetTypeDefinition(typeof(IgnoredClass));
            Assert.IsTrue(ignored.ContainsIgnoreAttribute());
        }

        [Test]
        public void does_not_read_ignore_attribute_off_provider_without_exists()
        {
            Thread.Sleep(1055);
            var ignored = assembly.GetTypeDefinition(typeof(NonIgnoredClass));
            Assert.IsFalse(ignored.ContainsIgnoreAttribute());
        }
    }

    public class NonIgnoredClass
    {
        
    }

    [MightyMooseIgnore]
    public class IgnoredClass
    {
        
    }

    public class MightyMooseIgnoreAttribute : Attribute
    {
        
    }
}