using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.FileSystem;
using System.IO;
using System.Reflection;
using AutoTest.Core.Caching;
using Rhino.Mocks;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Test.Core.FileSystem
{
    [TestFixture]
    public class WatchPathLocatorTests
    {
        private ICache _cache;
        private WatchPathLocator _locator;

        [SetUp]
        public void Setup()
        {
            _cache = MockRepository.GenerateMock<ICache>();
            _locator = new WatchPathLocator(_cache);
        }

        [Test]
        public void Should_use_default_path_if_there_are_no_projects_below_it()
        {
            _cache.Stub(x => x.GetAll<Project>()).Return(new Project[]
                        {
                            new Project(getRoot() + string.Format("Path{0}To{0}This{0}Awesome{0}Project{0}AProject.csproj", Path.DirectorySeparatorChar), null)
                        });

            var path = getRoot() + string.Format("src{0}something", Path.DirectorySeparatorChar);
            Assert.That(_locator.Locate(path), Is.EqualTo(path));
        }

        [Test]
        public void Should_use_lowest_path_when_projects_are_located_below_the_solution()
        {
            _cache.Stub(x => x.GetAll<Project>()).Return(new Project[]
                        { 
                            new Project(getRoot() + string.Format("Path{0}To{0}This{0}Project{0}AProject.csproj", Path.DirectorySeparatorChar), null),
                            new Project(getRoot() + string.Format("Path{0}To{0}Project{0}AnotherProject.csproj", Path.DirectorySeparatorChar), null)
                        });

            Assert.That(
                _locator.Locate(getRoot() + string.Format("Path{0}To{0}This{0}Awesome{0}Solution", Path.DirectorySeparatorChar)),
                Is.EqualTo(getRoot() + string.Format("Path{0}To", Path.DirectorySeparatorChar)));
        }

        private string getRoot()
        {
            if (OS.IsPosix)
                return Path.DirectorySeparatorChar.ToString();
            else
                return "C:\\";
        }
    }
}
