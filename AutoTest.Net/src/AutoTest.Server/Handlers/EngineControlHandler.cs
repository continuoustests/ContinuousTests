using System;
using System.Collections.Generic;
using System.Dynamic;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Caching.RunResultCache;

namespace AutoTest.Server.Handlers
{
	class EngineControlHandler : IHandler, IClientHandler, IClientResponseHandler
	{
        private IMergeRunResults _resultCache;
        private IDirectoryWatcher _watcher;

        public EngineControlHandler(IDirectoryWatcher watcher, IMergeRunResults resultCache) {
            _resultCache = resultCache;
            _watcher = watcher;
        }

        public void DispatchThrough(Action<string, object> dispatcher) {
        }

        public Dictionary<string, Action<dynamic>> GetClientHandlers() {
            var handlers = new Dictionary<string, Action<dynamic>>();
            handlers.Add("engine-pause", (msg) => {
                _resultCache.Clear();
                _watcher.Pause();
            });
            handlers.Add("engine-resume", (msg) => {
                _watcher.Resume();
            });

            return handlers;
        }

        public Dictionary<string, Action<dynamic, Action<object>>> GetClientResponseHandlers() {
            var handlers = new Dictionary<string, Action<dynamic, Action<object>>>();
            handlers.Add("get-engine-state", (msg, respondWith) => {
                respondWith(new { state = !_watcher.IsPaused });
            });
            return handlers;
        }
	}
}