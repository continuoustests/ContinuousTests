using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Presenters;
using AutoTest.Core.Messaging;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Presenters.Fakes
{
    class FakeInformationFeedbackView : IInformationFeedbackView
    {
        private InformationMessage _informationmessage = null;
        private WarningMessage _warningMessage = null;
        private ErrorMessage _errorMessage = null;

        public string InformationMessage { get { return _informationmessage.Message; } }
        public string WarningMessage { get { return _warningMessage.Warning; } }
        public string ErrorMessage { get { return _errorMessage.Error; } }

        #region IInformationFeedbackView Members

        public void RecievingInformationMessage(InformationMessage message)
        {
            _informationmessage = message;
        }

        public void RecievingWarningMessage(WarningMessage message)
        {
            _warningMessage = message;
        }

        public void RevievingErrorMessage(ErrorMessage message)
        {
            _errorMessage = message;
        }

        #endregion
    }
}
