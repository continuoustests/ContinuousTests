using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using NUnit.Framework;
using AutoTest.Core.Caching.Projects;
using AutoTest.Test.Core.Caching.Projects.Fakes;
using AutoTest.Core.FileSystem;
using System.IO;
using Rhino.Mocks;
using AutoTest.Core.Caching;

namespace AutoTest.Test.Core.Caching.Projects
{
    [TestFixture]
    public class ProjectParserTest
    {
        private ProjectParser _parser;
        private FakeFileSystemService _fs;
        private IConfiguration _config;

        [SetUp]
        public void SetUp()
        {
            _config = MockRepository.GenerateMock<IConfiguration>();
            _fs = new FakeFileSystemService();
            _parser = new ProjectParser(_fs, _config);
        }

        [Test]
        public void Should_mark_found_project_as_read()
        {
            _config.Stub(x => x.ProjectsToIgnore).Return(new string[] {});
            var document = _parser.Parse(getCSharpProject(), null);
            document.IsReadFromFile.ShouldBeTrue();
        }

        [Test]
        public void Should_find_CSharp_references()
        {
            _config.Stub(x => x.ProjectsToIgnore).Return(new string[] {});
            var document = _parser.Parse(getCSharpProject(), null);
            document.References.Length.ShouldEqual(1);
            document.References[0].ShouldEqual(
                Path.GetFullPath(
                    string.Format("TestResources{0}CSharpClassLibrary{0}CSharpClassLibrary.csproj",
                                  Path.DirectorySeparatorChar)
                        .Replace("\\", Path.DirectorySeparatorChar.ToString())));
        }

