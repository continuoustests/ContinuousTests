using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Test.Core.Caching.Projects.Fakes
{
    class FakeProjectParser : IProjectParser
    {
        private Stack<ProjectDocument> _documents;
        private bool _throwOnParse = false;

        public FakeProjectParser(ProjectDocument[] documents)
        {
            _documents = new Stack<ProjectDocument>(documents);
        }

        #region IProjectParser Members

        public ProjectDocument Parse(string projectFile)
        {
            if (_throwOnParse)
                throw new Exception("Fail encountered");
            return _documents.Pop();
        }

        public ProjectDocument Parse(string projectFile, ProjectDocument existingDocument)
        {
            if (_throwOnParse)
                throw new Exception("Fail encountered");
            return _documents.Pop();
        }

        #endregion

        public void ThrowExceptionOnParse()
        {
            _throwOnParse = true;
        }
    }
}
