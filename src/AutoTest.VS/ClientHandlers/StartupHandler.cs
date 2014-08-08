using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Client.Handlers;
using AutoTest.VM.Messages;
using AutoTest.Client.Logging;
using System.IO;
using AutoTest.Client.UI;
using EnvDTE80;
using EnvDTE;
using AutoTest.Messages;
using AutoTest.UI;
using ContinuousTests.VS;

namespace AutoTest.VS.ClientHandlers
{
    public class StartupHandler : IStartupHandler
    {
        private static List<IMessageListener> _listeners = new List<IMessageListener>();
        private static DTE2 _application = null;
        private OutputWindowPane _output = null;
        private object _outputLock = new object();

        public static void AddListener(IMessageListener listener)
        {
            _listeners.Add(listener);
        }

        public static void SetApplication(DTE2 application)
        {
            _application = application;
        }

        public void VMStarted(VMSpawnedMessage message)
        {
            Logger.Write("Recieved VMSpawnedMessage");
            Connect.NUnitTestRunner = message.NUnitTestRunner;
            Connect.MSTestRunner = message.MsTestRunner;
        }

        public void Connecting(int port, bool startPaused)
        {
            lock (_listeners)
            {
                foreach (var listener in _listeners)
                    listener.Connecting(port, startPaused);
            }
        }

        public void Disconnecting(int port)
        {
            lock (_listeners)
            {
                foreach (var listener in _listeners)
                    listener.Disconnecting(port);
            }
        }

        public void Consume(object message)
        {
            if (message.GetType().Equals(typeof(RecursiveRunResultMessage)))
                displayResult(((RecursiveRunResultMessage)message).Files);
            if (message.GetType().Equals(typeof(BuildRunMessage)))
                displayBuildRunMessage((BuildRunMessage)message);
            if (message.GetType().Equals(typeof(TestRunMessage)))
                displayTestRunMessage((TestRunMessage)message);
            if (message.GetType().Equals(typeof(FileChangeMessage)))
                displayFileChangeMessage((FileChangeMessage)message);
            if (message.GetType().Equals(typeof(ErrorMessage)))
                lock (_outputLock) { writeToOutput(((ErrorMessage)message).Error); }
            if (message.GetType().Equals(typeof(InformationMessage)))
                lock (_outputLock) { writeToOutput(((InformationMessage)message).Message); }
            if (message.GetType().Equals(typeof(WarningMessage)))
                lock (_outputLock) { writeToOutput(((WarningMessage)message).Warning); }
            notifyListeners(message);
        }

        private void displayFileChangeMessage(FileChangeMessage fileChangeMessage)
        {
            lock (_outputLock)
            {
                foreach (var file in fileChangeMessage.Files)
                    writeToOutput("Found file change: " + file.FullName);
                writeToOutput("");
            }
        }

        private void displayTestRunMessage(TestRunMessage message)
        {
            lock (_outputLock)
            {
                writeToOutput(message.Results.Assembly);
                var status = string.Format("{0} tests, {1} failed, {2} ignored", message.Results.All.Count(), message.Results.Failed.Count(), message.Results.Ignored.Count());
                writeToOutput(status);
                foreach (var test in message.Results.Passed)
                    writeToOutput(string.Format("({0}) {1} {2}", test.Runner, test.DisplayName, test.Status));
                foreach (var test in message.Results.Ignored)
                    writeToOutput(string.Format("({0}) {1} {2}", test.Runner, test.DisplayName, test.Status));
                foreach (var test in message.Results.Failed)
                    writeToOutput(string.Format("({0}) {1} {2}", test.Runner, test.DisplayName, test.Status));
                if (message.Results.All.Count() > 10)
                    writeToOutput(status);
                writeToOutput("");
            }
        }

        private void displayBuildRunMessage(BuildRunMessage message)
        {
            lock (_outputLock)
            {
                writeToOutput(message.Results.Project);
                var status = string.Format("{0} errors, {1} warnings", message.Results.ErrorCount, message.Results.WarningCount);
                writeToOutput(status);
                foreach (var error in message.Results.Errors)
                    writeToOutput(error.ErrorMessage + "\n\t" + error.File + " (" + error.LineNumber.ToString() + "," + error.LinePosition.ToString());
                if (message.Results.Errors.Count() > 10)
                    writeToOutput(status);
                writeToOutput("");
            }
        }

        private void writeToOutput(string text)
        {
            if (_output == null)
            {
                try
                {
                    lock (_application)
                    {
                        var window = _application.ToolWindows.OutputWindow;
                        _output = window.OutputWindowPanes.Add("ContinuousTests");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                    return;
                }
            }
            _output.OutputString(text + Environment.NewLine);
        }

        private static void displayResult(string[] files)
        {
            if (files.Length == 0)
                return;
            var header = "The following files are generated during build or testrun and will trigger new build/test runs." + Environment.NewLine +
                         "To prevent this from happening go into ContinuousTests->Configuration (Local), specify a ignore filename" + Environment.NewLine +
                         "and add patterns that will have the engine ignore changes to these files" + Environment.NewLine;
            var file = Path.GetTempFileName() + ".txt";
            using (var writer = new StreamWriter(file))
            {
                writer.WriteLine(header);
                foreach (var f in files)
                    writer.WriteLine(f);
            }
            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo(file);
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            process.Start();
        }

        private static void notifyListeners(object message)
        {
            lock (_listeners)
            {
                Logger.Write(string.Format("Recieved message of type {0}", message.GetType()));
                foreach (var listener in _listeners)
                {
                    try
                    {
                        listener.IncomingMessage(message);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                    }
                }
            }
        }
    }
}
