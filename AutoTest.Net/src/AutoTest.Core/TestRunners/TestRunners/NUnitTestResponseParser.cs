using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using AutoTest.Core.Messaging;
using System.Xml;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;
using System.IO;
using AutoTest.Core.DebugLog;

namespace AutoTest.Core.TestRunners.TestRunners
{
    public class NUnitTestResponseParser
    {
        private IMessageBus _bus;
        private List<TestResult> _result = new List<TestResult>();
		private List<TestRunResults> _runResults = new List<TestRunResults>();
		private string _content;
		private TestRunInfo[] _testSources;
        private bool _isPartialTestRuns = false;
        private TestRunner _runner;

        public TestRunResults[] Result { get { return _runResults.ToArray(); } }

        public NUnitTestResponseParser(IMessageBus bus, TestRunner runner)
        {
            _runner = runner;
            _bus = bus;
        }

        public void Parse(string content, TestRunInfo[] runInfos, bool isPartialTestRun)
        {
			_content = content;
			_testSources = runInfos;
            _isPartialTestRuns = isPartialTestRun;
			var testSuites = getTestSuites();
			Debug.WriteDetail(string.Format("Found {0} test sections", testSuites.Length));
			foreach (var testSuite in testSuites)
			{
				_result.Clear();
	            string[] testCases = getTestCases(testSuite);
				Debug.WriteDetail(string.Format("Found {0} test cases in section {1}", testCases.Length, getAssemblyName(testSuite)));
	            foreach (var testCase in testCases)
	            {
	                string name = getname(testCase);
	
	                var status = TestRunStatus.Passed;
	                if (testCase.Contains("executed=\"False\""))
	                    status = TestRunStatus.Ignored;
	                else if (testCase.Contains("success=\"False\""))
	                    status = TestRunStatus.Failed;
	
	                string message = "";
	                if (status.Equals(TestRunStatus.Ignored))
	                    message = getMessage(testCase);
	                else if (status.Equals(TestRunStatus.Failed))
	                    message = getMessage(testCase);
	
	                IStackLine[] stackTrace = new IStackLine[] {};
	                if (status.Equals(TestRunStatus.Failed))
	                    stackTrace = getStackTrace(testCase);
	                _result.Add(new TestResult(_runner, status, name, message, stackTrace));
	            }
				var runInfo = matchToTestSource(testSuite);
				if (runInfo ==  null)
				{
					Debug.WriteError("Could not match test suite {0} to any of the tested assemblies", getAssemblyName(testSuite));
					continue;
				}
				var results = getTestResults(runInfo);
				results.SetTimeSpent(getTimeSpent(testSuite));
				_runResults.Add(results);
			}
        }
		
		private TestRunResults getTestResults(TestRunInfo runInfo)
		{
			string project = "";
			if (runInfo.Project != null)
				project = runInfo.Project.Key;
			return new TestRunResults(project, runInfo.Assembly, _isPartialTestRuns, _runner, _result.ToArray());
		}
		
		private TestRunInfo matchToTestSource(string testSuite)
		{
			var assembly = getAssemblyName(testSuite);
			foreach (var source in _testSources)
			{
				if (source.Assembly.Length < assembly.Length)
					continue;
                var toCompare = source.Assembly.Substring(source.Assembly.Length - assembly.Length, assembly.Length);
				if (toCompare.Equals(assembly))
					return source;
                // XUnit screws up the extension casing so we have to do this stupid thing
                var extension = Path.GetExtension(toCompare);
                toCompare = toCompare.Substring(0, toCompare.Length - extension.Length) + extension.ToUpper();
                if (toCompare.Equals(assembly))
                    return source;
			}
			return null;
		}
		
		private string getAssemblyName(string testSuite)
		{
			var start = testSuite.IndexOf("name=\"");
			if (start == -1)
				return "";
			start += "name=\"".Length;
			var end = testSuite.IndexOf("\"", start);
			if (end == -1)
				return "";
			return testSuite.Substring(start, end - start);
		}
		
		private TimeSpan getTimeSpent(string testSuite)
		{
			var start = testSuite.IndexOf("time=\"");
			if (start == -1)
				return new TimeSpan(0);
			start += "time=\"".Length;
			var end = testSuite.IndexOf("\"", start);
			if (end == -1)
				return new TimeSpan(0);
			var time = testSuite.Substring(start, end - start);
			var chunks = time.Split(new char[] { '.' });
			if (chunks.Length != 2)
				return new TimeSpan(0);
			int seconds, milliseconds;
			if (!int.TryParse(chunks[0], out seconds))
				return new TimeSpan(0);
			if (!int.TryParse(chunks[1], out milliseconds))
				return new TimeSpan(0);
			return new TimeSpan(0, 0, 0, seconds, milliseconds);
		}
		
