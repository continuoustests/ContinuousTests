using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class MSTestStackLine : IStackLine
    {
        private string _line;
        private string _method = "";
        private string _file = "";
        private int _lineNumber = 0;

        public string Method { get { return _method; } }
        public string File { get { return _file; } }
        public int LineNumber { get { return _lineNumber; } }

        public MSTestStackLine(string line)
        {
            _line = line;
            _method = getMethod();
            _file = getFile();
            _lineNumber = getLineNumber();
        }

        public override string ToString()
        {
            return _line;
        }

        private string getMethod()
        {
            var start = _line.IndexOf("at ");
            if (start < 0)
                return "";
            start += "at ".Length;
            var end = _line.IndexOf(")");
            if (end < 0)
                return "";
            end += 1;
            return _line.Substring(start, end - start);
        }

        private string getFile()
        {
            var start = _line.IndexOf(") in ");
            if (start < 0)
                return "";
            start += ") in ".Length;
            var end = _line.IndexOf(":line");
            if (end < 0)
                return "";
            return _line.Substring(start, end - start);
        }

        private int getLineNumber()
        {
            var start = _line.IndexOf(":line");
            if (start < 0)
                return 0;
            start += ":line".Length;
            if (start >= _line.Length)
                return 0;
            var chunk = _line.Substring(start, _line.Length - start);
            int lineNumber;
            if (int.TryParse(chunk, out lineNumber))
                return lineNumber;
            return 0;
        }
    }
}
