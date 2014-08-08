using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.DebugLog;

namespace AutoTest.Core.Caching.Projects
{
    public class ProjectPreparer : IPrepare<Project>
    {
        private IProjectParser _parser;
        private ICache _cache;

        public ProjectPreparer(IProjectParser parser, ICache cache)
        {
            _parser = parser;
            _cache = cache;
        }

        #region IPrepare<Project> Members

        public Project Prepare(Project record)
        {
            if (record.Value == null || !record.Value.IsReadFromFile)
                return parseProject(record);
            return record;
        }

        #endregion

        private Project parseProject(Project record)
        {
            var document = parseDocument(record);
            if (document == null)
                return null;
            setupDependingProjects(record.Key, document);
            cacheProjectFiles(record.Key, document.Files);
            return new Project(record.Key, document);
        }

        private ProjectDocument parseDocument(Project record)
        {
            try
            {
                return _parser.Parse(record.Key, record.Value);
            }
            catch (Exception exception)
            {
                var messageString = string.Format("Failed parsing project {0}. Project will not be built. ({1})", record.Key, exception.Message);
                Debug.WriteInfo(messageString);
            }
            return null;
        }

        private void setupDependingProjects(string key, ProjectDocument document)
        {
            foreach (var reference in document.References)
            {
                var project = _cache.Get<Project>(reference);
                
                if (project == null)
                    project = createProject(reference);

                if (!project.Value.IsReferencedBy(key))
                    project.Value.AddReferencedBy(key);
            }
        }

        private void cacheProjectFiles(string p, ProjectFile[] projectFiles)
        {
            _cache.InvalidateProjectFiles(p);
            _cache.Add(projectFiles);
        }

        private Project createProject(string reference)
        {
            _cache.Add<Project>(reference);
            return _cache.Get<Project>(reference);
        }
    }
}
