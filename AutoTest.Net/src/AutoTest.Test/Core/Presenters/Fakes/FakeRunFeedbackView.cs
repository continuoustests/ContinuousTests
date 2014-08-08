using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Presenters;
using AutoTest.Core.Messaging;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Presenters.Fakes
{
    class FakeRunFeedbackView : IRunFeedbackView
    {
        private FileChangeMessage _fileChangeMessage = null;
        private RunStartedMessage _runStartedMessage = null;
        private RunFinishedMessage _runFinishedMessage = null;
        private BuildRunMessage _buildRunMessage = null;
        private TestRunMessage _testRunMessage = null;
        private RunInformationMessage _runInformationMessage = null;
		private ExternalCommandMessage _externalCommandMessage = null;

        public FileChangeMessage FileChangeMessage { get { return _fileChangeMessage; } }
        public RunStartedMessage RunStartedMessage { get { return _runStartedMessage; } }
        public RunFinishedMessage RunFinishedMessage { get { return _runFinishedMessage; } }
        public BuildRunMessage BuildRunMessage { get { return _buildRunMessage; } }
        public TestRunMessage TestRunMessage { get { return _testRunMessage; } }
        public RunInformationMessage RunInformationMessage { get { return _runInformationMessage; } }
		public ExternalCommandMessage ExternalCommandMessage { get { return _externalCommandMessage; } }
        public LiveTestStatusMessage LiveTestStatusMessage { get; private set; }

        #region IRunFeedbackView Members

        public void RecievingFileChangeMessage(FileChangeMessage message)
        {
            _fileChangeMessage = message;
        }

        public void RecievingBuildMessage(BuildRunMessage runMessage)
        {
            _buildRunMessage = runMessage;
        }

        public void RecievingTestRunMessage(TestRunMessage message)
        {
            _testRunMessage = message;
        }

        public void RecievingRunStartedMessage(RunStartedMessage message)
        {
            _runStartedMessage = message;
        }

        public void RecievingRunFinishedMessage(RunFinishedMessage message)
        {
            _runFinishedMessage = message;
        }

        public void RecievingRunInformationMessage(RunInformationMessage message)
        {
            _runInformationMessage = message;
        }
		
		public void RecievingExternalCommandMessage(ExternalCommandMessage message)
		{
			_externalCommandMessage = message;
		}

        public void RecievingLiveTestStatusMessage(LiveTestStatusMessage message)
        {
            LiveTestStatusMessage = message;
        }

        #endregion
    }
}
