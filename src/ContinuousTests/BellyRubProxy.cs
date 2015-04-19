using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Client;
using AutoTest.VM.Messages;
using AutoTest.Client.Handlers;
using BellyRub;
using BellyRub.UI;
using ContinuousTests.Handlers;

namespace ContinuousTests
{
    public class BellyRubProxy : IStartupHandler
    {
        private string _token;
        private BellyEngine _engine;
        private Browser _browser;
        private ATEClient _client;
        private Handler[] _handlers;

        public BellyRubProxy(string token, BellyEngine engine, Browser browser)
        {
            _token = token;
            _engine = engine;
            _browser = browser;
            _engine
                .On("engine-pause", m => _client.PauseEngine())
                .On("engine-resume", m => _client.ResumeEngine())
                .On("build-test-all", m => _client.RunAll())
                .On("abort-run", m => _client.AbortRun())
                .On("detect-recursion-on-next-run", m => _client.RunRecursiveRunDetection())
                .On("goto", msg => _client.GoTo(msg.file.ToString(), (int)msg.line, (int)msg.column))
                .RespondTo("get-token-path", (msg, respondWith) =>
                    respondWith(new { token = _token })
                );
            _handlers = new Handler[] {
                new StatusEventHandler(),
                new ShutdownEventHandler(),
                new FocusEventHandler(_browser),
                new RunEventHandler(),
                new RunItemEventHandler()
            };
            foreach (var handler in _handlers)
                handler.DispatchThrough((msg, o) => _engine.Send(msg, o));
        }

        public void SetClient(ATEClient client)
        {
            _client = client;
        }

        public void VMStarted(VMSpawnedMessage message)
        {
        }

        public void Connecting(int port, bool startPaused)
        {
        }

        public void Disconnecting(int port)
        {
        }

        public void Consume(object message)
        {
            Console.WriteLine("Handling "+message.GetType().ToString());
            foreach (var handler in _handlers) {
                handler.OnMessage(message);
            }
        }
    }
}
