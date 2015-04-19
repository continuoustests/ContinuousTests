using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using BellyRub;
using BellyRub.UI;
using AutoTest.Messages;
using AutoTest.Core.DebugLog;
using AutoTest.Core.Presenters;
using AutoTest.Server.Handlers;

namespace AutoTest.Server.Communication
{
    class MessageEndpoint : IMessageForwarder, IDisposable
    {
        private string _tokenPath;
        private BellyEngine _engine;
        private Browser _browser;

        private List<IHandler> _handlers = new List<IHandler>();
        private List<IInternalMessageHandler> _internalHandlers = new List<IInternalMessageHandler>();

        public bool IsAlive { get { return _engine.HasConnectedClients; } }

        public MessageEndpoint(string tokenPath, List<IHandler> handlers) {
            _tokenPath = tokenPath;
            _handlers = handlers;
            initializeEngine();
            addHandlers();
        }

        public void Dispose() {
            Stop();
        }

        public void Stop() {
            if (_engine != null) {
                _engine.Stop();
                _engine = null;
            }
        }
        
        public void Forward(object message) {
            Debug.WriteInfo("handling internal message: "+message.GetType().ToString());
            foreach (var handler in _internalHandlers)
                handler.OnInternalMessage(message);
        }

        private void initializeEngine() {
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _engine = new BellyEngine(Path.Combine(assemblyDir, "site"))
                .OnConnected(() => Debug.WriteDebug("Client connected"))
                .OnDisconnected(() => Debug.WriteDebug("Client disconnected"))
                .OnSendException((ex) => Debug.WriteDebug(ex.ToString()))
                .RespondTo("get-token-path", (msg, respondWith) =>
                    respondWith(new { token = _tokenPath })
                );
            _browser = _engine.Start();
            _engine.WaitForFirstClientToConnect();
            _browser.BringToFront();
        }

        private void addHandlers() {
            _handlers.Add(new FocusHandler(_browser));
            foreach (var handler in _handlers) {
                handler.DispatchThrough(send);
                if (handler is IClientHandler) {
                    var handlers = ((IClientHandler)handler).GetClientHandlers();
                    foreach (var key in handlers.Keys)
                        _engine.On(key, handlers[key]);
                }
                if (handler is IClientResponseHandler) {
                    var handlers = ((IClientResponseHandler)handler).GetClientResponseHandlers();
                    foreach (var key in handlers.Keys)
                        _engine.RespondTo(key, handlers[key]);
                }
                if (handler is IInternalMessageHandler)
                    _internalHandlers.Add((IInternalMessageHandler)handler);
            }
        }

        private void send(string subject, object body) {
            if (body == null)
                body = new object();
            Debug.WriteInfo("Sending: "+body.ToString());
            _engine.Send(subject, body);
        }
    }
}
