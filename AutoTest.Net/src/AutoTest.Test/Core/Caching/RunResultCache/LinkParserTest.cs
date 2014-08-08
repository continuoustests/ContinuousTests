using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Caching;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Caching
{
    [TestFixture]
    public class LinkParserTest
    {
        [Test]
        public void Should_replace_link_tags()
        {
            var parser = new LinkParser("some text <<Link>>some more text<</Link>> and yet <<Link>>some more<</Link>>");
            var links = parser.Parse();
            parser.ParsedText.ShouldEqual("some text some more text and yet some more");
			links.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_return_links()
        {
            var parser = new LinkParser("some text <<Link>>some more text<</Link>> and yet <<Link>>some more<</Link>>");
            var links = parser.Parse();
            
            links.Length.ShouldEqual(2);
			parser.ParsedText.Substring(links[0].Start, links[0].Length).ShouldEqual("some more text");
			parser.ParsedText.Substring(links[1].Start, links[1].Length).ShouldEqual("some more");
        }
    }
}
