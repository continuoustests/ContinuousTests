using System;
using AutoTest.Core.DebugLog;

namespace AutoTest.VM
{
    class Logger
    {
        private static bool _writeToWriter = false;
        private static IWriteDebugInfo _debugWriter = null;
        private static bool _runAsDebug = false;

        public static void Write(Exception ex)
        {
            WriteError(ex.ToString());
        }
         
        public static void WriteError(string ex)
        {
            if (_runAsDebug)
                Console.WriteLine(ex);
            if (_writeToWriter)
                _debugWriter.WriteError(ex);
        }

        public static void WriteInfo(string ex)
        {
            if (_runAsDebug)
                Console.WriteLine(ex);
            if (_writeToWriter)
                _debugWriter.WriteInfo(ex);
        }

        public static void WriteDebug(string ex)
        {
            if (_runAsDebug)
                Console.WriteLine(ex);
            if (_writeToWriter)
                _debugWriter.WriteDebug(ex);
        }

        public static void WritePreprocessor(string ex)
        {
            if (_runAsDebug)
                Console.WriteLine(ex);
            if (_writeToWriter)
                _debugWriter.WritePreProcessor(ex);
        }

        public static void WriteDetails(string ex)
        {
            if (_runAsDebug)
                Console.WriteLine(ex);
            if (_writeToWriter)
                _debugWriter.WriteDetail(ex);
        }

        public static void SetWriter(IWriteDebugInfo debugWriter)
        {
            _debugWriter = debugWriter;
        }

        public static void EnableWriter()
        {
            _writeToWriter = true;
        }

        public static void SetDebugMode(bool runAsDebug)
        {
            _runAsDebug = runAsDebug;
        }
    }
}
