using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert.Infrastructure
{
    internal class NodeFormatter
    {
        const char pipe = '|';
        const char dot = '\'';
        internal static string SimpleFormat(Node constantNode)
        {
            var textLine = new StringBuilder();
            var nodeInfos = new List<NodeInfo>();

            GetSimpleFormatString(constantNode, nodeInfos, textLine);
            return textLine.ToString();
        }
        internal static string[] Format(Node constantNode)
        {
            var textLine = new StringBuilder();
            var nodeInfos = new List<NodeInfo>();

            GetSimpleFormatString(constantNode, nodeInfos, textLine);

            var lines = new List<StringBuilder>();

            var stalks = new List<int>();
            foreach (var info in nodeInfos.OrderBy(x => x.Location))
            {
                var line = new StringBuilder(new string(' ', info.Location));
                stalks.ForEach(x => line[x] = pipe);
                stalks.Add(info.Location);
                line.Append(info.Value);
                lines.Add(line);
            }

            if(nodeInfos.Any())
            {
                for (int i = nodeInfos.Max(x=>x.Depth)-1; i >= 0 ; i--)
                {
                    var line = new StringBuilder(new string(' ', nodeInfos.Max(x=>x.Location)+1));
                    nodeInfos.ForEach(x => line[x.Location] = x.Depth > i ? dot : pipe);
                    lines.Add(line);
                }
            }

            lines.Add(textLine);

            return lines
                .AsEnumerable()
                .Reverse()
                .Select(x => x.ToString().TrimEnd())
                .Where(x=>x.Length > 0)
                .ToArray();
        }

        private static void GetSimpleFormatString(Node constantNode, List<NodeInfo> nodeInfos, StringBuilder textLine)
        {
            constantNode.Walk((text, value, depth) =>
                                  {
                                      if (value != null)
                                      {
                                          nodeInfos.Add(new NodeInfo { Location = textLine.Length, Value = value, Depth=depth });
                                      }
                                      textLine.Append(text);
                                  }, 0);
        }

        class NodeInfo
        {
            public int Location { get; set; }
            public string Value { get; set; }
            public int Depth { get; set; }
        }

    }

}
