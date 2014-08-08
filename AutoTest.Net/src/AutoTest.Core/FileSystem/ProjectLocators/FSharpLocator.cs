using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.Core.FileSystem.ProjectLocators
{
    public class FSharpLocator : ILocateProjects
    {
        private ICrawlForProjectFiles _filesLocator;

        public FSharpLocator(ICrawlForProjectFiles filesLocator)
        {
            _filesLocator = filesLocator;
        }

        #region ILocateProjects Members

        public ChangedFile[] Locate(string file)
        {
            return _filesLocator.FindParent(Path.GetDirectoryName(file), ".fsproj");
        }

        public bool IsProject(string file)
        {
            return Path.GetExtension(file).ToLower().Equals(".fsproj");
        }

        #endregion
    }
}
