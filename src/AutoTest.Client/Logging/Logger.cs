using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Client.Logging
{
    public class Logger
    {
        private static IErrorListener _listener = new FileLogger();

        public static void SetListener(IErrorListener listener)
        {
            _listener = listener;
        }

        public static void Write(string error)
        {
            pushToListener(error);
        }

        public static void Write(Exception ex)
        {
            var message = string.Format("{1}{0}{2}", Environment.NewLine, ex.Message, ex.StackTrace);
            pushToListener(message);
        }

        private static void pushToListener(string message)
        {
            if (_listener == null)
                return;
            _listener.OnError(message);
        }
    }
}
