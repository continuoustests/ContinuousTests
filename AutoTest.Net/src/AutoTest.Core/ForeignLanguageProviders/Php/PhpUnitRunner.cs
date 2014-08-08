using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using AutoTest.Messages;
using AutoTest.CoreExtensions;
using AutoTest.Core.Caching.RunResultCache;

namespace AutoTest.Core.ForeignLanguageProviders.Php
{
	public class PhpUnitRunner
    {
        private IRunResultCache _cache; 

        public PhpUnitRunner(IRunResultCache cache) {
            _cache = cache;
        }

        public List<TestRunResults> Run(string arguments, string workingDirectory, string testLocation, Action<string> onLine) {
            var file = Path.GetTempFileName();
            if (File.Exists(file))
                File.Delete(file);
            var results = new List<TestRunResults>();
            var errors = new StringBuilder();
            var lastStatus = DateTime.Now;
            var proc = new Process();
            proc .Query(
                "phpunit",
                "--log-junit \"" + file + "\" --tap " + arguments,
                false,
                workingDirectory,
                (error, line) => {
                    if (error) {
                        errors.AppendLine(line);
                        return;
                    }
                    if (DateTime.Now.Subtract(lastStatus) > TimeSpan.FromMilliseconds(300)) {
                        onLine(line);
                        lastStatus = DateTime.Now;
                    }
                });
            
            if (errors.Length != 0) {
                results
                    .Add(
                        new TestRunResults(
                            "PHP Parse error",
                            testLocation,
                            true,
                            TestRunner.PhpParseError,
                            new[] {
                                PhpUnitParseErrorParser.Parse(errors.ToString())
                            }));
            } else {
                results.Add(removeParseErrorsForLocation(testLocation));
            }
            if (errors.Length == 0 && File.Exists(file)) {
                var testRunResults = JUnitXmlParser.Parse(File.ReadAllText(file), testLocation);
                var finalResults = removeDeletedTests(testRunResults);
                results.AddRange(finalResults);
            }
            return results;
        }

        private TestRunResults removeParseErrorsForLocation(string testLocation) {
            return 
                new TestRunResults(
                    "PHP Parse error",
                    testLocation,
                    true,
                    TestRunner.PhpParseError,
                    _cache
                        .Failed
                        .Where(x => x.Key == testLocation)
                        .Select(x =>
                            new TestResult(
                                x.Value.Runner,
                                TestRunStatus.Passed,
                                x.Value.Name,
                                "",
                                new IStackLine[] {},
                                0))
                    .ToArray());
        }

        private List<TestRunResults> removeDeletedTests(List<TestRunResults> results) {
            var finalResults = new List<TestRunResults>();
            foreach (var result in results) {
                var tests = new List<TestResult>();
                tests.AddRange(result.All);
                var deleted
                    = _cache
                        .Failed
                        .Where(x => 
                            x.Key == result.Assembly &&
                            x.Project == result.Project &&
                            !tests.Any(y => y.Equals(x.Value)))
                        .Select(x =>
                                new TestResult(
                                    x.Value.Runner,
                                    TestRunStatus.Passed,
                                    x.Value.Name,
                                    "",
                                    new IStackLine[] {},
                                    0));
                tests.AddRange(deleted);
                finalResults
                    .Add(
                        new TestRunResults(
                            result.Project,
                            result.Assembly,
                            result.IsPartialTestRun,
                            result.Runner,
                            tests.ToArray()));
            }
            return finalResults;
        }
    }
}