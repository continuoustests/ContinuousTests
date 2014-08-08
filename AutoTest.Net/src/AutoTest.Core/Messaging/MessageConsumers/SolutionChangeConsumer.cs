using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Crawlers;
using AutoTest.Core.Configuration;
using System.IO;
using AutoTest.Core.DebugLog;
using System.Threading;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    class SolutionChangeConsumer : ISolutionChangeConsumer
    {
        private IConfiguration _configuration;
        private ISolutionParser _crawler;

        public SolutionChangeConsumer(ISolutionParser crawler, IConfiguration configuration)
        {
            _crawler = crawler;
            _configuration = configuration;
        }

        #region ISolutionChangeConsumer Members

        public void Consume(ChangedFile file)
        {
            if (!File.Exists(_configuration.WatchToken))
                return;
            if (file.Extension.ToLower().Equals(".sln"))
            {
                try
                {
                    tryCrawl(file);
                }
                catch (IOException ex)
                {
                    Debug.WriteException(ex);
                    Thread.Sleep(200);
                    try
                    {
                        tryCrawl(file);
                    }
                    catch
                    {
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteException(ex);
                }
            }
        }

        private void tryCrawl(ChangedFile file)
        {
            _crawler.Crawl(file.FullName);
        }

        #endregion
    }
}
