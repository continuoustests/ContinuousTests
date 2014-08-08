using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching;
using NUnit.Framework;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Test.Core.Caching.Projects.Fakes
{
    class FakeCache : ICache
    {
        private string _keyToGet = "";
        private Project _projectToGet = null;
        private List<string> _addedFields = new List<string>();
		private bool _throwErrorOnAdd = false;
        private List<ProjectFile> _files = new List<ProjectFile>();

        public void Add(IEnumerable<ProjectFile> file)
        {
            _files.AddRange(file);
        }

        public void InvalidateProjectFiles(string project)
        {
            _files.RemoveAll(x => x.Project.Equals(project));
        }

        public ProjectFile[] GetAllProjectFiles()
        {
            return _files.ToArray();
        }

        public bool IsProjectFile(string file)
        {
            return false;
        }

		public void ShouldThrowErrorOnAdd()
		{
			_throwErrorOnAdd = true;
		}

        public void ShouldHaveBeenAdded(string key)
        {
            Assert.IsTrue(_addedFields.Contains(key));
        }

        public void ShouldHaveAdded(string key)
        {
            Assert.IsFalse(_addedFields.Contains(key));
        }

        public FakeCache WhenGeting(string key)
        {
            _keyToGet = key;
            return this;
        }

        public void Return(Project project)
        {
            _projectToGet = project;
        }

        #region ICache Members

        public int Count
        {
            get { return _addedFields.Count; }
        }

        public void Add<T>(string key) where T : IRecord
        {
			if (_throwErrorOnAdd)
				throw new Exception("Add casued an error");
            _addedFields.Add(key);
        }

        public bool Exists(string key)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key) where T : IRecord
        {
            if (typeof(T).Equals(typeof(Project)) && _keyToGet.Equals(key))
                return (T) (IRecord) _projectToGet;
            return default(T);
        }

        public T Get<T>(int index) where T : IRecord
        {
            throw new NotImplementedException();
        }

        public T[] GetAll<T>() where T : IRecord
        {
            throw new NotImplementedException();
        }

        public void Reload<T>(string key) where T : IRecord
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
