using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Configuration;

namespace AutoTest.Test.Core.Caching.Projects.Fakes
{
    class FakeServiceLocator : IServiceLocator
    {
        private Func<ICache> _getCache;
        private IProjectParser _parser;

        public FakeServiceLocator(IProjectParser parser, Func<ICache> getCache)
        {
            _parser = parser;
            _getCache = getCache;
        }

        #region IServiceLocator Members

        public T Locate<T>()
        {
            if (typeof(T) == typeof(IPrepare<Project>))
                return (T)(IPrepare<Project>)new ProjectPreparer(_parser, _getCache.Invoke());
            if (typeof(T) == typeof(ICreate<Project>))
                return (T)(ICreate<Project>)new ProjectFactory();
            return default(T);
        }
		
		public T Locate<T>(string name)
		{
			return default(T);
		}

        public T[] LocateAll<T>()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
