using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Test.Core.Caching.Projects.Fakes
{
    [TestFixture]
    public class FakeCacheTest
    {
        private FakeCache _fakeCache;

        [SetUp]
        public void SetUp()
        {
            _fakeCache = new FakeCache();
        }

        [Test]
        public void Should_be_able_to_verify_added_keys()
        {
            _fakeCache.Add<FakeRecord>("somekey");
            _fakeCache.ShouldHaveBeenAdded("somekey");
        }

        [Test]
        public void Should_faile_when_key_not_added()
        {
            _fakeCache.Add<FakeRecord>("somekey");
            _fakeCache.ShouldHaveAdded("another_key");
        }

        [Test]
        public void Should_return_project_when_provided_with_one()
        {
            var project = new Project("somekey", null);
            _fakeCache.WhenGeting("somekey").Return(project);
            var returnedProject = _fakeCache.Get<Project>("somekey");
            returnedProject.ShouldBeTheSameAs(project);
        }

        [Test]
        public void Should_return_null_when_provided_with_wrong_key()
        {
            var project = new Project("somekey", null);
            _fakeCache.WhenGeting("somekey").Return(project);
            var returnedProject = _fakeCache.Get<Project>("another key");
            returnedProject.ShouldNotBeTheSameAs(project);
        }
    }
}
