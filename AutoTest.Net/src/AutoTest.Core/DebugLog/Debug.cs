using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using AutoTest.Core.Messaging;
using AutoTest.Core.FileSystem;
using AutoTest.Messages;

namespace AutoTest.Core.DebugLog
{
    public static class Debug
    {
        private static bool _isDisabled = true;
		private static IWriteDebugInfo _debugWriter;

        public static bool IsDisabled { get { return _isDisabled; } }

        private static void writeError(string text)
        {
            if (_isDisabled) return;
            _debugWriter.WriteError(text);
        }

        private static void writeInfo(string text)
        {
            if (_isDisabled) return;
            _debugWriter.WriteInfo(text);
        }

        private static void writeDebug(string text)
        {
			if (_isDisabled) return;
            _debugWriter.WriteDebug(text);
        }

        private static void writePreProcessor(string text)
        {
            if (_isDisabled) return;
            _debugWriter.WritePreProcessor(text);
        }

        private static void writeDetail(string text)
        {
            if (_isDisabled) return;
            _debugWriter.WriteDetail(text);
        }

        public static void EnableLogging(IWriteDebugInfo debugWriter)
        {
			_debugWriter = debugWriter;
            _isDisabled = false;
        }

        public static void DisableLogging()
        {
            _isDisabled = true;
        }

        public static void InitialConfigurationFinished()
        {
            writeInfo("Initial configuration finished");
        }

        public static void InitializedCache()
        {
            writeInfo("Cache initialized");
        }

        public static void RegisteredAssembly(Assembly assembly)
        {
            writeDebug(string.Format("Registered assembly {0}", assembly.FullName));
        }

        public static void ShutingDownContainer()
        {
            writeInfo("Shuting down configuration");
        }

        public static void RawFileChangeDetected(string file, WatcherChangeTypes type)
        {
            writeDetail(string.Format("Directory watcher found a change ({1}) in file: {0}", file, type.ToString()));
        }

        public static void AboutToPublishFileChanges(int numberOfFiles)
        {
            writeDebug(string.Format("Directory watcher about to publish change for {0} files", numberOfFiles));
        }

        internal static void Publishing<T>()
        {
            writeDebug(string.Format("Publishing message of type {0}", typeof(T)));
        }

        internal static void WitholdingMessage(object message)
        {
            writeDetail(string.Format("Message bus witheld a message of type {0}", message.GetType()));
        }

        internal static void Blocking<T>()
        {
            writeDetail(string.Format("Message bus started blocking {0}", typeof(T)));
        }

        internal static void ConsumingFileChange(FileChangeMessage message)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Consuming file change for:");
            foreach (var file in message.Files)
                builder.AppendLine(string.Format("    {0}", file.FullName));
            writeDebug(builder.ToString());
        }

        internal static void AboutToPublishProjectChanges(ProjectChangeMessage projectChange)
        {
            writeDetail(string.Format("File change consumer about to publish change for {0} files", projectChange.Files.Length));
        }

        internal static void ConsumingProjectChangeMessage(ProjectChangeMessage message)
        {
            if (_isDisabled) return;
            var builder = new StringBuilder();
            builder.AppendLine("Consuming project changes for:");
            foreach (var file in message.Files)
                builder.AppendLine(string.Format("    {0}", file.FullName));
            writeDebug(builder.ToString());
        }

        internal static void PresenterRecievedRunStartedMessage()
        {
            writeDetail("Presenter received run start message");
        }

        internal static void PresenterRecievedRunFinishedMessage()
        {
            writeDetail("Presenter received run finished message");
        }

        internal static void PresenterRecievedBuildMessage()
        {
            writeDetail("Presenter received build message");
        }

        internal static void PresenterRecievedTestRunMessage()
        {
            writeDetail("Presenter received test run message");
        }

        internal static void PresenterRecievedRunInformationMessage()
        {
            writeDetail("Presenter received run information message");
        }

        internal static void PresenterRecievedInformationMessage()
        {
            writeDetail("Presenter received information message");
        }

        internal static void PresenterRecievedWarningMessage()
        {
            writeDetail("Presenter received warning message");
        }

        internal static void PresenterRecievedErrorMessage()
        {
            writeDetail("Presenter received error message");
        }
    	
		public static void LaunchingEditor(string executable, string arguments)
		{
			writeDetail(string.Format("Launching {0} with {1}", executable, arguments));
		}
    	
		public static void ConfigurationFileMissing()
		{
			writeInfo("The configuration file (AutoTest.config) is missing.");
		}

        public static void FailedToConfigure(Exception ex)
        {
            writeError("Failed to configure application");
            writeException(ex);
        }

        public static void WriteError(string message) { writeError(message); }
        public static void WriteError(string message, params object[] args) { writeError(string.Format(message, args)); }
        public static void WriteInfo(string message) { writeInfo(message); }
        public static void WriteInfo(string message, params object[] args) { writeInfo(string.Format(message, args)); }
        public static void WriteDebug(string message) { writeDebug(message); }
        public static void WriteDebug(string message, params object[] args) { writeDebug(string.Format(message, args)); }
        public static void WritePreProcessor(string message) { writePreProcessor(message); }
        public static void WritePreProcessor(string message, params object[] args) { writePreProcessor(string.Format(message, args)); }
        public static void WriteDetail(string message) { writeDetail(message); }
        public static void WriteDetail(string message, params object[] args) { writeDetail(string.Format(message, args)); }
        public static void WriteException(Exception ex) { writeException(ex); }
		
		public static void ConsumingAssemblyChangeMessage(AssemblyChangeMessage message)
		{
			if (_isDisabled) return;
			var builder = new StringBuilder();
            builder.AppendLine("Consuming assembly changes for:");
            foreach (var file in message.Files)
                builder.AppendLine(string.Format("    {0}", file.FullName));
            writeDebug(builder.ToString());
		}
		
		public static void ChangedBuildProvider(string buildProvider)
		{
			if (_isDisabled) return;
			writeDebug(string.Format("Build provider changed to '{0}'", buildProvider));
		}

        private static void writeException(Exception ex)
        {
            writeError(getExceptionString(ex));
        }

        private static string getExceptionString(Exception ex)
        {
            var text = ex.ToString();
            if (ex.InnerException != null)
                text += Environment.NewLine + "Inner exception:" + Environment.NewLine + getExceptionString(ex.InnerException);
            return text;
        }
    }
}
