using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Plugins;
using System.Xml;

namespace AutoTest.TestRunners.Shared.Options
{
    public class OptionsXmlWriter
    {
        private IEnumerable<Plugin> _plugins;
        private RunOptions _options;

        public OptionsXmlWriter(IEnumerable<Plugin> plugins, RunOptions options)
        {
            _plugins = plugins;
            _options = options;
        }

        public void Write(string file)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = Environment.NewLine;
            using (var writer = XmlTextWriter.Create(file, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("run");
                foreach (var plugin in _plugins)
                {
                    writer.WriteStartElement("plugin");
                    writer.WriteAttributeString("type", plugin.Type);
                    writer.WriteString(plugin.Assembly);
                    writer.WriteEndElement();
                }
                foreach (var testRun in _options.TestRuns)
                {
                    writer.WriteStartElement("runner");
                    writer.WriteAttributeString("id", testRun.ID);
                    writeStringList(writer, testRun.Categories, "categories", "ignore_category");
                    foreach (var assembly in testRun.Assemblies)
                    {
                        writer.WriteStartElement("test_assembly");
                        writer.WriteAttributeString("name", assembly.Assembly);
                        if (assembly.IsVerified)
                            writer.WriteAttributeString("verified", "true");
                        writeStringList(writer, assembly.Tests, "tests", "test");
                        writeStringList(writer, assembly.Members, "members", "member");
                        writeStringList(writer, assembly.Namespaces, "namespaces", "namespace");
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private static void writeStringList(XmlWriter writer, IEnumerable<string> list, string mainTag, string nodeTag)
        {
            if (list.Count() > 0)
            {
                writer.WriteStartElement(mainTag);
                foreach (var category in list)
                    writer.WriteElementString(nodeTag, category);
                writer.WriteEndElement();
            }
        }
    }
}