		private string[] getTestSuites()
		{
			var mainTestSuite = getMainTestSuite();
			if (singleAssemblyTestRun())
				return new string[] { mainTestSuite };
			var subSuites = getSubTestSuites(mainTestSuite);
			if (subSuites.Length == 0)
				return new string[] { mainTestSuite };
			return subSuites;
		}
		
		private string getMainTestSuite()
		{
			int start = _content.IndexOf("<test-suite ");
			if (start == -1)
				return "";
			int end = _content.LastIndexOf("</test-suite>");
			if (end == -1)
				return "";
			end += "</test-suite>".Length;
			return _content.Substring(start, end - start);
		}
		
		private bool singleAssemblyTestRun()
		{
			return _content.IndexOf("<test-suite name=\"UNNAMED\"") == -1 &&
				_content.IndexOf("<test-suite type=\"Test Project\" name=\"\"") == -1;
		}
		
		private string[] getSubTestSuites(string mainTestSuite)
		{
			var subSuites = new List<string>();
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(mainTestSuite);
			var nodes = xmlDocument.SelectNodes("test-suite/results/test-suite");
			foreach (XmlNode node in nodes)
				subSuites.Add(node.OuterXml);
			return subSuites.ToArray();
		}

        private string[] getTestCases(string content)
        {
            int start = 0;
            List<string> testCases = new List<string>();
            do
            {
                start = getTestCaseStart(content, start);
                if (start < 0)
                    continue;
                int end = getTestCaseEnd(content, start);
                if (end < 0)
                    break;
                testCases.Add(content.Substring(start, end - start));
                start = end;
            } while (start >= 0);
            return testCases.ToArray();
        }

        private int getTestCaseStart(string content, int start)
        {
            return content.IndexOf("<test-case", start, StringComparison.CurrentCultureIgnoreCase);
        }

        private int getTestCaseEnd(string content, int start)
        {
            var closingBracket = content.IndexOf(">", start);
            int selfClosedEnd = content.IndexOf("/>", start);
            if (selfClosedEnd > closingBracket)
                selfClosedEnd = -1;
            int endTag = content.IndexOf("</test-case>", start, StringComparison.CurrentCultureIgnoreCase);
            if (selfClosedEnd < 0 && endTag < 0)
            {
                _bus.Publish(new WarningMessage(string.Format("Invalid NUnit response format. Could not find <testcase> closing tag for {0}",
                                   content)));
                return -1;
            }

            int end;
            if (selfClosedEnd == -1 || (endTag > 0 && endTag < selfClosedEnd))
                end = endTag + "</test-case>".Length;
            else
                end = selfClosedEnd + "/>".Length;
            return end;
        }

        private string getname(string testCase)
        {
            string tagStart = "name=\"";
            string tagEnd = "\"";
            return getStringContent(testCase, tagStart, tagEnd).Trim();
        }

        private string getMessage(string testCase)
        {
            string tagStart = "<message><![CDATA[";
            string tagEnd = "]]></message>";
            var content = getStringContent(testCase, tagStart, tagEnd).TrimEnd();
            return content;
        }

        private IStackLine[] getStackTrace(string testCase)
        {
            var tagStart = "<stack-trace><![CDATA[";
            var tagEnd = "]]></stack-trace>";
			var content = getStringContent(testCase, tagStart, tagEnd).TrimEnd();
            var separator = getStackSeparator(content);
            var lines = content.Split(new string[]{separator}, StringSplitOptions.RemoveEmptyEntries);
			Debug.WriteDetail("Number of lines: " + lines.Length.ToString());
            List<IStackLine> stackLines = new List<IStackLine>();
            foreach (var line in lines)
			{
				Debug.WriteDetail("Parsing stack line " + line);
                stackLines.Add(new NUnitStackLine(separator + line));
			}
            return stackLines.ToArray();
        }
		
		private string getStackSeparator(string content)
		{
			var start = content.IndexOf(" (");
			if (start == -1)
			{
				start = content.IndexOf("(");
				if (start == -1)
					return Environment.NewLine;
			}
			var prefix = content.Substring(0, start);
			var prefixEnd = prefix.LastIndexOf(" ");
			if (prefixEnd == -1)
				return Environment.NewLine;
			return prefix.Substring(0, prefixEnd + 1);
		}

        private string getStringContent(string testCase, string tagStart, string tagEnd)
        {
            int start = testCase.IndexOf(tagStart, 0, StringComparison.CurrentCultureIgnoreCase) + tagStart.Length;
            if (start < tagStart.Length)
                return "";
            int end = testCase.IndexOf(tagEnd, start, StringComparison.CurrentCultureIgnoreCase);
            if (end < 0)
                return "";
            return testCase.Substring(start, end - start);
        }
    }
}
