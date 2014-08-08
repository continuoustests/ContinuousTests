using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using AutoTest.Messages;

namespace AutoTest.Core.ForeignLanguageProviders.Php
{
	public class JUnitXmlParser
    {
        public static List<TestRunResults> Parse(string xml, string testLocation) {
            try {
                var results = new List<TestRunResults>();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var suites = doc.SelectNodes("testsuites/testsuite");
                handleSuite(suites, results, testLocation);
                return results;
            } catch {
                return new List<TestRunResults>();
            }
        }

        private static void handleSuite(XmlNodeList suites, List<TestRunResults> results, string testLocation) {
            foreach (XmlNode suite in suites) {
                handleSuite(suite.SelectNodes("testsuite"), results, testLocation);
                var tests = getTests(suite);
                if (tests.Count() > 0) {
                    var result 
                        = new TestRunResults(
                            suite.Attributes["name"].Value,
                            testLocation,
                            false,
                            TestRunner.PhpUnit, tests.ToArray());
                    result.SetTimeSpent(TimeSpan.FromMilliseconds(1000*double.Parse(suite.Attributes["time"].Value)));
                    results.Add(result);
                }
            }
        }

        private static IEnumerable<TestResult> getTests(XmlNode suite) {
            var cases = suite.SelectNodes("testcase");
            var tests = new List<TestResult>();
            foreach (XmlNode testcase in cases) {
                var test
                    = new TestResult(
                        TestRunner.PhpUnit,
                        getStateFrom(testcase),
                        testcase.Attributes["class"].Value +"\\" + testcase.Attributes["name"].Value); 
                if (test.Status == TestRunStatus.Failed) {
                    test.Message = getMessage(testcase);
                    test.StackTrace = getStackTrace(testcase, test.Message).ToArray();
                }
                tests.Add(test);
            }
            return tests;
        }

        private static TestRunStatus getStateFrom(XmlNode test) {
            if (!test.HasChildNodes)
                return TestRunStatus.Passed;
            return TestRunStatus.Failed;
        }

        private static string getErrorMessage(XmlNode node) {
             if (node.SelectSingleNode("failure") != null)
                return node.SelectSingleNode("failure").InnerText; 
            if (node.SelectSingleNode("error") != null)
                return node.SelectSingleNode("error").InnerText; 
            return "";
        }

        private static string getMessage(XmlNode node) {
            var msg = getErrorMessage(node);
            msg = trimMessage(msg);
            var end = msg.LastIndexOf(Environment.NewLine + Environment.NewLine);
            if (end == -1)
                return msg;
            return msg.Substring(0, end).Trim(new[] {'\r','\n'});
        }

        private static List<StackLine> getStackTrace(XmlNode node, string message) {
            var stackLines = new List<StackLine>();
            var msg = getErrorMessage(node);
            msg = trimMessage(msg);
            if (msg == message)
                return stackLines;
            var lines
                = msg
                    .Substring(message.Length, msg.Length - message.Length)
                    .Split(new[] { Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines) {
                int lineNumber;
                var colon = line.LastIndexOf(":");
                if (colon > 2) {
                    if (!int.TryParse(line.Substring(colon + 1, line.Length - (colon + 1)), out lineNumber))
                        lineNumber = 0;
                } else
                    lineNumber = 0;
                var file = line;
                if (lineNumber > 0)
                    file = line.Substring(0, colon);
                stackLines.Add(new StackLine() { Method = "", File = file, LineNumber = lineNumber });
            }
            return stackLines;
        }

        private static string trimMessage(string msg) {
            return msg.Trim(new[] {'\r','\n',' ','\t'});
        }

    }

    class StackLine : IStackLine
    {
        public string Method { get; set; }
        public string File { get; set; }
        public int LineNumber { get; set; }
        public string ToString() {
            return File + ":" + LineNumber.ToString();
        }
    }
}