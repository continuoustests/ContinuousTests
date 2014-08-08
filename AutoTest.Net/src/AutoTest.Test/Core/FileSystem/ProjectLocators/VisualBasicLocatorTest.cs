using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.FileSystem;
using AutoTest.Test.Core.FileSystem.Fakes;
using AutoTest.Core.FileSystem.ProjectLocators;
using AutoTest.Messages;

namespace AutoTest.Test.Core.FileSystem.ProjectLocators
{
    [TestFixture]
    public class VisualBasicLocatorTest
    {

        private ChangedFile[] _changedProjects;
        private VisualBasicLocator _locator;
        private FakeProjectFileCrawler _crawler;

        [SetUp]
        public void SetUp()
        {
            _changedProjects = new ChangedFile[] { };
            _crawler = new FakeProjectFileCrawler(_changedProjects);
            _locator = new VisualBasicLocator(_crawler);
        }

        [Test]
        public void Should_locate_visual_basic_project()
        {
            var files = _locator.Locate("somechangedfile.vb");
            files.ShouldBeTheSameAs(_changedProjects);
            _crawler.ShouldHaveBeenAskedToLookFor(".vbproj");
        }

        [Test]
        public void Should_verify_that_file_is_project()
        {
            _locator.IsProject("somefile.vbproj").ShouldBeTrue();
        }
    }
}
