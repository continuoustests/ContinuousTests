using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AutoTest.Messages;

namespace AutoTest.UI
{
    public interface IListItemBehaviour
    {
        int Left { get; set; }
        int Width { get; set; }
        string Name { get; }
        bool Visible { get; set; }

    }

    class ListItemBehaviourHandler : IDisposable
    {
        private FeedbackProvider _provider;
        private IListItemBehaviour _cancelRun;
        private IListItemBehaviour _debugTest;
        private IListItemBehaviour _testDetails;
        private IListItemBehaviour _errorDescription;
        private List<KeyValuePair<string, IListItemBehaviour>> _controls = new List<KeyValuePair<string, IListItemBehaviour>>();

        public ListItemBehaviourHandler(
            FeedbackProvider provider,
            IListItemBehaviour cancelRun,
            IListItemBehaviour debugTest,
            IListItemBehaviour testDetails,
            IListItemBehaviour errorDescription)
        {
            _provider = provider;
            _cancelRun = cancelRun;
            _debugTest = debugTest;
            _testDetails = testDetails;
            _errorDescription = errorDescription;
            addControl(_cancelRun);
            if (_provider.CanDebug)
                addControl(_debugTest);
            addControl(_testDetails);
            addControl(_errorDescription);
        }

        private void addControl(IListItemBehaviour control)
        {
            _controls.Add(new KeyValuePair<string, IListItemBehaviour>(control.Name, control));
        }

        public void Organize(object item, bool isRunning)
        {
            if (item == null)
                onNothing(isRunning);
            else if (item.GetType() == typeof(CacheBuildMessage))
                onBuildMessage((CacheBuildMessage)item, isRunning);
            else if (item.GetType() == typeof(CacheTestMessage))
                onTestMessage((CacheTestMessage)item, isRunning);
        }

        private void onBuildMessage(CacheBuildMessage cacheBuildMessage, bool isRunning)
        {
            if (isRunning)
                displayAndOrder(new string[] { _errorDescription.Name, _cancelRun.Name });
            else
                displayAndOrder(new string[] { _errorDescription.Name });
            
        }

        private void onTestMessage(CacheTestMessage cacheTestMessage, bool isRunning)
        {
            var controls = new List<string>();
            controls.Add(_testDetails.Name);
            controls.Add(_debugTest.Name);
            if (isRunning)
                controls.Add(_cancelRun.Name);
            displayAndOrder(controls.ToArray());
        }

        private void onNothing(bool isRunning)
        {
            if (isRunning)
                displayAndOrder(new string[] { _cancelRun.Name });
            else
                displayAndOrder(new string[] { });
        }

        private void displayAndOrder(string[] controlsToShow)
        {
            var nextControlPosition = _provider.Width - 5;
            foreach (var control in _controls)
            {
                if (controlsToShow.Contains(control.Key))
                {
                    var item = control.Value;
                    item.Left = nextControlPosition - item.Width;
                    nextControlPosition = item.Left - 5;
                    item.Visible = true;
                    continue;
                }
                control.Value.Visible = false;
            }
        }

        public void Dispose()
        {
            _cancelRun = null;
            _debugTest = null;
            _testDetails = null;
            _errorDescription = null;
        }
    }
}
