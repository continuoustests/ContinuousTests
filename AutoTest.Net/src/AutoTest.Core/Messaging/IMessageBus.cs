using System;
using AutoTest.Core.BuildRunners;
using AutoTest.Messages;

namespace AutoTest.Core.Messaging
{
    public class FileChangeMessageEventArgs : EventArgs
    {
        public FileChangeMessage Message { get; private set; }

        public FileChangeMessageEventArgs(FileChangeMessage message)
        {
            Message = message;
        }
    }

    public class RunStartedMessageEventArgs : EventArgs
    {
        public RunStartedMessage Message { get; private set; }

        public RunStartedMessageEventArgs(RunStartedMessage message)
        {
            Message = message;
        }
    }

    public class RunFinishedMessageEventArgs : EventArgs
    {
        public RunFinishedMessage Message { get; private set; }

        public RunFinishedMessageEventArgs(RunFinishedMessage message)
        {
            Message = message;
        }
    }

    public class InformationMessageEventArgs : EventArgs
    {
        public InformationMessage Message { get; private set; }

        public InformationMessageEventArgs(InformationMessage message)
        {
            Message = message;
        }
    }

    public class WarningMessageEventArgs : EventArgs
    {
        public WarningMessage Message { get; private set; }

        public WarningMessageEventArgs(WarningMessage message)
        {
            Message = message;
        }
    }

    public class ErrorMessageEventArgs : EventArgs
    {
        public ErrorMessage Message { get; private set; }

        public ErrorMessageEventArgs(ErrorMessage message)
        {
            Message = message;
        }
    }

    public class BuildMessageEventArgs : EventArgs
    {
        public BuildRunMessage Message { get; private set; }

        public BuildMessageEventArgs(BuildRunMessage runMessage)
        {
            Message = runMessage;
        }
    }

    public class TestRunMessageEventArgs : EventArgs
    {
        public TestRunMessage Message { get; private set; }

        public TestRunMessageEventArgs(TestRunMessage message)
        {
            Message = message;
        }
    }

    public class RunInformationMessageEventArgs : EventArgs
    {
        public RunInformationMessage Message { get; private set; }

        public RunInformationMessageEventArgs(RunInformationMessage message)
        {
            Message = message;
        }
    }
	
	public class ExternalCommandArgs : EventArgs
    {
        public ExternalCommandMessage Message { get; private set; }

        public ExternalCommandArgs(ExternalCommandMessage message)
        {
            Message = message;
        }
    }

    public class LiveTestFeedbackArgs : EventArgs
    {
        public LiveTestStatusMessage Message { get; private set; }

        public LiveTestFeedbackArgs(LiveTestStatusMessage message)
        {
            Message = message;
        }
    }

    public interface IMessageBus
    {
        event EventHandler<FileChangeMessageEventArgs> OnFileChangeMessage;
        event EventHandler<RunStartedMessageEventArgs> OnRunStartedMessage;
        event EventHandler<RunFinishedMessageEventArgs> OnRunFinishedMessage;
        event EventHandler<InformationMessageEventArgs> OnInformationMessage;
        event EventHandler<WarningMessageEventArgs> OnWarningMessage;
        event EventHandler<BuildMessageEventArgs> OnBuildMessage;
        event EventHandler<TestRunMessageEventArgs> OnTestRunMessage;
        event EventHandler<ErrorMessageEventArgs> OnErrorMessage;
        event EventHandler<RunInformationMessageEventArgs> OnRunInformationMessage;
		event EventHandler<ExternalCommandArgs> OnExternalCommand;
        event EventHandler<LiveTestFeedbackArgs> OnLiveTestFeedback;
        void Publish<T>(T message);

        string BuildProvider { get; }
		void SetBuildProvider(string buildProvider);
    }
}