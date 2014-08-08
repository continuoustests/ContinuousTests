using System.Diagnostics;
using AutoTest.VM.Messages.Communication;

namespace AutoTest.VS.RiskClassifier
{
    class DebugClientFeedbackProvider : IClientFeedbackProvider
    {
        public void OnError(string ex)
        {
            Debug.WriteLine(ex);
        }
    }
}