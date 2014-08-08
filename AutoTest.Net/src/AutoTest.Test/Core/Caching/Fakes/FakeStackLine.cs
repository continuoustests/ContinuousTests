using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.TestRunners;
using AutoTest.Messages;

namespace AutoTest.WinForms.Test.ResultsCache.Fakes
{
    class FakeStackLine : IStackLine
    {
        public FakeStackLine(string method, string file, int lineNumber)
        {
            Method = method;
            File = file;
            LineNumber = lineNumber;
        }

        #region IStackLine Members

        public string Method { get; private set; }
        public string File { get; private set; }
        public int LineNumber { get; private set; }

        #endregion
    }
}
