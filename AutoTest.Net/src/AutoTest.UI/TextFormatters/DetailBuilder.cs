using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.UI.TextFormatters
{
    public class DetailBuilder
    {
        public string Text { get; private set; }
        public List<Link> Links { get; private set; }

        public DetailBuilder(CacheBuildMessage message)
        {
            Text = getText(message);
            Links = getLinks();
        }

        public DetailBuilder(CacheTestMessage message)
        {
            Text = getText(message);
            Links = getLinks();
        }

        private string getText(CacheBuildMessage message)
        {
            if (File.Exists(Path.Combine(Path.GetDirectoryName(message.Project), message.BuildItem.File)))
            {
                return string.Format(
                    "Project: {0}{6}{6}" +
                    "File: {4}{1}:line {2}{5}{6}{6}" +
                    "Message:{6}{3}",
                    message.Project,
                    message.BuildItem.File,
                    message.BuildItem.LineNumber,
                    message.BuildItem.ErrorMessage,
                    LinkParser.TAG_START,
                    LinkParser.TAG_END,
                    Environment.NewLine);
            }
            else
            {
                return string.Format(
                    "Project: {0}{4}" +
                    "File: {1}:line {2}{4}" +
                    "Message:{4}{3}",
                    message.Project,
                    message.BuildItem.File,
                    message.BuildItem.LineNumber,
                    message.BuildItem.ErrorMessage,
                    Environment.NewLine);
            }
        }

        private string getText(CacheTestMessage message)
        {
            var stackTrace = new StringBuilder();
            foreach (var line in message.Test.StackTrace)
            {
                if (File.Exists(line.File))
                {
                    stackTrace.AppendLine(string.Format("at {0} in {1}{2}:line {3}{4}",
                                                        line.Method,
                                                        LinkParser.TAG_START,
                                                        line.File,
                                                        line.LineNumber,
                                                        LinkParser.TAG_END));
                }
                else
                {
                    var text = string.Format("at {0} in {1}{2}{3}",
                                                        line.Method,
                                                        LinkParser.TAG_START,
                                                        line.File,
                                                        LinkParser.TAG_END);
                    if (text.Replace("<<Link>>", "").Replace("<</Link>>", "").Trim() != "at  in")
                        stackTrace.AppendLine(text);
                }
            }
            return string.Format(
                "Assembly: {0}{4}" +
                "Test: {1}{4}" +
                "Duration: " + getDuration(message) + "{4}{4}" +
                "Message:{4}{2}{4}{4}" +
                "Stack trace{4}{3}",
                message.Assembly,
                message.Test.DisplayName,
                message.Test.Message,
                stackTrace.ToString(),
                Environment.NewLine);
        }

        private string getDuration(CacheTestMessage message)
        {
            var duration = message.Test.TimeSpent;
            if (duration.TotalHours >= 1)
                return string.Format("{0} hours {1} minutes {2} seconds {3} milliseconds", duration.Hours, duration.Minutes, duration.Seconds, duration.Milliseconds);
            if (duration.TotalMinutes >= 1)
                return string.Format("{0} minutes {1} seconds {2} milliseconds", duration.Minutes, duration.Seconds, duration.Milliseconds);
            if (duration.TotalSeconds >= 1)
                return string.Format("{0} seconds {1} milliseconds", duration.Seconds, duration.Milliseconds);
            return string.Format("{0} milliseconds", duration.Milliseconds);
        }

        private List<Link> getLinks()
        {
            Text = Text.Trim();
            if (Text.EndsWith(Environment.NewLine))
                Text = Text.Substring(0, Text.Length - Environment.NewLine.Length);
            var parser = new LinkParser(Text);
            var links = parser.Parse();
            Text = parser.ParsedText;
            var textForLambda = Text;
            return links
                .Where(x => containsLink(textForLambda.Substring(x.Start, x.Length)))
                .Select(x =>
                new Link(
                    getBeginningOfLine(textForLambda, x.Start),
                    getEndOfLine(textForLambda, x.Start + x.Length),
                    getFile(textForLambda.Substring(x.Start, x.Length)),
                    getLine(textForLambda.Substring(x.Start, x.Length)))).ToList();
        }

        private int getBeginningOfLine(string content, int index)
        {
            var start = content.LastIndexOf(Environment.NewLine, index);
            if (start == -1)
                return 0;
            return start + Environment.NewLine.Length;
        }

        private int getEndOfLine(string content, int index)
        {
            var end = content.IndexOf(Environment.NewLine, index);
            if (end == -1)
                return content.Length - 1;
            return end;
        }

        private bool containsLink(string text)
        {
            return text.IndexOf(":line") != -1;
        }

        private string getFile(string text)
        {
            return text.Substring(0, text.IndexOf(":line"));
        }

        private int getLine(string text)
        {
            var start = text.IndexOf(":line") + ":line".Length;
            return int.Parse(text.Substring(start, text.Length - start));
        }
    }
}
