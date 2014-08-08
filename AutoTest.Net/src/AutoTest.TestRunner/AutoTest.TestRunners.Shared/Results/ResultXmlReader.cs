using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using System.Xml;
using AutoTest.TestRunners.Shared.Errors;
using System.IO;

namespace AutoTest.TestRunners.Shared.Results
{
    public class ResultXmlReader
    {
        private string _file = null;
        private TextReader _xml = null;
        private string _readerContent = null;
        private List<TestResult> _results;

        private string _currentRunner = "";
        private string _currentAssembly = "";
        private string _currentFixture = "";
        private TestResult _currentTest;
        private StackLine _currentStackLine;

        public ResultXmlReader(TextReader xml)
        {
            _xml = xml;
            _readerContent = xml.ReadToEnd();
        }

        public ResultXmlReader(string file)
        {
            _file = file;
        }

        public IEnumerable<TestResult> Read()
        {
            _results = new List<TestResult>();
            try
            {
                bool read = true;
                using (var reader = getReader())
                {
                    while (true)
                    {
                        if (read)
                            reader.Read();
                        if (reader.EOF)
                            break;
                        read = true;

                        if (reader.Name.Equals("runner"))
                            getRunner(reader);
                        else if (reader.Name.Equals("assembly"))
                            getAssembly(reader);
                        else if (reader.Name.Equals("fixture"))
                            getFixture(reader);
                        else if (reader.Name.Equals("test"))
                            getTest(reader);
                        else if (reader.Name.Equals("message") && reader.NodeType != XmlNodeType.EndElement)
                        {
                            _currentTest.Message = reader.ReadElementContentAsString();
                            read = false;
                        }
                        else if (reader.Name.Equals("line"))
                            getStackLine(reader);
                        else if (reader.Name.Equals("method") && reader.NodeType != XmlNodeType.EndElement)
                        {
                            _currentStackLine.Method = reader.ReadElementContentAsString();
                            read = false;
                        }
                        else if (reader.Name.Equals("file") && reader.NodeType != XmlNodeType.EndElement)
                        {
                            readFile(reader);
                            read = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _results = new List<TestResult>();
                if (_file != null)
                    _results.Add(ErrorHandler.GetError("Invalid result file" + Environment.NewLine + File.ReadAllText(_file)));
                else
                    _results.Add(ErrorHandler.GetError("Invalid result stream " + Environment.NewLine + _readerContent));
                _results.Add(ErrorHandler.GetError(ex));
            }
            return _results;
        }

        private XmlTextReader getReader()
        {
            if (_xml == null)
                return new XmlTextReader(_file);
            else
                return new XmlTextReader(new StringReader(_readerContent));
        }

        private void readFile(XmlTextReader reader)
        {
            int line;
            if (int.TryParse(reader.GetAttribute("line"), out line))
                _currentStackLine.Line = line;
            _currentStackLine.File = reader.ReadElementContentAsString();
        }

        private void getStackLine(XmlTextReader reader)
        {
            if (reader.NodeType == XmlNodeType.EndElement)
                _currentTest.AddStackLine(_currentStackLine);
            else
                _currentStackLine = new StackLine();
        }

        private void getTest(XmlTextReader reader)
        {
            if (reader.NodeType == XmlNodeType.EndElement)
            {
                _results.Add(_currentTest);
            }
            else
            {
                _currentTest = new TestResult();
                _currentTest.Runner = _currentRunner;
                _currentTest.Assembly = _currentAssembly;
                _currentTest.TestFixture = _currentFixture;
                _currentTest.State = getTestState(reader.GetAttribute("state"));
                _currentTest.DurationInMilliseconds = double.Parse(reader.GetAttribute("duration"));
                _currentTest.TestName = reader.GetAttribute("name");
                _currentTest.TestDisplayName = reader.GetAttribute("displayName");
            }
        }

        private TestState getTestState(string state)
        {
            switch (state.ToLower())
            {
                case "failed":
                    return TestState.Failed;
                case "ignored":
                    return TestState.Ignored;
                case "passed":
                    return TestState.Passed;
                case "panic":
                    return TestState.Panic;
            }
            return TestState.Failed;
        }

        private void getFixture(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return;
            else if (reader.NodeType == XmlNodeType.EndElement)
                _currentFixture = "";
            else
                _currentFixture = reader.GetAttribute("name");
        }

        private void getAssembly(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return;
            else if (reader.NodeType == XmlNodeType.EndElement)
                _currentAssembly = "";
            else
                _currentAssembly = reader.GetAttribute("name");
        }

        private void getRunner(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return;
            else if (reader.NodeType == XmlNodeType.EndElement)
                _currentRunner = "";
            else
                _currentRunner = reader.GetAttribute("id");
        }
    }
}
