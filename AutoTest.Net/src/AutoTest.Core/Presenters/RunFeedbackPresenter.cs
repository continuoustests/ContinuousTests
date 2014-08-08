using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging;
using AutoTest.Core.DebugLog;
using AutoTest.Core.Caching.RunResultCache;

namespace AutoTest.Core.Presenters
{
    public class RunFeedbackPresenter : IRunFeedbackPresenter, IDisposable
    {
        private IMessageBus _bus;
        private IMergeRunResults _runResultMerger;
        private IRunFeedbackView _view;

        public IRunFeedbackView View
        {
            get { return _view; }
            set
            {
                _view = value;
                _bus.OnFileChangeMessage += new EventHandler<FileChangeMessageEventArgs>(_bus_OnFileChangeMessage);
                _bus.OnRunStartedMessage += new EventHandler<RunStartedMessageEventArgs>(_bus_OnRunStartedMessage);
                _bus.OnRunFinishedMessage += new EventHandler<RunFinishedMessageEventArgs>(_bus_OnRunFinishedMessage);
                _bus.OnBuildMessage +=new EventHandler<BuildMessageEventArgs>(_bus_OnBuildMessage);
                _bus.OnTestRunMessage += new EventHandler<TestRunMessageEventArgs>(_bus_OnTestRunMessage);
                _bus.OnRunInformationMessage += new EventHandler<RunInformationMessageEventArgs>(_bus_OnRunInformationMessage);
				_bus.OnExternalCommand += new EventHandler<ExternalCommandArgs>(_bus_OnExternalCommandMessage);
                _bus.OnLiveTestFeedback += new EventHandler<LiveTestFeedbackArgs>(_bus_OnLiveTestFeedback);
            }
        }

        public RunFeedbackPresenter(IMessageBus bus, IMergeRunResults runResultMerger)
        {
            _bus = bus;
            _runResultMerger = runResultMerger;
        }

        void _bus_OnFileChangeMessage(object sender, FileChangeMessageEventArgs e)
        {
            Debug.WriteDebug("Presenter recieved " + e.Message.GetType().ToString());
            _view.RecievingFileChangeMessage(e.Message);
        }

        void _bus_OnRunStartedMessage(object sender, RunStartedMessageEventArgs e)
        {
            Debug.PresenterRecievedRunStartedMessage();
            _view.RecievingRunStartedMessage(e.Message);
        }

        void _bus_OnRunFinishedMessage(object sender, RunFinishedMessageEventArgs e)
        {
            Debug.PresenterRecievedRunFinishedMessage();
            _view.RecievingRunFinishedMessage(e.Message);
        }

        void  _bus_OnBuildMessage(object sender, BuildMessageEventArgs e)
        {
            Debug.PresenterRecievedBuildMessage();
            _runResultMerger.Merge(e.Message.Results);
 	        _view.RecievingBuildMessage(e.Message);
        }

        void _bus_OnTestRunMessage(object sender, TestRunMessageEventArgs e)
        {
            Debug.PresenterRecievedTestRunMessage();
            _runResultMerger.Merge(e.Message.Results);
            _view.RecievingTestRunMessage(e.Message);
        }

        void _bus_OnRunInformationMessage(object sender, RunInformationMessageEventArgs e)
        {
            Debug.PresenterRecievedRunInformationMessage();
            _view.RecievingRunInformationMessage(e.Message);
        }
		
		void _bus_OnExternalCommandMessage(object sender, ExternalCommandArgs e)
		{
			Debug.WriteDetail("Presenter received external command message");
			_view.RecievingExternalCommandMessage(e.Message);
		}

        void _bus_OnLiveTestFeedback(object sender, LiveTestFeedbackArgs e)
        {
            Debug.WriteDetail("Presenter received live test feedback message");
            _view.RecievingLiveTestStatusMessage(e.Message);
        }

        #region IDisposable Members

        public void Dispose()
        {
            _bus.OnRunStartedMessage -= _bus_OnRunStartedMessage;
            _bus.OnRunFinishedMessage -= _bus_OnRunFinishedMessage;
            _bus.OnBuildMessage -= _bus_OnBuildMessage;
            _bus.OnTestRunMessage -= _bus_OnTestRunMessage;
			_bus.OnExternalCommand -= _bus_OnExternalCommandMessage;
        }

        #endregion
    }
}
