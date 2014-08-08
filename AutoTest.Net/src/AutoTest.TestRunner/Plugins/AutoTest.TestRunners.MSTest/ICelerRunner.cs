using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Options;

namespace AutoTest.TestRunners.MSTest
{
    interface ICelerRunner
    {
        IEnumerable<TestResult> Run(RunSettings settings);
    }
}
