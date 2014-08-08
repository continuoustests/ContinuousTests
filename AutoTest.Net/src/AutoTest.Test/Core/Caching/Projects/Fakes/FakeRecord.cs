using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching;

namespace AutoTest.Test.Core.Caching.Projects.Fakes
{
    class FakeRecord : IRecord
    {
        private string _key;

        public FakeRecord(string key)
        {
            _key = key;
        }

        #region IRecord Members

        public string Key
        {
            get { return _key; }
        }

        #endregion
    }
}
