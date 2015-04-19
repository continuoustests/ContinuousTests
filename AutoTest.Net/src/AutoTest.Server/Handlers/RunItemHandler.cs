using System;
using System.Linq;
using System.Dynamic;
using System.Drawing;
using System.Collections.Generic;
using AutoTest.Messages;

namespace AutoTest.Server.Handlers
{
	class RunItemHandler : IHandler, IClientHandler, IInternalMessageHandler
	{
        private Action<string, object> _dispatch;
        private long _itemCounter = 0;
        private List<KeyValuePair<long, CacheBuildMessage>> _buildMessages =
            new List<KeyValuePair<long, CacheBuildMessage>>();
        private List<KeyValuePair<long, CacheTestMessage>> _testMessages =
            new List<KeyValuePair<long, CacheTestMessage>>();

        public void DispatchThrough(Action<string, object> dispatcher) {
            _dispatch = dispatcher;
        }

        public Dictionary<string, Action<dynamic>> GetClientHandlers() {
            var handlers = new Dictionary<string, Action<dynamic>>();
            handlers.Add("clear-run-items", (msg) => {
                _buildMessages.Clear();
                _testMessages.Clear();
            });

            return handlers;
        }

        public void OnInternalMessage(object message) {
            if (message.Is<RunStartedMessage>()) {
                clearRunnerTypeAnyItems();
            }
            if (message.Is<BuildRunMessage>()) {
                if (((BuildRunMessage)message).Results.Errors.Length == 0) {
                    var project = ((BuildRunMessage)message).Results.Project; // Make sure no errors remain in log
                    lock (_buildMessages) {
                        var toRemove = new List<KeyValuePair<long, CacheBuildMessage>>();
                        foreach (var item in _buildMessages) {
                            if (project == null || item.Value.Project.Equals(project))
                                toRemove.Add(item);
                        }
                        var ids = new List<long>();
                        foreach (var item in toRemove) {
                            ids.Add(item.Key);
                            _buildMessages.Remove(item);
                        }
                        _dispatch("remove-builditems", new { ids = ids.ToArray() });
                    }
                }
            }
            if (message.Is<CacheMessages>()) {
                var cache = (CacheMessages)message;
                removeItems(cache);
                foreach (var error in cache.ErrorsToAdd)
                    addItem("Build error", formatBuildResult(error), Color.Red, error);
                foreach (var failed in cache.FailedToAdd)
                    addItem("Test failed", formatTestResult(failed), Color.Red, failed);
                foreach (var warning in cache.WarningsToAdd)
                    addItem("Build warning", formatBuildResult(warning), Color.Black, warning);
                foreach (var ignored in cache.IgnoredToAdd)
                    addItem("Test ignored", formatTestResult(ignored), Color.Black, ignored);
            }
            if (message.Is<LiveTestStatusMessage>()) {
                var liveStatus = (LiveTestStatusMessage)message;
                foreach (var test in liveStatus.FailedButNowPassingTests) {
                    var testItem = new CacheTestMessage(test.Assembly, test.Test);
                    removeTestItem((t) => isTheSameTestAs(testItem, t));
                }

                foreach (var test in liveStatus.FailedTests) {
                    var testItem = new CacheTestMessage(test.Assembly, test.Test);
                    removeTestItem((t) => isTheSameTestAs(testItem, t));
                    addItem("Test failed", formatTestResult(testItem), Color.Red, testItem);
                }
            }
        }

        private string formatBuildResult(CacheBuildMessage item) {
            return string.Format("{0}, {1}", item.BuildItem.ErrorMessage, item.BuildItem.File);
        }

        private string formatTestResult(CacheTestMessage item) {
            return string.Format("{0} -> ({1}) {2}", item.Test.Status, item.Test.Runner.ToString(), item.Test.DisplayName);
        }

        private void addItem(string type, string message, Color colour, object tag) {
            if (testExists(tag))
                return;

            var id = getNextId();
            dynamic o = new ExpandoObject();
            o.id = id;
            o.type = type;
            o.message = message;
            o.color = colour.Name;
            if (tag.GetType() == typeof(CacheBuildMessage)) {
                var msg = (CacheBuildMessage)tag;
                lock (_buildMessages) {
                    _buildMessages.Add(new KeyValuePair<long, CacheBuildMessage>(id, msg));
                }
                if (colour.Name == "Red")
                    o.error = msg;
                else
                    o.warning = msg;
            } else {
                var msg = (CacheTestMessage)tag;
                lock (_testMessages) {
                    _testMessages.Add(new KeyValuePair<long, CacheTestMessage>(id, msg));
                }
                if (colour.Name == "Red")
                    o.failed = msg;
                else
                    o.ignored = msg;
            }
            _dispatch("add-item", o);
        }

        private bool testExists(object tag) {
            if (tag.GetType() != typeof(CacheTestMessage))
                return false;
            var test = (CacheTestMessage)tag;
            return itemExists((item) => item.GetType() == typeof(CacheTestMessage) && isTheSameTestAs(test, item as CacheTestMessage));
        }

        private bool itemExists(Func<object, bool> check) {
            foreach (var item in _buildMessages) {
                if (check(item))
                    return true;
            }
            foreach (var item in _testMessages) {
                if (check(item))
                    return true;
            }
            return false;
        }

        private bool isTheSameTestAs(CacheTestMessage original, CacheTestMessage item) {
            return 
                original.Assembly.Equals(item.Assembly) &&
                original.Test.Runner.Equals(item.Test.Runner) &&
                original.Test.Name.Equals(item.Test.Name) &&
                original.Test.DisplayName.Equals(item.Test.DisplayName);
        }

        private void removeItems(CacheMessages cache)
        {
            foreach (var item in cache.ErrorsToRemove)
                removeBuildItem((itm) => itm.Equals(item));
            foreach (var item in cache.WarningsToRemove)
                removeBuildItem((itm) => itm.Equals(item));

            foreach (var item in cache.TestsToRemove)
                removeTestItem((t) => {
                        return
                            t.Assembly.Equals(item.Assembly) &&
                            t.Test.Runner.Equals(item.Test.Runner) &&
                            t.Test.Name.Equals(item.Test.Name);
                    });
        }

        private void clearRunnerTypeAnyItems() {
            removeTestItem((t) => t.Test.Runner == TestRunner.Any);
        }

        private void removeBuildItem(Func<CacheBuildMessage, bool> check) {
            lock (_buildMessages) {
                var items = _buildMessages.Where(x => check(x.Value)).ToArray();
                if (items.Length == 1) {
                    _dispatch("remove-builditem", new { id = items[0].Key });
                    _buildMessages.Remove(items[0]);
                }
            }
        }

        private void removeTestItem(Func<CacheTestMessage, bool> check) {
            lock (_testMessages) {
                var items = _testMessages.Where(x => check(x.Value)).ToArray();
                if (items.Length == 1) {
                    _dispatch("remove-testitem", new { id = items[0].Key });
                    _testMessages.Remove(items[0]);
                }
            }
        }

        private long getNextId() {
            var id = _itemCounter;
            _itemCounter++;
            return id;
        }
	}
}