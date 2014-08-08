using System;
using AutoTest.Core.Configuration;
using System.Threading;
using System.Collections.Generic;
using AutoTest.Core.DebugLog;
using AutoTest.Messages;

namespace AutoTest.Core.Messaging
{
    public class MessageBus : IMessageBus
    {
        private readonly IServiceLocator _services;
		private string _buildProvider = "MSBuild";

        public string BuildProvider { get { return _buildProvider; } }

        public event EventHandler<FileChangeMessageEventArgs> OnFileChangeMessage;
        public event EventHandler<RunStartedMessageEventArgs> OnRunStartedMessage;
        public event EventHandler<RunFinishedMessageEventArgs> OnRunFinishedMessage;
        public event EventHandler<InformationMessageEventArgs> OnInformationMessage;
        public event EventHandler<WarningMessageEventArgs> OnWarningMessage;
        public event EventHandler<BuildMessageEventArgs> OnBuildMessage;
        public event EventHandler<TestRunMessageEventArgs> OnTestRunMessage;
        public event EventHandler<ErrorMessageEventArgs> OnErrorMessage;
        public event EventHandler<RunInformationMessageEventArgs> OnRunInformationMessage;
		public event EventHandler<ExternalCommandArgs> OnExternalCommand;
        public event EventHandler<LiveTestFeedbackArgs> OnLiveTestFeedback;

        public MessageBus(IServiceLocator services)
        {
            _services = services;
        }

        public void Publish<T>(T message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            Debug.Publishing<T>();
            ThreadPool.QueueUserWorkItem(tryPublish<T>, message);
        }
		
		public void SetBuildProvider(string buildProvider)
		{
			_buildProvider = buildProvider;
			Debug.ChangedBuildProvider(_buildProvider);
		}

        private void tryPublish<T>(object threadContext)
        {
            try
            {
                publish<T>(threadContext);
            }
            catch (Exception exception)
            {
                Publish(new ErrorMessage(exception));
            }
        }

        private void publish<T>(object threadContext)
        {
            T message = (T) threadContext;
			// Used to return after this. Silly events ;)
            handleByType<T>(message);

            var consumers = locateConsumers<T>();
            foreach (var instance in consumers)
                instance.Consume(message);

            consumeByOveriddenConsumers<T>(message);
        }

        private void consumeByOveriddenConsumers<T>(T message)
        {
            var overrideConsumers = locateOverrideConsumers<T>();
            foreach (var consumer in overrideConsumers)
            {
                if (consumer.IsRunning)
                    consumer.Terminate();
                consumer.Consume(message);
            }
        }

        private IOverridingConsumer<T>[] locateOverrideConsumers<T>()
        {
            return _services.LocateAll<IOverridingConsumer<T>>();
        }
		
		private IConsumerOf<T>[] locateConsumers<T>()
		{
			if (typeof(T).Equals(typeof(FileChangeMessage)))
				return new IConsumerOf<T>[] { _services.Locate<IConsumerOf<T>>(_buildProvider) };
			return _services.LocateAll<IConsumerOf<T>>();
		}

        private bool handleByType<T>(T message)
        {
            bool handled = false;
            if (typeof(T) == typeof(FileChangeMessage))
            {
                if (OnFileChangeMessage != null)
                    OnFileChangeMessage(this, new FileChangeMessageEventArgs((FileChangeMessage)(IMessage)message));
            }
            if (typeof(T) == typeof(InformationMessage))
            {
                if (OnInformationMessage != null)
                    OnInformationMessage(this, new InformationMessageEventArgs((InformationMessage) (IMessage) message));
            }
            else if (typeof(T) == typeof(BuildRunMessage))
            {
                if (OnBuildMessage != null)
                    OnBuildMessage(this, new BuildMessageEventArgs((BuildRunMessage) (IMessage) message));
            }
            else if (typeof(T) == typeof(TestRunMessage))
            {
                if (OnTestRunMessage != null)
                    OnTestRunMessage(this, new TestRunMessageEventArgs((TestRunMessage)(IMessage)message));
            }
            else if (typeof(T) == typeof(RunStartedMessage))
            {
                if (OnRunStartedMessage != null)
                    OnRunStartedMessage(this, new RunStartedMessageEventArgs((RunStartedMessage)(IMessage)message));
            }
            else if (typeof(T) == typeof(RunFinishedMessage))
            {
                if (OnRunFinishedMessage != null)
                    OnRunFinishedMessage(this, new RunFinishedMessageEventArgs((RunFinishedMessage)(IMessage)message));
            }
            else if (typeof(T) == typeof(WarningMessage))
            {
                if (OnWarningMessage != null)
                    OnWarningMessage(this, new WarningMessageEventArgs((WarningMessage)(IMessage)message));
            }
            else if (typeof(T) == typeof(ErrorMessage))
            {
                if (OnErrorMessage != null)
                    OnErrorMessage(this, new ErrorMessageEventArgs((ErrorMessage)(IMessage)message));
            }
            else if (typeof(T) == typeof(RunInformationMessage))
            {
                if (OnRunInformationMessage != null)
                    OnRunInformationMessage(this, new RunInformationMessageEventArgs((RunInformationMessage)(IMessage)message));
            }
			else if (typeof(T) == typeof(ExternalCommandMessage))
            {
                if (OnExternalCommand != null)
                    OnExternalCommand(this, new ExternalCommandArgs((ExternalCommandMessage)(IMessage)message));
            }
            else if (typeof(T) == typeof(LiveTestStatusMessage))
            {
                if (OnLiveTestFeedback != null)
                    OnLiveTestFeedback(this, new LiveTestFeedbackArgs((LiveTestStatusMessage)(IMessage)message));
            }
            return handled;
        }
    }
}
