using System;
using System.Linq;
using System.Threading;
using AutoTest.VS.Util;
using EnvDTE;
using System.IO;
using AutoTest.VM.Messages;
using AutoTest.Client.Logging;
using AutoTest.Client;
using AutoTest.VS.ClientHandlers;
using AutoTest.Client.UI;
using AutoTest.Messages;
using AutoTest.VS;
using System.Collections.Generic;
using AutoTest.VS.RealtimeChanges;
using AutoTest.VS.Listeners;
using AutoTest.Client.HTTP;
namespace ContinuousTests.VS
{
    public partial class Connect
    {
        private SynchronizationContext _syncContext;
        private static SolutionEvents _solutionEvents;
        private static _dispSolutionEvents_OpenedEventHandler _openedEvent;
        private static _dispSolutionEvents_AfterClosingEventHandler _afterClosingEvent;

        private static BuildEvents _buildEvents;
        private static _dispBuildEvents_OnBuildDoneEventHandler _buildCompletedEvent;

        private static TextEditorEvents _textEditorEvents;
        private static _dispTextEditorEvents_LineChangedEventHandler _lineChangedEvent;
        

        private static readonly ATEClient _client = new ATEClient();
        private static List<KeyValuePair<RealtimeChangeMessage, Document>> _dirtyRealtimeDocuments = new List<KeyValuePair<RealtimeChangeMessage, Document>>();
        private ChangeTracker _changeTracker = new ChangeTracker();

        public static bool IsSolutionOpened;

        public static StartupHandler StartupHandler;

        public static string NUnitTestRunner = "";
        public static string MSTestRunner = "";

        public static AutoTestVSRunInformation RunInformation { get { return _control; } }

        public static string Solution { get; private set; }

        public static CacheTestMessage LastDebugRun { get; set; } // Now we're talking a hot mess

        public static AutoModeDoubleBuildOnDemandHandler DoubleBuildOnDemandHandler = null;

        public static void AddListener(IMessageListener listener)
        {
            StartupHandler.AddListener(listener);
        }

        public static void SendConfigUpdate(ConfigurationUpdateMessage message)
        {
            _client.UpdateConfiguration(message);
        }

        public static void ShowMessageForm()
        {
            _client.ShowMessageForm();
        }

        public static void AbortRun()
        {
            _client.AbortRun();
        }

        public static void SetCustomOutputpath(string path)
        {
            _client.SetCustomOutputpath(path);
        }

        public static string GetAssembly(string projectPath)
        {
            return _client.GetAssemblyFromProject(projectPath);
        }

        private void bindWorkspaceEvents()
        {
            Logger.Write("Binding up Visual Studio events");
            bindSolutionEvents();
            bindEventsOnSolution();
            bindEventsOnBuild();
            bindTextEditorEvents();
            Logger.Write("Visual Studio events bound");
        }

        private void bindEventsOnBuild()
        {
            if (_buildEvents != null)
                return;

            _buildEvents = _applicationObject.Events.BuildEvents;
            _buildCompletedEvent = new _dispBuildEvents_OnBuildDoneEventHandler(BuildEvents_OnBuildDone);
            _buildEvents.OnBuildDone += _buildCompletedEvent;
        }

        private void bindTextEditorEvents()
        {
            if (_textEditorEvents != null)
                return;

            _textEditorEvents = _applicationObject.Events.TextEditorEvents;
            _lineChangedEvent = new _dispTextEditorEvents_LineChangedEventHandler(TextEditorEvents_OnLineChanged);
            _textEditorEvents.LineChanged += _lineChangedEvent;
        }

        private void bindSolutionEvents()
        {
            if (_solutionEvents == null)
                _solutionEvents = _applicationObject.Events.SolutionEvents;
        }

        private void bindEventsOnSolution()
        {
            if (_openedEvent != null)
                return;

            _openedEvent = new _dispSolutionEvents_OpenedEventHandler(onSolutionOpened);
            _solutionEvents.Opened += _openedEvent;

            _afterClosingEvent = new _dispSolutionEvents_AfterClosingEventHandler(onSolutionClosingFinished);
            _solutionEvents.AfterClosing += _afterClosingEvent;
        }

