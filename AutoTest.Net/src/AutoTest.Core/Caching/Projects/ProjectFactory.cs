using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Caching.Projects
{
    public class ProjectFactory : ICreate<Project>
    {
        #region ICreate<Project> Members

        public Project Create(string key)
        {
            return new Project(key, null);
        }

        #endregion
    }
}