        [Test]
        public void Should_find_VisualBasic_references()
        {
            _config.Stub(x => x.ProjectsToIgnore).Return(new string[] {});
            var document = _parser.Parse(getVisualBasicProject(), null);
            document.References.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_find_FSharp_references()
        {
            _config.Stub(x => x.ProjectsToIgnore).Return(new string[] {});
            var document = _parser.Parse(getFSharpProject(), null);
            document.References.Length.ShouldEqual(1);
            document.References[0].ShouldEqual(
                Path.GetFullPath(
                    string.Format("TestResources{0}FSharpClassLibrary{0}FSharpClassLibrary.fsproj",
                                  Path.DirectorySeparatorChar)
                        .Replace("\\", Path.DirectorySeparatorChar.ToString())));
        }

        [Test]
        public void Should_add_exists_referencedby_records()
        {
            _config.Stub(x => x.ProjectsToIgnore).Return(new string[] {});
            var existingDocument = new ProjectDocument(ProjectType.CSharp);
            existingDocument.AddReferencedBy("someproject");
            var document = _parser.Parse(getCSharpProject(), existingDocument);
            document.ReferencedBy[0].ShouldEqual("someproject");
        }

        [Test]
        public void Should_find_default_namespace()
        {
            _config.Stub(x => x.ProjectsToIgnore).Return(new string[] {});
            var document = _parser.Parse(getCSharpProject(), null);
            document.DefaultNamespace.ShouldEqual("CSharpNUnitTestProject");

            document = _parser.Parse(getVisualBasicProject(), null);
            document.DefaultNamespace.ShouldEqual("NUnitTestProjectVisualBasic");
        }

        [Test]
        public void Should_set_assembly_name()
        {
            _config.Stub(x => x.ProjectsToIgnore).Return(new string[] {});
            var document = _parser.Parse(getCSharpProject(), null);
            document.AssemblyName.ShouldEqual("CSharpNUnitTestProject.dll");

            document = _parser.Parse(getVisualBasicProject(), null);
            document.AssemblyName.ShouldEqual("NUnitTestProjectVisualBasic.exe");
        }

        [Test]
        public void Should_force_output_path_to_out_own_custom()
        {
            _config.Stub(x => x.ProjectsToIgnore).Return(new string[] {});
            var document = _parser.Parse(getCSharpProject(), null);
            document.OutputPath.ShouldEqual(string.Format("bin{0}AutoTest.Net{0}", Path.DirectorySeparatorChar));

            document = _parser.Parse(getVisualBasicProject(), null);
            document.OutputPath.ShouldEqual(string.Format("bin{0}AutoTest.Net{0}", Path.DirectorySeparatorChar));
        }

        [Test]
        public void Should_set_framework_version()
        {
            _config.Stub(x => x.ProjectsToIgnore).Return(new string[] {});
            var document = _parser.Parse(getCSharpProject(), null);
            document.Framework.Equals("v3.5");

            document = _parser.Parse(getVisualBasicProject(), null);
            document.Framework.Equals("v3.5");
        }

        [Test]
        public void Should_set_product_version()
        {
            _config.Stub(x => x.ProjectsToIgnore).Return(new string[] {});
            var document = _parser.Parse(getCSharpProject(), null);
            document.ProductVersion.Equals("9.0.30729");

            document = _parser.Parse(getVisualBasicProject(), null);
            document.ProductVersion.Equals("9.0.30729");
        }
		
		[Test]
        public void Should_find_binary_reference()
        {
            _config.Stub(x => x.ProjectsToIgnore).Return(new string[] {});
            var document = _parser.Parse(getCSharpProject(), null);
            document.BinaryReferences.Length.ShouldEqual(10);
            document.BinaryReferences[0].ShouldEqual("nunit.framework, Version=2.4.8.0, Culture=neutral, PublicKeyToken=96d09a1eb7f44a77, processorArchitecture=MSIL");
			document.BinaryReferences[1].ShouldEqual("xunit, Version=1.5.0.1479, Culture=neutral, PublicKeyToken=8d05b1bb7a6fdb6c, processorArchitecture=MSIL");
			document.BinaryReferences[2].ShouldEqual("Machine.Specifications");
			document.BinaryReferences[3].ShouldEqual("Microsoft.VisualStudio.QualityTools.UnitTestFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL");
			document.BinaryReferences[4].ShouldEqual("System");
			document.BinaryReferences[5].ShouldEqual("System.Core");
			document.BinaryReferences[6].ShouldEqual("System.Xml.Linq");
			document.BinaryReferences[7].ShouldEqual("System.Data.DataSetExtensions");
			document.BinaryReferences[8].ShouldEqual("System.Data");
			document.BinaryReferences[9].ShouldEqual("System.Xml");
        }

        [Test]
        public void Should_read_files()
        {
            _config.Stub(x => x.ProjectsToIgnore).Return(new string[] {});
            var document = _parser.Parse(getCSharpProject(), null);
            document.Files[0].File.ShouldEqual(Path.GetFullPath(string.Format("TestResources{0}VS2008{0}Class1.cs", Path.DirectorySeparatorChar).Replace("\\", Path.DirectorySeparatorChar.ToString())));
            document.Files[0].Type.ShouldEqual(FileType.Compile);
            document.Files[1].File.ShouldEqual(Path.GetFullPath(string.Format("TestResources{0}VS2008{0}Properties{0}AssemblyInfo.cs", Path.DirectorySeparatorChar).Replace("\\", Path.DirectorySeparatorChar.ToString())));
            document.Files[2].File.ShouldEqual(Path.GetFullPath(string.Format("TestResources{0}VS2008{0}UI{0}VersionedConfigOption.resx", Path.DirectorySeparatorChar).Replace("\\", Path.DirectorySeparatorChar.ToString())));
            document.Files[2].Type.ShouldEqual(FileType.Resource);
            document.Files[3].File.ShouldEqual(Path.GetFullPath(string.Format("TestResources{0}VS2008{0}Resources{0}MM_shaded.bmp", Path.DirectorySeparatorChar).Replace("\\", Path.DirectorySeparatorChar.ToString())));
            document.Files[3].Type.ShouldEqual(FileType.None);
        }

        [Test]
        public void Should_not_parse_ignored_projects()
        {
            _config.Stub(x => x.ProjectsToIgnore).Return(new[] { "*IgnoredProject.csproj" });
            Assert.That(_parser.Parse("An/IgnoredProject.csproj", null), Is.Null);
        }

        private string getCSharpProject()
        {
            return string.Format("TestResources{0}VS2008{0}CSharpNUnitTestProject.csproj", Path.DirectorySeparatorChar);
        }

        private string getVisualBasicProject()
        {
            return string.Format("TestResources{0}VS2008{0}NUnitTestProjectVisualBasic.vbproj", Path.DirectorySeparatorChar);
        }
		
		private string getCSharpNoAnyCPUProject()
		{
			return string.Format("TestResources{0}VS2008{0}CSharpNoAnyCPU.csproj", Path.DirectorySeparatorChar);
		}

        private string getFSharpProject()
        {
            return string.Format("TestResources{0}VS2008{0}FSharpNUnitTestProject.fsproj", Path.DirectorySeparatorChar);
        }
    }
}
