using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Caching;
using Rhino.Mocks;
using AutoTest.Test.Core.Messaging.Fakes;

namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
    [TestFixture]
    public class BuildListGeneratorTest
    {
        private BuildListGenerator _generator;
        private ICache _cache;
        private FakeProjectPrioritizer _prioritizer;

        [SetUp]
        public void SetUp()
        {
            _cache = MockRepository.GenerateMock<ICache>();
            _prioritizer = new FakeProjectPrioritizer();
            _generator = new BuildListGenerator(_cache, _prioritizer);
        }

        [Test]
        public void Should_generate_list()
        {
            var project1 = new Project("project1", new ProjectDocument(ProjectType.CSharp));
            project1.Value.AddReferencedBy("project2");
            project1.Value.AddReferencedBy("project3");
            var project2 = new Project("project2", new ProjectDocument(ProjectType.CSharp));
            project2.Value.AddReferencedBy("project3");

            _cache.Stub(c => c.Get<Project>("project1")).Return(project1);
            _cache.Stub(c => c.Get<Project>("project2")).Return(project2);
            _cache.Stub(c => c.Get<Project>("project3")).Return(new Project("project3", new ProjectDocument(ProjectType.CSharp)));

            var list = _generator.Generate(new string[] { "project1", "project3" });

            list.Length.ShouldEqual(3);
            list[0].ShouldEqual("project1");
            list[1].ShouldEqual("project2");
            list[2].ShouldEqual("project3");
        }

        [Test]
        public void Should_prioritize_list()
        {
            _cache.Stub(c => c.Get<Project>("project1"))
                .Return(new Project("project1", new ProjectDocument(ProjectType.CSharp)));

            var list = _generator.Generate(new string[] {"project1"});

            list.Length.ShouldEqual(1);
            _prioritizer.HasBeenCalled.ShouldBeTrue();
        }
    }
}
