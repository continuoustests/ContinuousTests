using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using AutoTest.Client.Config;
using AutoTest.Client.Logging;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace AutoTest.VS.RiskClassifier
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(RiskTag))]
    class RiskTaggerProvider : ITaggerProvider
    {
        [Import]
        ITextDocumentFactoryService TextDocumentFactoryService;
        private static readonly Dictionary<string, object> cache = new Dictionary<string, object>();

        public RiskTaggerProvider()
        {
            try
            {
                var config = new MMConfiguration();
                var dte = ServiceProvider.GlobalProvider.GetService(typeof (DTE)) as DTE;
                if (dte == null) return;
                config.Reload(dte.Solution.FileName);
                disabled = config.MinimizerLevel == "off" || !config.RiscEnabled;
            }
            
            catch(Exception ex)
            {
                Logger.Write("RiskTaggerProvider::ctor" + ex);
            }
        }

        [Import]
        internal IClassifierAggregatorService AggregatorService;

        private readonly bool disabled;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (disabled) return null;

            try
            {   
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }
                ITextDocument document;
                if (!TextDocumentFactoryService.TryGetTextDocument(buffer, out document))
                    return
                        new RiskTagger(AggregatorService.GetClassifier(buffer), buffer, TextDocumentFactoryService) as
                        ITagger<T>;
                var name = document.FilePath;
                if (!cache.ContainsKey(name))
                {
                    Logger.Write("Creating a new RiskTagger for " + name);
                    var tagger = new RiskTagger(AggregatorService.GetClassifier(buffer), buffer,
                                                TextDocumentFactoryService);
                    if(tagger.Enabled) 
                        cache.Add(name, tagger);
                }
                object ret;
                cache.TryGetValue(name, out ret);
                return ret as ITagger<T>;
            }
            catch(Exception ex)
            {
                Logger.Write("Exception occured when tagging");
                Logger.Write(ex);
                return null;
            }
        }
    }
}