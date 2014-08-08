using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging;
using AutoTest.Core.DebugLog;

namespace AutoTest.Core.Presenters
{
    public class InformationFeedbackPresenter : IInformationFeedbackPresenter
    {
        private IMessageBus _bus;
        private IInformationFeedbackView _view;

        public IInformationFeedbackView View
        {
            get { return _view; }
            set
            {
                _view = value;
                _bus.OnInformationMessage += new EventHandler<InformationMessageEventArgs>(_bus_OnInformationMessage);
                _bus.OnWarningMessage += new EventHandler<WarningMessageEventArgs>(_bus_OnWarningMessage);
                _bus.OnErrorMessage += new EventHandler<ErrorMessageEventArgs>(_bus_OnErrorMessage);
            }
        }

        public InformationFeedbackPresenter(IMessageBus bus)
        {
            _bus = bus;
        }

        void _bus_OnInformationMessage(object sender, InformationMessageEventArgs e)
        {
            Debug.PresenterRecievedInformationMessage();
            _view.RecievingInformationMessage(e.Message);
        }

        void _bus_OnWarningMessage(object sender, WarningMessageEventArgs e)
        {
            Debug.PresenterRecievedWarningMessage();
            _view.RecievingWarningMessage(e.Message);
        }

        void _bus_OnErrorMessage(object sender, ErrorMessageEventArgs e)
        {
            Debug.PresenterRecievedErrorMessage();
            _view.RevievingErrorMessage(e.Message);
        }
    }
}
