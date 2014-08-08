using System.Globalization;
using System.IO;
using System.Linq;

using AutoTest.Core.DebugLog;
using AutoTest.Messages;
using System;

namespace AutoTest.Core.TestRunners
{
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

    public class NUnitStackLine : IStackLine
    {
        readonly string _file = "";
        readonly string _line;
        readonly int _lineNumber;
        readonly string _method = "";

        public NUnitStackLine(string line)
        {
            _line = line.Replace(Environment.NewLine, "");
            _method = GetMethod();
            _file = GetFile();
            _lineNumber = GetLineNumber();
        }

        public string Method
        {
            get { return _method; }
        }

        public string File
        {
            get { return _file; }
        }

        public int LineNumber
        {
            get { return _lineNumber; }
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
}