using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Caching;
using Rhino.Mocks;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
    [TestFixture]
    public class ReferencePrioritizerTest
    {
        private ProjectPrioritizer _prioritizer;
        private ICache _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = MockRepository.GenerateMock<ICache>();
            _prioritizer = new ProjectPrioritizer(_cache);
        }

        [Test]
        public void When_no_references_should_return_argument()
        {
            var references = new string[] {"reference1", "reference2"};
            _cache.Stub(c => c.Get<Project>("reference1")).Return(getProject());
            _cache.Stub(c => c.Get<Project>("reference2")).Return(getProject());

            var prioritized = _prioritizer.Prioritize(references);
            
            prioritized[0].ShouldEqual(references[0]);
            prioritized[1].ShouldEqual(references[1]);
        }

        [Test]
        public void When_having_referencing_tests_prioritize_referenced()
        {
            var references = new string[] {"reference1", "reference2"};
            _cache.Stub(c => c.Get<Project>("reference1")).Return(getProject());
            _cache.Stub(c => c.Get<Project>("reference2")).Return(getProjectThatIsReferencedBy(new string[] { "reference1" }));
            
            var prioritized = _prioritizer.Prioritize(references);
            
            prioritized[0].ShouldEqual(references[1]);
            prioritized[1].ShouldEqual(references[0]);
        }

        [Test]
        public void When_having_multiple_references_should_prioritize_by_reference()
        {
            var references = new string[] { "reference1", "reference2", "reference3", "reference4" };
            _cache.Stub(c => c.Get<Project>("reference1")).Return(getProject());
            _cache.Stub(c => c.Get<Project>("reference2")).Return(getProjectThatIsReferencedBy(new string[] { "reference1", "reference3" }));
            _cache.Stub(c => c.Get<Project>("reference3")).Return(getProjectThatIsReferencedBy(new string[] { "reference1" }));
            _cache.Stub(c => c.Get<Project>("reference4")).Return(getProjectThatIsReferencedBy(new string[] { "reference3", "reference2" }));

            var prioritized = _prioritizer.Prioritize(references);

            prioritized[0].ShouldEqual(references[3]);
            prioritized[1].ShouldEqual(references[1]);
            prioritized[2].ShouldEqual(references[2]);
            prioritized[3].ShouldEqual(references[0]);
        }

        private Project getProject()
        {
            return new Project("", new ProjectDocument(ProjectType.CSharp));
        }

        private Project getProjectThatIsReferencedBy(string[] references)
        {
            var project = getProject();
            project.Value.AddReferencedBy(references);
            return project;
        }
    }
}
