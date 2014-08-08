using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Windows.Forms;
using AutoTest.Client.GraphGenerators;
using AutoTest.Client.Logging;
using AutoTest.VM.Messages;
using EnvDTE80;

namespace AutoTest.VS.GraphGenerators
{
    public class DGMLGraphVisualization : IGraphVisualization
    {
        private readonly DTE2 _application;

        public DGMLGraphVisualization(DTE2 application)
        {
            _application = application;
        }

        public void GenerateAndShowGraphFor(VisualGraphGeneratedMessage message)
        {
            try
            {
                var filename = Builddgml(message);
                Open(filename);
            }
            catch (Exception ex)
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

        private void Open(string filename)
        {
            var window = _application.OpenFile(EnvDTE.Constants.vsViewKindDesigner, filename);
            window.Activate();
        }


        private static string Builddgml(VisualGraphGeneratedMessage graph)
        {
            var filename = Path.GetTempFileName() + ".dgml";
            using (var f = File.Open(filename, FileMode.OpenOrCreate))
            {
                f.SetLength(0);
                using (var writer = new StreamWriter(f))
                {
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    writer.WriteLine("<DirectedGraph xmlns=\"http://schemas.microsoft.com/vs/2009/dgml\">");
                    writer.WriteLine("<Nodes>");
                    foreach (var n in graph.Nodes)
                    {
                        writer.WriteLine("<Node Id=\"" + SecurityElement.Escape(n.FullName) + "\" Group=\"" + GetGroup(n) + "\" Label=\"" + SecurityElement.Escape(n.DisplayName) + "\" />");
                    }
                    writer.WriteLine("</Nodes>");
                    writer.WriteLine("<Links>");
                    foreach (var c in graph.Connections)
                    {
                        writer.WriteLine("<Link Source=\"" + SecurityElement.Escape(c.From) + "\" Target=\"" + SecurityElement.Escape(c.To) + "\" Category=\"links\"/>");
                    }
                    writer.WriteLine("</Links>");
                    writer.WriteLine("</DirectedGraph>");
                }
            }
            return filename;
        }

        private static string GetGroup(GraphNode node)
        {
            if (node.IsTest)
            {
                return "Tests";
            }
            if (node.IsRootNode)
            {
                return "Root";
            }
            if (node.IsInterface)
            {
                return "Interface";
            }
            return "Normal";
        }
    }
}