        private void onSolutionOpened()
        {
            Logger.Write("Atempting solution open");
            try
            {
                if (!File.Exists(_applicationObject.Solution.FullName))
                    return;
                Analytics.SendEvent("SolutionOpen");
                Solution = _applicationObject.Solution.FullName;
                _applicationObject.ExecuteCommand("ContinuousTests.VS.Connect.ContinuousTests_FeedbackWindow", "");
                Logger.Write(string.Format("Spawning virtual machine for {0}", _applicationObject.Solution.FullName));
                spawnVM(_applicationObject.Solution.FullName);
                _client.TestProfilerCorrupted();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            IsSolutionOpened = true;
        }

        private void onSolutionClosingFinished()
        {
            try
            {
                Solution = "";
                Analytics.SendEvent("SolutionClosed");
                Logger.Write("Closing solution and disconnecting from the VM");
                if (_control != null)
                    _control.ClearList();
                terminateVM();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            IsSolutionOpened = false;
        }

        void BuildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            var succeeded = _applicationObject.Solution.SolutionBuild.LastBuildInfo == 0;
            if (succeeded)
                _client.RequestManualMinimize();

            _buildRunner.PusblishBuildErrors();
        }

        void TextEditorEvents_OnLineChanged(TextPoint StartPoint, TextPoint EndPoint, int Hint)
        {
            Logger.Write("Line change detected");
            if (!_client.MMConfiguration.RealtimeFeedback || !_client.IsRunning)
                return;

            Logger.Write("Real time changes enabled");
            if (!File.Exists(StartPoint.Parent.Parent.FullName))
                return;

            Logger.Write("Queuing realtime changes");
            ThreadPool.QueueUserWorkItem(queueRealtimeChange, StartPoint);
        }

        void queueRealtimeChange(object state)
        {
            // Wait for VS to actually update the content
            System.Threading.Thread.Sleep(20);

            _syncContext.Post((s) =>
                {
                    var StartPoint = (TextPoint)state;

                    try
                    {
                        Logger.Write("About to lock on text point list");
                        lock (_dirtyRealtimeDocuments)
                        {
                            var project = StartPoint.Parent.Parent.ProjectItem.ContainingProject.FullName;
                            var file = StartPoint.Parent.Parent.FullName;
                            var content = StartPoint.Parent.CreateEditPoint(StartPoint.Parent.StartPoint).GetText(StartPoint.Parent.EndPoint);
                            var document = StartPoint.Parent.Parent;

                            clearOutdatedRealtimeChanges(file);
                            _dirtyRealtimeDocuments.Add(new KeyValuePair<RealtimeChangeMessage, Document>(new RealtimeChangeMessage(project, file, content), document));
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                    }
                }, state);

            _changeTracker.Start(() =>
                {
                    _syncContext.Post((s) =>
                        {
                            lock (_dirtyRealtimeDocuments)
                            {
                                _client.QueueRealtimeChange(
                                    new RealtimeChangeList(_dirtyRealtimeDocuments.GroupBy(x => x.Key).Select(x => x.Key)));
                            }
                        }, null);
                });
        }

        private static void clearOutdatedRealtimeChanges(string file)
        {
            _dirtyRealtimeDocuments
                .RemoveAll(x => isDocumentInvalidOrSaved(x) || x.Key.File.Equals(file));
        }

        public static bool CannotDebug(EnvDTE80.DTE2 app, CacheTestMessage test)
        {
            lock (_dirtyRealtimeDocuments)
            {
                clearOutdatedRealtimeChanges("");
                var inRealtime = _dirtyRealtimeDocuments.Count != 0;
                if (inRealtime)
                {
                    _client.PauseEngine();
                    System.Threading.Thread.Sleep(1000);
                    app.ExecuteCommand("File.SaveAll");
                    _client.ResumeEngine();
                }
            }
            return false;
        }

        private static bool isDocumentInvalidOrSaved(KeyValuePair<RealtimeChangeMessage, Document> x)
        {
            try
            {
                return x.Value == null || x.Value.Saved;
            }
            catch
            {
                return true;
            }
        }

        private void spawnVM(string solutionPath)
        {
            _client.Start(new StartupParams(solutionPath), StartupHandler);
        }

        private void terminateVM()
        {
            _client.Stop();
        }
    }
}
