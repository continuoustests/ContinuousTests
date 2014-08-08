using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    public class BuildListGenerator : IGenerateBuildList
    {
        private ICache _cache;
        private List<string> _list;
        private IPrioritizeProjects _prioritizer;

        public BuildListGenerator(ICache cache, IPrioritizeProjects prioritizer)
        {
            _cache = cache;
            _prioritizer = prioritizer;
        }

        public string[] Generate(string[] keys)
        {
            _list = new List<string>();
			foreach (var key in keys)
                addProject(key);
            return _prioritizer.Prioritize(_list.ToArray());
        }

        private void addProject(string key)
        {
            var project = _cache.Get<Project>(key);
            if (project == null || project.Value == null)
                return;
            addToList(key);
            foreach (var reference in project.Value.ReferencedBy)
                addProject(reference);
        }

        private void addToList(string key)
        {
            if (!_list.Contains(key))
                _list.Add(key);
        }
    }
}
