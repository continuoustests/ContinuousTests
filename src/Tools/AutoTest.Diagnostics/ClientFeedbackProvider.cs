using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.VM.Messages.Communication;

namespace AutoTest.Diagnostics
{
    class ClientFeedbackProvider : IClientFeedbackProvider
    {
        public void OnError(string ex)
        {
        }
    }
}
