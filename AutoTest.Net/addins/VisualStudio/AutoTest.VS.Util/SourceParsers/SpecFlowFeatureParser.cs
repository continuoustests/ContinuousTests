using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.VS.Util.SourceParsers
{
    public class SpecFlowFeatureParser
    {
        private string _feature;
        private string _codeBehind;

        public SpecFlowFeatureParser(string feature, string codeBehind)
        {
            _feature = feature;
            _codeBehind = codeBehind;
        }

        public Signature GetTest(int line)
        {
            if (_feature == null || _codeBehind == null)
                return null;

            var scenario_pattern = "Scenario: ";
            var lines = splitToLines(_feature);
            if (trimLine(lines[line]) == "")
                return getTestFromScenario(null);
            for (int i = line; i >= 0; i--)
            {
                var content = trimLine(lines[i]);
                if (content.StartsWith(scenario_pattern))
                    return getTestFromScenario(content.Substring(scenario_pattern.Length, content.Length - scenario_pattern.Length).Trim(new char[] { ' ', '\t' }));
            }
            return null;
        }

        private Signature getTestFromScenario(string scenario)
        {
            var lines = splitToLines(_codeBehind);
            if (lines.Length == 0) return null;

            LineInfo line = null;
            if (scenario == null)
                line = new LineInfo() { Line = lines.Length - 1, Content = lines[lines.Length - 1] };
            else
                line = getFirstLine(lines, string.Format("(\"{0}\")", scenario));

            if (line == null)
                return null;
            var method = getMethod(getFirstLine(lines, "public* void*(", line.Line));
            var cls = getClass(getFirstLineRev(lines, "public * class", line.Line));
            if (cls == null)
                return null;
            var ns = getNamespace(getFirstLineRev(lines, "namespace ", line.Line));
            if (ns == null)
                return null;
            if (method != null)
                return new Signature(SignatureType.Method, string.Format("{0}.{1}.{2}", ns, cls, method));
            else
                return new Signature(SignatureType.Class, string.Format("{0}.{1}", ns, cls));
        }

        private string getNamespace(LineInfo lineInfo)
        {
            var pattern = "namespace ";
            var line = lineInfo.Content;
            var nsStart = line.IndexOf(pattern);
            if (nsStart == -1)
                return null;
            nsStart += pattern.Length;
            return line.Substring(nsStart, line.Length - nsStart).Trim();
        }

        private string getClass(LineInfo lineInfo)
        {
            var line = lineInfo.Content;
            var inheritStart = line.IndexOf(':');
            var genericStart = line.IndexOf('<');
            var newEnd = inheritStart > genericStart ? genericStart : inheritStart;
            if (newEnd > -1)
                line = line.Substring(0, newEnd);
            var pattern = " class ";
            var classStart = line.IndexOf(pattern);
            if (classStart == -1)
                return null;
            classStart += pattern.Length;
            return line.Substring(classStart, line.Length - classStart).Trim();
        }

        private string getMethod(LineInfo lineInfo)
        {
            if (lineInfo == null)
                return null;
            var end = lineInfo.Content.LastIndexOf('(');
            if (end == -1)
                return null;
            var start = lineInfo.Content.LastIndexOf(' ', end);
            if (start == -1)
                return null;
            return lineInfo.Content.Substring(start, end - start).Trim();
        }

        private LineInfo getFirstLineRev(string[] lines, string searchPattern, int fromLine)
        {
            for (int i = fromLine; i >= 0; i--)
            {
                var line = trimLine(lines[i]);
                if (wildCardMatch(searchPattern, line))
                    return new LineInfo() { Line = i, Content = line };
            }
            return null;
        }

        private LineInfo getFirstLine(string[] lines, string searchPattern)
        {
            return getFirstLine(lines, searchPattern, 0);
        }

        private LineInfo getFirstLine(string[] lines, string searchPattern, int fromLine)
        {
            for (int i = fromLine; i < lines.Length; i++)
            {
                var line = trimLine(lines[i]);
                if (wildCardMatch(searchPattern, line))
                    return new LineInfo() { Line = i, Content = line };
            }
            return null;
        }

        private bool wildCardMatch(string searchPattern, string line)
        {
            int start = 0;
            var criterias = searchPattern.Split('*');
            foreach (var criteria in criterias)
            {
                start = line.IndexOf(criteria, start);
                if (start == -1)
                    break;
            }
            return start >= 0;
        }

        private static string trimLine(string line)
        {
            return line.Trim(new char[] { ' ', '\t' });
        }

        private string[] splitToLines(string content)
        {
            return content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }
    }

    class LineInfo
    {
        public int Line { get; set; }
        public string Content { get; set; }
    }

    public class Signature
    {
        public SignatureType Type { get; private set; }
        public string Name { get; private set; }

        public Signature(SignatureType type, string name)
        {
            Type = type;
            Name = name;
        }
    }

    public enum SignatureType
    {
        Class,
        Method
    }
}
