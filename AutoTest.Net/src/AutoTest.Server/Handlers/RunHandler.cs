using System;
using System.Dynamic;
using System.Collections.Generic;
using AutoTest.Messages;
using AutoTest.Core.Messaging;
using AutoTest.Core.Messaging.MessageConsumers;

namespace AutoTest.Server.Handlers
{
	class RunHandler : IHandler, IClientHandler, IInternalMessageHandler
	{
        private Action<string, object> _dispatch;
        private IMessageBus _bus;
        private RecursiveRunCauseConsumer _recursiveConsumer;

        private string _currentBuildProvider;
        private bool _detectRecursiveRun = false;
        private bool _isDetectingRecursiveRun = false;

        public RunHandler(IMessageBus bus, RecursiveRunCauseConsumer recursiveConsumer) {
            _bus = bus;
            _recursiveConsumer = recursiveConsumer;
        }

        public void DispatchThrough(Action<string, object> dispatcher) {
            _dispatch = dispatcher;
        }

        public Dictionary<string, Action<dynamic>> GetClientHandlers() {
            var handlers = new Dictionary<string, Action<dynamic>>();
            handlers.Add("abort-run", (msg) => {
                _bus.Publish(new AbortMessage(""));    
            });
            handlers.Add("detect-recursion-on-next-run", (msg) => {
                _detectRecursiveRun = true;
            });

            return handlers;
        }

        public void OnInternalMessage(object message) {
            if (message.Is<RunStartedMessage>()) {
                if (_detectRecursiveRun) {
                    _currentBuildProvider = _bus.BuildProvider;
                    _bus.SetBuildProvider("RecursiveRunConsumer");
                    _detectRecursiveRun = false;
                    _isDetectingRecursiveRun = true;
                }
                _dispatch("run-started", new { files = ((RunStartedMessage)message).Files });
            }
            if (message.Is<RunFinishedMessage>()) {
                if (_isDetectingRecursiveRun) {
                    _isDetectingRecursiveRun = false;
                    _bus.SetBuildProvider(_currentBuildProvider);
                    _dispatch("recursive-run-result", new { files = _recursiveConsumer.Files });
                }
                _dispatch("run-finished", null);
            }
        }
	}
}