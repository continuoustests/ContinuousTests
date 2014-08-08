using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.FileSystem.ProjectLocators;
using AutoTest.Core.FileSystem;
using AutoTest.Test.Core.FileSystem.Fakes;
using AutoTest.Messages;

namespace AutoTest.Test.Core.FileSystem.ProjectLocators
{
    [TestFixture]
    public class CSharpLocatorTest
    {
        private ChangedFile[] _changedProjects;
        private CSharpLocator _locator;
        private FakeProjectFileCrawler _crawler;

        [SetUp]
        public void SetUp()
        {
            _changedProjects = new ChangedFile[] { };
            _crawler = new FakeProjectFileCrawler(_changedProjects);
            _locator = new CSharpLocator(_crawler);
        }

        [Test]
        public void Should_locate_csharp_project()
        {
            var files = _locator.Locate("somechangedfile.cs");
            files.ShouldBeTheSameAs(_changedProjects);
            _crawler.ShouldHaveBeenAskedToLookFor(".csproj");
        }

        [Test]
        public void Should_verify_that_file_is_project()
        {
            _locator.IsProject("somefile.csproj").ShouldBeTrue();
        }
    }
}
