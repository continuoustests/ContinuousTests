using System.Collections.Generic;
using System.Linq;

namespace AutoTest.Core.TestRunners
{
    public class TestRunResults
    {
        private readonly IEnumerable<TestResult> _testResults;

        public TestRunResults(IEnumerable<TestResult> testResults)
        {
            _testResults = testResults;
        }
        public IEnumerable<TestResult> All
        {
            get
            {
                return _testResults;
            }
        }
    }
}