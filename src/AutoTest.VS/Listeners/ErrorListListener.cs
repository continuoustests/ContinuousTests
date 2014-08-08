using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Client.UI;
using AutoTest.VM.Messages;
using EnvDTE80;
using AutoTest.Messages;

namespace AutoTest.VS.Listeners
{
    class ErrorListListener : IMessageListener
    {
        private DTE2 _application;

        public void Connecting(int port, bool startPaused)
        {
        }

        public void Disconnecting(int port)
        {
            // TODO: Clear failing test items from error list
        }

        public void IncomingMessage(object message)
        {
            if (message.GetType().Equals(typeof(CacheMessages)))
            {
                //TODO : Add failing tests to error list window
            }
        }
    }
}
