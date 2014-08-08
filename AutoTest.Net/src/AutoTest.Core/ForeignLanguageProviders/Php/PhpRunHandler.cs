using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using AutoTest.Messages;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Messaging;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.ForeignLanguageProviders.Php;
using AutoTest.Core.Configuration;
using AutoTest.Core.Caching.RunResultCache;

namespace AutoTest.Core.ForeignLanguageProviders.Php
{
	public class PhpRunHandler
	{
        private IMessageBus _bus;
        private IConfiguration _config;
        private ILocateRemovedTests _removedTestLocator;
        private IRunResultCache _cache; 

        public bool IsRunning { get; private set; }

        public PhpRunHandler(IMessageBus bus, IConfiguration config, ILocateRemovedTests removedTestLocator, IRunResultCache cache)
        {
        	_bus = bus;
            _config = config;
            _removedTestLocator = removedTestLocator;
            _cache = cache;
        }

        public void Handle(List<ChangedFile> files)
        {
        	IsRunning = true;
            _bus.Publish(new RunStartedMessage(files.ToArray()));
            var runReport = new RunReport();

            var configsString = _config.AllSettings("php.phpunit.configs");
            var configs = configsString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var config in configs) {
                var configLocation = _config.AllSettings("php.phpunit." + config + ".configlocation");
                var patterns = _config.AllSettings("php.phpunit." + config + ".Convention.Pattern");
                var testPaths = _config.AllSettings("php.phpunit." + config + ".Convention.TestPaths");

                var testLocations = new List<string>();
                if (patterns.Length == 0 && testPaths.Length == 0) {
                    testLocations.Add("");
                } else {
                    var matcher = new PhpFileConventionMatcher(
                                        _config.WatchPath,
                                        patterns.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries),
                                        testPaths.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries));
                    foreach (var file in files) {
                        foreach (var location in matcher.Match(file.FullName)) {
                            if (!Directory.Exists(location))
                                continue;
                            if (!testLocations.Contains(location))
                                testLocations.Add(location);
                        }
                    }
                }

                if (configLocation != "")
                    configLocation = "-c " + configLocation + " ";

                foreach (var location in testLocations) {
                    var results 
                        = new PhpUnitRunner(_cache)
                        .Run(
                            configLocation + location,
                            _config.WatchPath,
                            location,
                            (line) => {
                                sendLiveFeedback(line);
                            });
                    AutoTest.Core.DebugLog.Debug.WriteDebug("Returned " + results.Count.ToString() + " results");
                    var resultList = new List<TestRunResults>();
                    var runInfos = results.Select(x => new TestRunInfo(new Project(x.Project, null), x.Assembly)).ToArray();
                    resultList.AddRange(results);
                    resultList.AddRange(_removedTestLocator.RemoveUnmatchedRunInfoTests(results.ToArray(), runInfos));
                    foreach (var result in resultList) {
                        AutoTest.Core.DebugLog.Debug.WriteDebug("Result contains " + result.All.Length.ToString() + " tests");
                        runReport.AddTestRun(
                            result.Project,
                            result.Assembly,
                            result.TimeSpent,
                            result.Passed.Length,
                            result.Ignored.Length,
                            result.Failed.Length);
                        _bus.Publish(new TestRunMessage(result));
                    }
                }
            }
            // Oh my god.. please fix this
            // Issue with ordering of TestRunMessage and RunFinishedMessage
            System.Threading.Thread.Sleep(100);
            _bus.Publish(new RunFinishedMessage(runReport));
            IsRunning = false;
        }

        public void Abort() 
        {
        }

        private void sendLiveFeedback(string line) {
            var parser = new PhpUnitLiveParser();
            if (!parser.Parse(line))
                return;
            _bus.Publish(
                new LiveTestStatusMessage(
                    parser.Class,
                    parser.Test,
                    -1,
                    parser.TestsCompleted,
                    new LiveTestStatus[] {},
                    new LiveTestStatus[] {}));
        }
	}
}