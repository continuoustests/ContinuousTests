using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging.MessageConsumers;

namespace AutoTest.Test.Core.Messaging.Fakes
{
    class FakeProjectPrioritizer : IPrioritizeProjects
    {
        private bool _hasBeenCalled = false;

        public bool HasBeenCalled { get { return _hasBeenCalled; } }

        #region IPrioritizeProjects Members

        public string[] Prioritize(string[] references)
        {
            _hasBeenCalled = true;
            return references;
        }

        #endregion
    }
}
