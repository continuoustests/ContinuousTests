using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using System.Reflection;
using AutoTest.TestRunners.Shared.Targeting;

namespace AutoTest.TestRunners.Tests.AssemblyAnalysis
{
    [TestFixture]
    public class TargetFrameworkLocatorTests
    {
        [Test]
        public void Should_get_framework_version_from_assembly()
        {
            var assembly = Assembly.GetExecutingAssembly().Location;
            var locator = new AssemblyPropertyReader();
            Assert.That(locator.GetTargetFramework(assembly), Is.EqualTo(new Version(2, 0)));
        }
    }
}
