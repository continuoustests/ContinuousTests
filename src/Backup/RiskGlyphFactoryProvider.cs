using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using AutoTest.Client.Logging;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using RiskClassifier;

namespace AutoTest.VS.RiskClassifier
{
    [Export(typeof(IGlyphFactoryProvider))]
    [Name("RiskGlyph")]
    [MarginContainer(PredefinedMarginNames.Glyph)]
    [ContentType("code")]
    [TagType(typeof(RiskTag))]
    [Order(After = "ReSharperVsGlyphFactoryProvider")]
    [Order(After = "PinnedTipGlyphFactoryProvider")]
    [Order(After = "VsTextMarker")]
    internal sealed class RiskGlyphFactoryProvider : IGlyphFactoryProvider
    {
       [ImportMany(typeof(IGlyphFactoryProvider))]
       public List<Lazy< IGlyphFactoryProvider, IOrderable >> GlyphFactories { get; private set; }
    
        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
        {
            if (GlyphFactories != null)
            {
                foreach (var glyph in GlyphFactories)
                {
                    Logger.Write("Found glyph factory of " + glyph.Metadata.Name);

                }
            }
            else
            {
                Logger.Write("glyph factories were null!");
            }
            return new RiskGlyphFactory();
        }
    }
}