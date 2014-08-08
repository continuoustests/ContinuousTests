using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging;
using AutoTest.Messages;

namespace AutoTest.Core.Presenters
{
    public interface IRunFeedbackView
    {
        void RecievingFileChangeMessage(FileChangeMessage message);
        void RecievingRunStartedMessage(RunStartedMessage message);
        void RecievingRunFinishedMessage(RunFinishedMessage message);
        void RecievingBuildMessage(BuildRunMessage message);
        void RecievingTestRunMessage(TestRunMessage message);
        void RecievingRunInformationMessage(RunInformationMessage message);
		void RecievingExternalCommandMessage(ExternalCommandMessage message);
        void RecievingLiveTestStatusMessage(LiveTestStatusMessage message);
    }
}
