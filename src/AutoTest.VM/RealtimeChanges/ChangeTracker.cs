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

namespace AutoTest.VM.RealtimeChanges
{
    class ChangeTracker
    {
        private RealtimeChangePreProcessor _preProcessor;
        private IConfiguration _configuration;
        private IMessageBus _bus;
        private IGenerateBuildList _listGenerator;

        private List<RealtimeChangeMessage> _messages = new List<RealtimeChangeMessage>();
        private bool _isRunning = false;

        public ChangeTracker(RealtimeChangePreProcessor preProcessor, IConfiguration configuration, IMessageBus bus, IGenerateBuildList listGenerator)
        {
            _preProcessor = preProcessor;
            _configuration = configuration;
            _bus = bus;
            _listGenerator = listGenerator;
        }

        private void startRun()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                new Thread(run).Start();
            }
            else
            {
                _bus.Publish(new AbortMessage("New realtime change on the way"));
            }
        }

        public void Enqueue(RealtimeChangeList msgs)
        {
            lock (_messages)
            {
                msgs.Messages
                    .ForEach(msg =>
                        {
                            _messages.RemoveAll(x => x.File.Equals(msg.File));
                            _messages.Add(msg);
                        });

                startRun();
            }
        }

        public void Consume()
        {
            Logger.WriteDebug("Consuming run finished message for change tracker");
            int count = 0;
            lock (_messages)
            {
                count = _messages.Count;
            }

            _isRunning = false;
            if (count != 0)
            {
                startRun();
                return;
            }
        }

        private void run()
        {
            try
            {
                var messages = new List<RealtimeChangeMessage>();
                lock (_messages)
                {
                    var relations = _listGenerator.Generate(_messages.GroupBy(x => x.Project).Select(x => x.Key).ToArray());
                    relations
                        .Where(x => _messages.Count(y => y.Project.Equals(x)) == 0).ToList()
                        .ForEach(x => _messages.Add(new RealtimeChangeMessage(x, null, null)));
                    _messages.ForEach(x =>
                        {
                            messages.Add(x);
                        });
                    _messages.Clear();
                }

                var tempFiles = assembleSolution(messages);
                if (tempFiles != null)
                    _preProcessor.Invoke(tempFiles);
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString());
            }
        }
        
        private TempFiles assembleSolution(List<RealtimeChangeMessage> messages)
        {
            try
            {
                return new SolutionAssembler(messages).AssembleTo(_configuration.WatchToken);
            }
            catch 
            {
                try
                {
                    Thread.Sleep(500);
                    return new SolutionAssembler(messages).AssembleTo(_configuration.WatchToken);
                }
                catch 
                {
                }
            }
            return null;
        }
    }
}
