using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Caching;
using Rhino.Mocks;

namespace AutoTest.Test.Core.Caching.Projects
{
    [TestFixture]
    public class ProjectReloaderTest
    {
        private ProjectReloader _reloader;
        private ICache _cache;
        private Project _project;

        [SetUp]
        public void SetUp()
        {
            _cache = MockRepository.GenerateMock<ICache>();
            _reloader = new ProjectReloader(_cache);
            _project = new Project("project", new ProjectDocument(ProjectType.CSharp));
            _project.Value.AddReferencedBy("Referenced by");
            _project.Value.HasBeenReadFromFile();
        }

        [Test]
        public void Should_mark_as_dirty()
        {
            _reloader.MarkAsDirty(_project);
            _project.Value.IsReadFromFile.ShouldBeFalse();
        }

        [Test]
        public void Should_preserve_referencedbys()
        {
            _reloader.MarkAsDirty(_project);
            _project.Value.ReferencedBy.Contains("Referenced by").ShouldBeTrue();
        }

        [Test]
        public void Should_remove_remote_referencedbys()
        {
            var referencedProject1 = new Project("reference1", new ProjectDocument(ProjectType.CSharp));
            referencedProject1.Value.AddReferencedBy(_project.Key);
            _cache.Stub(c => c.Get<Project>("reference1")).Return(referencedProject1);
            var referencedProject2 = new Project("reference2", new ProjectDocument(ProjectType.CSharp));
            referencedProject2.Value.AddReferencedBy(_project.Key);
            _cache.Stub(c => c.Get<Project>("reference2")).Return(referencedProject2);
            _project.Value.AddReference(referencedProject1.Key);
            _project.Value.AddReference(referencedProject2.Key);

            _reloader.MarkAsDirty(_project);
            referencedProject1.Value.ReferencedBy.Length.ShouldEqual(0);
            referencedProject2.Value.ReferencedBy.Length.ShouldEqual(0);
        }
    }
}
