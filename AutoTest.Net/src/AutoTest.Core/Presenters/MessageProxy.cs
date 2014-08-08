using System;
using AutoTest.Core.Presenters;
using AutoTest.Messages;
using AutoTest.Core.Caching.RunResultCache;
using AutoTest.Core.Configuration;
using AutoTest.Core.Notifiers;
using System.Collections.Generic;
using AutoTest.Core.Messaging;
using AutoTest.Core.DebugLog;

namespace AutoTest.Core.Presenters
{
    public interface IMessageProxy
    {
        event EventHandler RunStarted;
        event EventHandler RunFinished;
        void SetMessageForwarder(IMessageForwarder forwarder);
    }

    public class MessageProxy : IMessageProxy, IRunFeedbackView, IInformationFeedbackView, IConsumerOf<AbortMessage>
    {
        private readonly IRunFeedbackPresenter _presenter;
        private readonly IInformationFeedbackPresenter _infoPresenter;
        private readonly IConfiguration _configuration;
        private readonly ISendNotifications _notifier;
        private IMessageForwarder _forwarder;

        public event EventHandler RunStarted;
        public event EventHandler RunFinished;

        public MessageProxy(IRunFeedbackPresenter presenter, IInformationFeedbackPresenter infoPresenter, IConfiguration configuration, ISendNotifications notifier)
        {
            _configuration = configuration;
            _notifier = notifier;
            _presenter = presenter;
            _presenter.View = this;
            _infoPresenter = infoPresenter;
            _infoPresenter.View = this;
        }

        public void SetMessageForwarder(IMessageForwarder forwarder)
        {
            _forwarder = forwarder;
        }

        public void RecievingFileChangeMessage(FileChangeMessage message)
        {
            Debug.WriteDebug("handling file changed");
            trySend(message);
        }

        public void RecievingRunStartedMessage(RunStartedMessage message)
        {
            Debug.WriteDebug("handling run started");
            if (RunStarted != null)
                RunStarted(this, new EventArgs());
            trySend(message);
            if (_configuration.NotifyOnRunStarted)
                runNotification("Detected changes, running..", null);
        }

        public void RecievingRunFinishedMessage(RunFinishedMessage message)
        {
            Debug.WriteDebug("handling run finished");
            if (RunFinished != null)
                RunFinished(this, new EventArgs());
            trySend(message);
            if (_configuration.NotifyOnRunCompleted)
                runNotification(getRunFinishedMessage(message.Report), message.Report);
        }

        public void RecievingRunInformationMessage(RunInformationMessage message)
        {
            Debug.WriteDetail("handling run information message");
            trySend(message);
        }

        public void RecievingBuildMessage(BuildRunMessage message)
        {
            Debug.WriteDetail("handling build message");
            trySend(buildCacheMessage());
            trySend(message);
        }

        public void RecievingTestRunMessage(TestRunMessage message)
        {
            Debug.WriteDetail("handling test run");
            trySend(buildCacheMessage());
            trySend(message);
        }

        public void RecievingInformationMessage(InformationMessage message)
        {
            Debug.WriteDetail(string.Format("handling information {0}", message.Message));
            trySend(message);
        }

        public void RecievingWarningMessage(WarningMessage message)
        {
            Debug.WriteDetail(string.Format("handling warning {0}", message.Warning));
            trySend(message);
        }

        public void RevievingErrorMessage(ErrorMessage message)
        {
            Debug.WriteDetail(string.Format("handling error {0}", message.Error));
            trySend(message);
        }

        public void RecievingExternalCommandMessage(ExternalCommandMessage message)
        {
            Debug.WriteDetail(string.Format("handling external command from {0} saying {1}", message.Sender, message.Command));
            trySend(message);
        }

        public void RecievingLiveTestStatusMessage(LiveTestStatusMessage message)
        {
            Debug.WriteDetail("Handling live test status");
            trySend(message);
        }

        public void Consume(AbortMessage message)
        {
            Debug.WriteDetail("handling abort message");
            trySend(message);
        }

        private CacheMessages buildCacheMessage()
        {
            var cache = BootStrapper.Services.Locate<IRunResultCache>();
            var delta = cache.PopDeltas();
            var message = new CacheMessages();
            foreach (var error in delta.AddedErrors)
                message.AddError(new CacheBuildMessage(error.Key, error.Value));
            foreach (var error in delta.RemovedErrors)
                message.RemoveError(new CacheBuildMessage(error.Key, error.Value));

            foreach (var warning in delta.AddedWarnings)
                message.AddWarning(new CacheBuildMessage(warning.Key, warning.Value));
            foreach (var warning in delta.RemovedWarnings)
                message.RemoveWarning(new CacheBuildMessage(warning.Key, warning.Value));

            foreach (var failed in getTests(delta.AddedTests, TestRunStatus.Failed))
                message.AddFailed(new CacheTestMessage(failed.Key, failed.Value));
            foreach (var ignored in getTests(delta.AddedTests, TestRunStatus.Ignored))
                message.AddIgnored(new CacheTestMessage(ignored.Key, ignored.Value));

            foreach (var test in delta.RemovedTests)
                message.RemoveTest(new CacheTestMessage(test.Key, test.Value));

            return message;
        }

        private TestItem[] getTests(TestItem[] testItems, TestRunStatus testRunStatus)
        {
            var items = new List<TestItem>();
            foreach (var item in testItems)
            {
                if (item.Value.Status.Equals(testRunStatus))
                    items.Add(item);
            }
            return items.ToArray();
        }

        protected void trySend(object message)
        {
            try
            {
                _forwarder.Forward(message);
            }
            catch (Exception ex)
            {
                Debug.WriteError(string.Format("Failed sending message of typef {0}", message.GetType()));
                Debug.WriteError(ex.ToString());
            }
        }

        private void runNotification(string msg, RunReport report)
        {
            var notifyType = getNotify(report);
            _notifier.Notify(msg, notifyType);
        }

        private NotificationType getNotify(RunReport report)
        {
            if (report == null)
                return NotificationType.Information;
            if (report.NumberOfBuildsFailed > 0 || report.NumberOfTestsFailed > 0)
                return NotificationType.Red;
            if (report.NumberOfTestsIgnored > 0)
                return NotificationType.Yellow;
            return NotificationType.Green;
        }

        private string getRunFinishedMessage(RunReport report)
        {
            return string.Format(
                        "Ran {0} build(s) ({1} succeeded, {2} failed) and {3} test(s) ({4} passed, {5} failed, {6} ignored)",
                        report.NumberOfProjectsBuilt,
                        report.NumberOfBuildsSucceeded,
                        report.NumberOfBuildsFailed,
                        report.NumberOfTestsRan,
                        report.NumberOfTestsPassed,
                        report.NumberOfTestsFailed,
                        report.NumberOfTestsIgnored);
        }
    }
}
