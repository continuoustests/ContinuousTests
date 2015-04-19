using System;
using System.Linq;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using AutoTest.Messages;
using AutoTest.Core.Caching;
using AutoTest.Core.DebugLog;
using AutoTest.Core.Messaging;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.OnDemandTestRun;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Server.Handlers
{
	class TriggerRunHandler : IHandler, IClientHandler, IInternalMessageHandler
	{
        private ICache _cache;
        private IMessageBus _bus;
        private IPreProcessBuildruns[] _buildPreProcessors;
        private bool _disableOnDemandRunnerOnNextRunCompleted = false;

        public TriggerRunHandler(ICache cache, IMessageBus bus, IEnumerable<IPreProcessBuildruns> buildPreProcessors) {
            _cache = cache;
            _bus = bus;
            _buildPreProcessors = buildPreProcessors.ToArray();
        }

        public void DispatchThrough(Action<string, object> dispatcher) {
        }

        public void OnInternalMessage(object message) {
            if (message.Is<RunFinishedMessage>()) {
                if (_disableOnDemandRunnerOnNextRunCompleted)
                    getOnDemandPreProcessor().Deactivate(); 
            }
        }

        public Dictionary<string, Action<dynamic>> GetClientHandlers() {
            var handlers = new Dictionary<string, Action<dynamic>>();
            handlers.Add("build-test-all", (msg) => {
                var message = new ProjectChangeMessage();
                var projects = _cache.GetAll<Project>();
                foreach (var project in projects) {
                    if (project.Value == null)
                        continue;
                    project.Value.RebuildOnNextRun();
                    message.AddFile(new ChangedFile(project.Key));
                }
                _bus.Publish(message);
            });
            handlers.Add("build-test-projects", (msg) => {
                var message = new ProjectChangeMessage();
                var projects = ((IEnumerable<object>)msg.projects).Select(x => x.ToString());
                projects.ToList().ForEach(x => message.AddFile(new ChangedFile(x)));
                _bus.Publish(message);
            });
            handlers.Add("on-demand-test-run", (msg) => {
                var runs = ((IEnumerable<dynamic>)msg.runs)
                        .Select(x => {
                            var run = new OnDemandRun(
                                x.project.ToString(),
                                ((IEnumerable<dynamic>)x.tests).Select(y => y.ToString()).ToArray(),
                                ((IEnumerable<dynamic>)x.members).Select(y => y.ToString()).ToArray(),
                                ((IEnumerable<dynamic>)x.namespaces).Select(y => y.ToString()).ToArray()
                            );
                            if ((bool)x.project_runall_tests == true)
                                run.ShouldRunAllTestsInProject();
                            return run;
                        });

                var message = new ProjectChangeMessage();
                var projects = _cache.GetAll<Project>();
                Debug.WriteDebug(string.Format("Recieved {0} runs", runs.Count()));
                addProjects(runs, message, projects);
                var onDemandPreProcessor = getOnDemandPreProcessor();
                foreach (var run in runs) {
                    Debug.WriteDebug("Adding test run to preprocessor " + run.Project);
                    onDemandPreProcessor.AddRuns(run);
                }
                onDemandPreProcessor.Activate();
                _disableOnDemandRunnerOnNextRunCompleted = true;
                _bus.Publish(message);
            });

            return handlers;
        }

        private OnDemanTestrunPreprocessor getOnDemandPreProcessor() {
            foreach (var preProcessor in _buildPreProcessors) {
                if (preProcessor.GetType() == typeof(OnDemanTestrunPreprocessor))
                    return (OnDemanTestrunPreprocessor)preProcessor;
            }
            return null;
        }

        private void addProjects(IEnumerable<OnDemandRun> runs, ProjectChangeMessage message, Project[] projects) {
            foreach (var run in runs) {
                var project = projects.FirstOrDefault(x => x.Key.Equals(run.Project));
                if (project == null) {
                    Debug.WriteError(string.Format("Did not find matching project for run {0}", run.Project));
                    continue;
                }
                message.AddFile(new ChangedFile(run.Project));
            }
        }

	}
}