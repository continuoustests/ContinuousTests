using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.FileSystem.ProjectLocators;
using AutoTest.Core.FileSystem;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Messaging.Fakes
{
    class FakeProjectLocator : ILocateProjects
    {
        private ChangedFile[] _files;
        private bool _isProject = false;

        public FakeProjectLocator(ChangedFile[] files)
        {
            _files = files;
        }

        public void WhenAskedIfFileIsProjectReturn(bool isProject)
        {
            _isProject = isProject;
        }

        #region ILocateProjects Members

        public ChangedFile[] Locate(string file)
        {
            return _files;
        }

        public bool IsProject(string file)
        {
            return _isProject;
        }

        #endregion
    }
}
