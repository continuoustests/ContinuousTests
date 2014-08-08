using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Client.UI;
using AutoTest.Client.Config;
using AutoTest.Client;
using System.Threading;
using AutoTest.Messages;

namespace AutoTest.VS.Listeners
{
    public class AutoModeDoubleBuildOnDemandHandler : IMessageListener
    {
        private readonly ATEClient _client;
        private bool _shoudlResume = false;

        public AutoModeDoubleBuildOnDemandHandler(ATEClient client)
        {
            _client = client;
        }

        public void Connecting(int port, bool startPaused)
        {
        }

        public void Disconnecting(int port)
        {
        }

        public void IncomingMessage(object message)
        {
            if (message.GetType().Equals(typeof(RunFinishedMessage)))
            {
                if (_shoudlResume)
                {
                    _shoudlResume = false;
                    _client.ResumeEngine();
                }
            }
        }

        public void PrepareForOnDemandRun()
        {
            _shoudlResume = false;
            if (_client.IsRunning && _client.MMConfiguration.BuildExecutables.Count() == 0)
            {
                _client.PauseEngine();
                Thread.Sleep(100);
                _shoudlResume = true;
            }
        }
    }
}
