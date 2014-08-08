using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging;
using AutoTest.Messages;

namespace AutoTest.Core.Presenters
{
    public interface IInformationFeedbackView
    {
        void RecievingInformationMessage(InformationMessage message);
        void RecievingWarningMessage(WarningMessage message);
        void RevievingErrorMessage(ErrorMessage message);
    }
}
