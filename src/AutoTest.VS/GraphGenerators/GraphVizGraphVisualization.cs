using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using AutoTest.Client.GraphGenerators;
using AutoTest.Client.Logging;
using AutoTest.VM.Messages;

namespace AutoTest.VS.GraphGenerators
{
    public class GraphVizGraphVisualization : IGraphVisualization
    {
        public void GenerateAndShowGraphFor(VisualGraphGeneratedMessage message)
        {
            try
            {
            var filename = BuildGraphViz(message);
            Open(filename);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public bool WantsRefresh()
        {
            return false;
        }

        public string GetCurrentSignature()
        {
            return null;
        }

        private static void Open(string filename)
        {
                var notepad = @"C:\windows\system32\notepad.exe";
                //var executable = @"C:\Program Files\Graphviz2.26.3\bin\dotty.exe";
                var info = new ProcessStartInfo(filename);
                Logger.Write("built graph opening window.");
                info.WindowStyle = ProcessWindowStyle.Maximized;
                Process.Start(info);
            
        }


        private static string BuildGraphViz(VisualGraphGeneratedMessage graph)
        {
            var filename = Path.GetTempFileName() + ".dot";
            using (var f = File.Open(filename, FileMode.OpenOrCreate))
            {
                f.SetLength(0);
                using (var writer = new StreamWriter(f))
                {
                    writer.WriteLine("digraph d {");
                    writer.WriteLine("\trankdir = LR;");
                    if (graph.Connections == null) MessageBox.Show("connections null");
                    if (graph.Nodes == null) MessageBox.Show("nodes null");
                    foreach (var c in graph.Connections)
                    {
                        writer.WriteLine("\t\"" + c.To + "\" -> \"" + c.From + "\";");
                    }
                    writer.WriteLine();
                    foreach(var n in graph.Nodes)
                    {
                        writer.WriteLine("\t\"" + n.FullName + "\"[label=\"" + n.DisplayName + "\", " + GetShape(n) + "];");
                    }
                    writer.WriteLine("}");
                }
            }
            return filename;
        }

        private static string GetShape(GraphNode node)
        {
            if(node.IsTest)
            {
                return "shape=rectangle,style=\"filled\", fillcolor=yellow";
            }
            if(node.IsRootNode)
            {
                return "shape=rectangle,style=\"filled\" ,fillcolor=green";
            }
            if (node.IsInterface)
            {
                return "shape=rectangle,style=\"filled\",fillcolor=lightblue";
            }
            return "shape=rectangle";
        }
    }
}