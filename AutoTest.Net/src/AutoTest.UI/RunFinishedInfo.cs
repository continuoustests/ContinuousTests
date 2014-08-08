using AutoTest.Messages;

namespace AutoTest.UI
{
    public class RunFinishedInfo
    {
        public string Text { get; private set; }
        public bool Succeeded { get; private set; }
        public RunReport Report { get; private set; }

        public RunFinishedInfo(string text, bool succeeded, RunReport report)
        {
            Text = text;
            Succeeded = succeeded;
            Report = report;
        }
    }
}
