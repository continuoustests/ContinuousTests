using System;
using System.Reflection;

namespace celer.Core
{
    public class RunResult
    {
        private readonly MethodInfo _test;
        private readonly bool _wasRun;
        private readonly bool _passed;
        private readonly double _millisecondsSpent;
        private readonly Exception _exception;

        public bool WasRun
        {
            get { return _wasRun; }
        }

        public bool Passed
        {
            get { return _passed; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        public MethodInfo Test
        {
            get { return _test; }
        }

        public double MillisecondsSpent
        {
            get { return _millisecondsSpent; }
        }

        public RunResult(MethodInfo test, bool wasRun, bool passed, Exception exception, double millisecondsSpent)
        {
            _test = test;
            _wasRun = wasRun;
            _passed = passed;
            _exception = exception;
            _millisecondsSpent = millisecondsSpent;
        }
    }
}