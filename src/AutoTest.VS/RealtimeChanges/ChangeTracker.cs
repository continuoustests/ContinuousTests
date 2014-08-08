using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.VM.Messages;
using AutoTest.Core.Messaging;
using AutoTest.Messages;
using System.Threading;
using System.IO;
using AutoTest.Core.Configuration;
using AutoTest.Core.Caching;
using AutoTest.Core.Messaging.MessageConsumers;

namespace AutoTest.VS.RealtimeChanges
{
    class ChangeTracker
    {
        private System.Timers.Timer _timer;
        private Action _onElaps;

        public ChangeTracker()
        {
            _timer = new System.Timers.Timer(400);
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_elapsed);
            _timer.Enabled = true;
            _timer.AutoReset = false;
            _timer.Stop();
        }

        public void Start(Action onElaps)
        {
            _onElaps = onElaps;
            _timer.Stop();
            _timer.Start();
        }

        private void timer_elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _onElaps.Invoke();
            _timer.Stop();
        }
    }
}
