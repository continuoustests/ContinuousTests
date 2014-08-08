using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    public class ProjectPrioritizer : IPrioritizeProjects
    {
        private ICache _cache;
        private List<string> _prioritized;
        private string[] _references;

        public ProjectPrioritizer(ICache cache)
        {
            _cache = cache;
        }

        #region IPrioritizeProjects Members

        public string[] Prioritize(string[] references)
        {
            _references = references;
            return sortByReferenced();
        }

        #endregion

        private string[] sortByReferenced()
        {
            _prioritized = new List<string>(_references);
            for (int i = 0; i < _references.Length; i++)
                prioritizeReferenceAndItsReferencedBys(i);
            return _prioritized.ToArray();
        }

        private void prioritizeReferenceAndItsReferencedBys(int referenceIndex)
        {
            var myNewIndex = referenceIndex;
            var project = _cache.Get<Project>(_references[referenceIndex]);
            foreach (var referencedBy in project.Value.ReferencedBy)
            {
                int index = getReferencedBysIndex(referencedBy);
                if (notInReferenceList(index) || referenceIsPrioritized(index, myNewIndex))
                    continue;
                myNewIndex = moveReferenceInFrontOfReferencedBy(myNewIndex, index);
            }
        }

        private bool referenceIsPrioritized(int index, int myNewIndex)
        {
            return index > myNewIndex;
        }

        private bool notInReferenceList(int index)
        {
            return index < 0;
        }

        private int getReferencedBysIndex(string referencedBy)
        {
            return _prioritized.FindIndex(0, p => p.Equals(referencedBy));
        }

        private int moveReferenceInFrontOfReferencedBy(int referenceIndex, int index)
        {
            var reference = _prioritized[referenceIndex];
            _prioritized.RemoveAt(referenceIndex);
            _prioritized.Insert(index, reference);
            return index;
        }
    }
}
