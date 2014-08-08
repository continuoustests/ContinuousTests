using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.FileSystem;
using AutoTest.Messages;

namespace AutoTest.Test.Core.FileSystem.Fakes
{
    class FakeProjectFileCrawler : ICrawlForProjectFiles
    {
        private ChangedFile[] _projects;
        private string _passedExtension = "";

        public FakeProjectFileCrawler(ChangedFile[] projects)
        {
            _projects = projects;
        }

        public void ShouldHaveBeenAskedToLookFor(string extension)
        {
            extension.ShouldEqual(_passedExtension);
        }

        #region ICrawlForProjectFiles Members

        public ChangedFile[] FindParent(string startDirectory, string fileExtension)
        {
            _passedExtension = fileExtension;
            return _projects;
        }

        #endregion
    }
}
