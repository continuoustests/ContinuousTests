using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Caching.Projects;
using System.IO;

namespace AutoTest.Test.Core.Caching.Projects
{
    [TestFixture]
    public class ProjectReferenceParserTests
    {
        [Test]
        public void Should_load_all_references()
        {
            var file = string.Format("TestResources{0}VS2008{0}CSharpNUnitTestProject.csproj", Path.DirectorySeparatorChar);
            var parser = new ProjectReferenceParser();
            var references = parser.GetAllBinaryReferences(file);

            Assert.That(references.Count(), Is.EqualTo(10));
            Assert.That(references.ElementAt(0), Is.EqualTo("nunit.framework, Version=2.4.8.0, Culture=neutral, PublicKeyToken=96d09a1eb7f44a77, processorArchitecture=MSIL"));
            Assert.That(references.ElementAt(1), Is.EqualTo("xunit, Version=1.5.0.1479, Culture=neutral, PublicKeyToken=8d05b1bb7a6fdb6c, processorArchitecture=MSIL"));
            Assert.That(references.ElementAt(2), Is.EqualTo("Machine.Specifications"));
            Assert.That(references.ElementAt(3), Is.EqualTo("Microsoft.VisualStudio.QualityTools.UnitTestFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL"));
            Assert.That(references.ElementAt(4), Is.EqualTo("System"));
            Assert.That(references.ElementAt(5), Is.EqualTo("System.Core"));
            Assert.That(references.ElementAt(6), Is.EqualTo("System.Xml.Linq"));
            Assert.That(references.ElementAt(7), Is.EqualTo("System.Data.DataSetExtensions"));
            Assert.That(references.ElementAt(8), Is.EqualTo("System.Data"));
            Assert.That(references.ElementAt(9), Is.EqualTo("System.Xml"));
        }

        [Test]
        public void Should_load_all_project_references()
        {
            var file = string.Format("TestResources{0}VS2008{0}CSharpNUnitTestProject.csproj", Path.DirectorySeparatorChar); ;
            var parser = new ProjectReferenceParser();
            var references = parser.GetAllProjectReferences(file);

            Assert.That(references.Count(), Is.EqualTo(1));
            Assert.That(references.ElementAt(0), Is.EqualTo(@"..\CSharpClassLibrary\CSharpClassLibrary.csproj"));
        }
    }
}
