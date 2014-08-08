using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace AutoTest.TestRunners.Shared.Results
{
    public enum TestState
    {
        Passed,
        Ignored,
        Failed,
        Panic
    }

    [Serializable]
    public class TestResult
    {
        private List<StackLine> _stackLines = new List<StackLine>();

        public string Runner { get; set; }
        public string Assembly { get; set; }
        public string TestFixture { get; set; }
        public double DurationInMilliseconds { get; set; }
        public string TestName { get; set; }
        public string TestDisplayName { get; set; }
        public TestState State { get; set; }
        public string Message { get; set; }
        public StackLine[] StackLines { get { return _stackLines.ToArray(); } }

        public TestResult()
        {
            Runner = "";
            Assembly = "";
            TestFixture = "";
            DurationInMilliseconds = 0;
            TestName = "";
            TestDisplayName = null;
            State = TestState.Failed;
            Message = "";
        }

        public TestResult(string runner, string assembly, string fixture, double milliseconds, string testName, TestState state, string message)
        {
            Runner = runner;
            Assembly = assembly;
            TestFixture = fixture;
            DurationInMilliseconds = milliseconds;
            TestName = testName;
            TestDisplayName = null;
            State = state;
            Message = message;
        }

        public TestResult(string runner, string assembly, string fixture, double milliseconds, string testName, string testDisplayName, TestState state, string message)
        {
            Runner = runner;
            Assembly = assembly;
            TestFixture = fixture;
            DurationInMilliseconds = milliseconds;
            TestName = testName;
            TestDisplayName = testDisplayName;
            State = state;
            Message = message;
        }

        public void AddStackLine(StackLine line)
        {
            _stackLines.Add(line);
        }

        public void AddStackLines(StackLine[] lines)
        {
            _stackLines.AddRange(lines);
        }

        public string ToXml()
        {
            return new ResultsXmlWriter(new TestResult[] { this }).GetXml();
        }

        public static TestResult FromXml(string xml)
        {
            using (var stream = new StringReader(xml))
            {
                return new ResultXmlReader(stream).Read().FirstOrDefault();
            }
        }
    }

    [Serializable]
    public class StackLine
    {
        private string _file = "";
        readonly string _line;
        private int _lineNumber;
        private string _method = "";

        public StackLine()
        {
        }

        public StackLine(string line)
        {
            _line = line.Replace(Environment.NewLine, "");
            _method = GetMethod();
            _file = GetFile();
            _lineNumber = GetLineNumber();
        }

        public string Method
        {
            get { return _method; }
            set { _method = value; }
        }

        public string File
        {
            get { return _file; }
            set { _file = value; }
        }

        public int Line
        {
            get { return _lineNumber; }
            set { _lineNumber = value; }
        }

        public override string ToString()
        {
            return _line;
        }

        string GetMethod()
        {
            var end = _line.IndexOf(")");
            if (end < 0)
            {
                return "";
            }
            end += 1;

            var openParen = _line.Substring(0, end).LookBehindIndexOf('(');
            if (openParen < 0)
            {
                return "";
            }

            var start = _line.Substring(0, openParen).LookBehindIndexOf(' ');
            if (start < 0)
            {
                return "";
            }

            if (_line.Substring(0, start).LookBehindIndexOf(' ') == openParen - 1)
            {
                // This is Mono where at format is "at namespace.method ()".
                start = _line.Substring(0, start - 1).LookBehindIndexOf(' ');
                if (start < 0)
                {
                    return "";
                }
            }

            return _line.Substring(start, end - start);
        }

        string GetFile()
        {
            var end = _line.LastIndexOf(":");
            if (end < 0)
            {
                return "";
            }

            var directorySeparator =
                _line.Substring(0, end).LookBehindLastIndexOfAny(new[]
                                                                 {
                                                                     Path.DirectorySeparatorChar,
                                                                     Path.AltDirectorySeparatorChar
                                                                 });
            if (directorySeparator < 0)
            {
                return "";
            }

            var whitespace = _line.Substring(0, directorySeparator).LookBehindIndexOf(' ');
            if (whitespace < 0)
            {
                return "";
            }

            return _line.Substring(whitespace, end - whitespace);
        }

        int GetLineNumber()
        {
            var end = _line.LastIndexOf(":");
            if (end < 0)
            {
                return 0;
            }

            var whitespace = _line.Substring(end).LastIndexOf(' ');
            if (whitespace < 0)
            {
                // This might be a Mono stack line, which does not have whitespace after the ':' separator.
                whitespace = 0;
            }
            whitespace = whitespace + end + 1;

            if (_line.Length < whitespace)
            {
                return 0;
            }

            var lineString = _line.Substring(whitespace);
            int line;
            if (int.TryParse(lineString, NumberStyles.None, CultureInfo.InvariantCulture, out line))
            {
                return line;
            }

            return 0;
        }
    }

    internal static class StringExtensions
    {
        public static int LookBehindIndexOf(this string instance, char value)
        {
            var chars = instance.Reverse().ToArray();
            var reversed = new string(chars);

            var index = reversed.IndexOf(value);
            if (index == -1)
            {
                return -1;
            }

            return instance.Length - index;
        }

        public static int LookBehindLastIndexOfAny(this string instance, char[] value)
        {
            var chars = instance.Reverse().ToArray();
            var reversed = new string(chars);

            var index = reversed.LastIndexOfAny(value);
            if (index == -1)
            {
                return -1;
            }

            return instance.Length - index;
        }
    }
}
