using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AutoTest.Client.Logging;
using AutoTest.Client.SequenceDiagramGenerators;
using AutoTest.VM.Messages;

namespace AutoTest.VS.SequenceDiagramGenerators
{
    class NotepadDiagramVisualization : ISequenceDiagramVisualization
    {
        public void GenerateAndShowDiagramFor(TestInformationGeneratedMessage message)
        {
            var filename = Path.GetTempFileName() + ".diag";
            File.AppendAllText(filename, BuildStringFor(message));
            Open(filename);
        }

        public string GetCurrentSignature()
        {
            return null;
        }

        public bool WantsRefresh()
        {
            return false;
        }

        private string BuildStringFor(TestInformationGeneratedMessage message)
        {
            var ret = "";
            ret += message.Item + "\n";
            ret += RecurseElements(0, message.Test);
            return ret;
        }


        private static void Open(string filename)
        {
            var notepad = @"C:\windows\system32\notepad.exe";
            //var executable = @"C:\Program Files\Graphviz2.26.3\bin\dotty.exe";
            var info = new ProcessStartInfo(filename);
            info.WindowStyle = ProcessWindowStyle.Maximized;
            Process.Start(info);

        }

        private static string RecurseElements(int depth, Chain test)
        {
            var children = test.Children.Aggregate("", (current, child) => current + RecurseElements(depth + 1, child));
            return new string(' ',depth + 1) + test.DisplayName + (depth != 0 ? (test.TimeEnd - test.TimeStart).ToString("#.000") + " ms here " : "") + "\r\n" + children;
        }
    }
}