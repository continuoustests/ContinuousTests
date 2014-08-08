using System;
using System.Collections.Generic;
using System.Windows;
using AutoTest.Client.Logging;
using AutoTest.VS.RiskClassifier;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace RiskClassifier
{

    public class RiskGlyphFactory : IGlyphFactory
    { 
        const double _glyphSize = 16.0;
        private static readonly Dictionary<string, MarginUIElement> cache = new Dictionary<string, MarginUIElement>();

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            try
            {
                // Ensure we can draw a glyph for this marker.)
                if (tag == null || !(tag is RiskTag))
                {
                    return null;
                }
                var riskTag = tag as RiskTag;
                MarginUIElement element;
                if (riskTag.Signature.StringSignature == null) return null;
                if (cache.TryGetValue(riskTag.Signature.StringSignature, out element))
                {
                    element.Detach();
                }
                var g = new MarginUIElement(riskTag, line);
                g.Height = _glyphSize;
                g.Width = _glyphSize;
                g.SetUIElements();
                cache.Remove(riskTag.Signature.StringSignature);
                cache.Add(riskTag.Signature.StringSignature, g);
                return g;
            }
            catch (Exception exception)
            {
                Logger.Write("RiskGlyphFactory Exception");
                Logger.Write(exception);
                return null;
            }
        }
    }
}