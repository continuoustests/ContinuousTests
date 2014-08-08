using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Plugins;

namespace AutoTest.TestRunners
{
    interface ITestRunner
    {
        void SetupResolver(bool startLogger);
        IEnumerable<TestResult> Run(Plugin plugin, string id, RunSettings settings);
    }
}