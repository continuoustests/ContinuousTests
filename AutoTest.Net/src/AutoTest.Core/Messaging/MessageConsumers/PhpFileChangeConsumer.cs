using System;
using System.Linq;
using System.Collections.Generic;
using AutoTest.Messages;
using AutoTest.Core.Messaging;
using AutoTest.Core.Configuration;
using AutoTest.Core.ForeignLanguageProviders.Php;
using AutoTest.Core.Caching.RunResultCache;

namespace AutoTest.Core.Messaging.MessageConsumers
{
	public class PhpFileChangeConsumer : IOverridingConsumer<FileChangeMessage>, IConsumerOf<AbortMessage>
	{
        private IConfiguration _config;
        private PhpRunHandler _handler;

        public bool IsRunning { get { return _handler.IsRunning; } }

        public PhpFileChangeConsumer(IMessageBus bus, IConfiguration config, ILocateRemovedTests removedTestLocator, IRunResultCache cache) {
            _config = config;
            _handler = new PhpRunHandler(bus, config, removedTestLocator, cache);
        }

		public void Consume(FileChangeMessage message)
        {
            if (_config.Providers != "php")
                return;
            
        	var phpFiles 
        		= message.Files
        			.Where(x => x.Extension.ToLower() == ".php")
                    .Distinct()
        			.ToList();
        	if (phpFiles.Count == 0)
        		return;
            _handler.Handle(phpFiles); 
        }

        public void Consume(AbortMessage message) {
            Terminate();
        }

        public void Terminate() {
            _handler.Abort();
            while (IsRunning) {
                System.Threading.Thread.Sleep(10);
            }
        }
	}
}