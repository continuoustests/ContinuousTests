using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;

namespace AutoTest.TestRunners.NUnit
{
    class NUnitOptionsParser
    {
        private RunSettings _runnerSettings;
        private List<Options> _options = new List<Options>();

        public IEnumerable<Options> Options { get { return _options; } }

        public NUnitOptionsParser(RunSettings settings)
        {
            _runnerSettings = settings;
        }

        public void Parse()
        {
            var options = new Options(
                    _runnerSettings.Assembly.Assembly,
                    getCategories(),
                    getTests()
                );
            _options.Add(options);
        }

        private string getCategories()
        {
            var categories = "";
            foreach (var category in _runnerSettings.IgnoreCategories)
                categories += categories.Length.Equals(0) ? category : string.Format(",{0}", category);
            return categories;
        }

        private string getTests()
        {
            var tests = "";
            var item = _runnerSettings.Assembly;
            foreach (var test in item.Tests)
                tests += tests.Length.Equals(0) ? test : string.Format(",{0}", test);
            foreach (var member in item.Members)
                tests += tests.Length.Equals(0) ? member : string.Format(",{0}", member);
            foreach (var ns in item.Namespaces)
                tests += tests.Length.Equals(0) ? ns : string.Format(",{0}", ns);
            return tests;
        }
    }

    class Options
    {
        public string Assembly { get; private set; }
        public string Categories { get; private set; }
        public string Tests { get; private set; }

        public Options(string assembly, string categories, string tests)
        {
            Assembly = assembly;
            Categories = categories;
            Tests = tests;
        }
    }
}
