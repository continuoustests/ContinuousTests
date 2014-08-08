using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Test.Core.Caching.Projects
{
    [TestFixture]
    public class ProjectDocumentTest
    {
        private ProjectDocument _document;

        [SetUp]
        public void setUp()
        {
            _document = new ProjectDocument(ProjectType.CSharp);
        }

        [Test]
        public void Can_add_reference()
        {
            _document.AddReference("project");
            _document.References[0].ShouldEqual("project");
        }

        [Test]
        public void Can_remove_reference()
        {
            _document.AddReference("project1");
            _document.AddReference("project2");
            _document.RemoveReference("project1");
            _document.References.Length.ShouldEqual(1);
            _document.References[0].ShouldEqual("project2");
        }

        [Test]
        public void Can_add_reference_range()
        {
            _document.AddReference(new string[] { "project1", "project2" });
            _document.References[0].ShouldEqual("project1");
            _document.References[1].ShouldEqual("project2");
        }

        [Test]
        public void Can_add_referencedBy()
        {
            _document.AddReferencedBy("project");
            _document.ReferencedBy[0].ShouldEqual("project");
        }

        [Test]
        public void Can_remove_referencedBy()
        {
            _document.AddReferencedBy("project1");
            _document.AddReferencedBy("project2");
            _document.RemoveReferencedBy("project1");
            _document.ReferencedBy.Length.ShouldEqual(1);
            _document.ReferencedBy[0].ShouldEqual("project2");
        }

        [Test]
        public void Can_add_referencedBy_range()
        {
            _document.AddReferencedBy(new string[] { "project1", "project2" });
            _document.ReferencedBy[0].ShouldEqual("project1");
            _document.ReferencedBy[1].ShouldEqual("project2");
        }

        [Test]
        public void When_calling_HasBeenReadFromFile_IsReadFromFile_should_be_true()
        {
            _document.IsReadFromFile.ShouldBeFalse();
            _document.HasBeenReadFromFile();
            _document.IsReadFromFile.ShouldBeTrue();
        }

        [Test]
        public void Can_verify_referenced_By()
        {
            _document.AddReferencedBy("AnotherProject");
            _document.IsReferencedBy("AnotherProject").ShouldBeTrue();
        }

        [Test]
        public void Can_verify_references()
        {
            _document.AddReference("AnotherProject");
            _document.IsReferencing("AnotherProject").ShouldBeTrue();
        }

        [Test]
        public void Should_set_assembly_name()
        {
            _document.SetAssemblyName("some name");
            _document.AssemblyName.ShouldEqual("some name");
        }

        [Test]
        public void Should_set_outputpath()
        {
            _document.SetOutputPath("output path");
            _document.OutputPath.ShouldEqual("output path");
        }

        [Test]
        public void Should_set_framework()
        {
            _document.SetFramework("v3.5");
            _document.Framework.ShouldEqual("v3.5");
        }

        [Test]
        public void Should_set_VSVersion()
        {
            _document.SetVSVersion("9.0");
            _document.ProductVersion.ShouldEqual("9.0");
        }
    }
}
