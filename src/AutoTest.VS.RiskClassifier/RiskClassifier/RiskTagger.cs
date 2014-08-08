using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using AutoTest.Client.Logging;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Classification;

namespace AutoTest.VS.RiskClassifier
{
    public class RiskTagger : ITagger<RiskTag>, IDisposable
    {
        private readonly IClassifier _classifier;
        private readonly ITextBuffer _buffer;
        private readonly ITextDocumentFactoryService _textDocumentFactoryService;
        private readonly DTE _dte;
        private List<Signature> _signatures = new List<Signature>();
        private readonly ITextDocument _document;
        private string _filename;
        private NormalizedSnapshotSpanCollection _spans = new NormalizedSnapshotSpanCollection();
        private readonly SynchronizationContext _syncContext;
        private readonly bool _enabled = true;

        public bool Enabled
        {
            get { return _enabled;  }
        }

        internal RiskTagger(IClassifier classifier, ITextBuffer buffer, ITextDocumentFactoryService textDocumentFactoryService)
        {
            try
            {
                _classifier = classifier;
                _buffer = buffer;
                _textDocumentFactoryService = textDocumentFactoryService;
                _dte = ServiceProvider.GlobalProvider.GetService(typeof (DTE)) as DTE;
                ITextDocument document;
                _syncContext = SynchronizationContext.Current;
                if (!_textDocumentFactoryService.TryGetTextDocument(_buffer, out document))
                    throw new Exception();
                _document = document;
                _filename = document.FilePath.ToLower();
                _enabled = _filename.EndsWith(".vb") || _filename.EndsWith(".cs");
                if (_enabled)
                {
                    _document.FileActionOccurred += _document_FileActionOccurred;
                    _buffer.ChangedLowPriority += buffer_ChangedLowPriority;
                    CodeModelCache.CreateIfNeeded(document, _buffer, _dte);
                    GetSignatures();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("RiskTagger::ctor" + ex);
            }
        }

        void _document_FileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
        {
            try {
            if (e.FileActionType == FileActionTypes.DocumentRenamed)
            {
                var doc = (ITextDocument) sender;
                
                CodeModelCache.CreateIfNeeded((ITextDocument) sender, _buffer, _dte);
                _filename = doc.FilePath.ToLower();
                CodeModelCache.TryUpdateCache(e.FilePath.ToLower(), RaiseTagsChanged, new NormalizedSnapshotSpanCollection());
            }
            }
            catch (Exception ex)
            {
                Logger.Write("DocActionOccurred" + ex);
            }
        }

        private void buffer_ChangedLowPriority(object sender, TextContentChangedEventArgs e)
        {
            try
            {
                if (_document == null) return;
                if (_document.FilePath == null) return;
                CodeModelCache.TryUpdateCache(_document.FilePath.ToLower(), RaiseTagsChanged,
                                              new NormalizedSnapshotSpanCollection());
            }
            catch (Exception ex)
            {
                Logger.Write("BufferChangedLowPriority" + ex);
            }
        }

        private void RaiseTagsChanged(NormalizedSnapshotSpanCollection spans)
        {
            if (TagsChanged == null) return; 
            _syncContext.Post(c =>
                                  {
                                      if (TagsChanged == null) return;
                                      TagsChanged(this,
                                                  new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0,
                                                                                             _buffer.CurrentSnapshot.
                                                                                                 Length)));
                                  }, null);
        }

        private void GetSignatures()
        {
            _signatures = CodeModelCache.GetCodeInfoFor(_document);
        }

        public IEnumerable<ITagSpan<RiskTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var ret = new List<ITagSpan<RiskTag>>();

            try
            {
                GetSignatures();
                var foundLines = new List<int>();
                lock (_spans)
                {
                    _spans = spans;
                }
                foreach (var span in spans)
                {
                    foreach (var classification in _classifier.GetClassificationSpans(span))
                    {
                        var x = classification.Span.Start.GetContainingLine().LineNumber;
                        var currentSig = _signatures.FirstOrDefault(y => y.LineNumber == x);
                        if (currentSig != null && !foundLines.Contains(x))
                        {
                            Logger.Write("Adding tag to " + currentSig.StringSignature + " at " +
                                         currentSig.LineNumber);
                            ret.Add(
                                new TagSpan<RiskTag>(new SnapshotSpan(classification.Span.Start, 1),
                                                     new RiskTag(currentSig)));
                            foundLines.Add(x);
                        }
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                Logger.Write("error occured in tagger");
                Logger.Write(ex);
            }
            return ret;

        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        public void Dispose()
        {
            if(_buffer != null)
                _buffer.ChangedLowPriority -= buffer_ChangedLowPriority;
            if(_document != null)
                _document.FileActionOccurred -= _document_FileActionOccurred;
        }
    }
}