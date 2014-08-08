using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using System.IO;
using System.Xml;
using AutoTest.TestRunners.Shared.Plugins;

namespace AutoTest.TestRunners.Shared.Options
{
    public class OptionsXmlReader
    {
        private string _file;
        private List<Plugin> _plugins = new List<Plugin>();
        private RunOptions _options = null;
        private XmlDocument _xml = null;

        private RunnerOptions _currentRunner = null;
        private AssemblyOptions _currentAssembly = null;

        public IEnumerable<Plugin> Plugins { get { return _plugins; } }
        public RunOptions Options { get { return _options; } }
        public bool IsValid { get; private set; }

        public OptionsXmlReader(string file)
        {
            _file = file;
            IsValid = false;
        }

        public void Parse()
        {
            if (!File.Exists(_file))
            {
                _plugins = null;
                return;
            }

            _options = new RunOptions();
            using (var reader = new XmlTextReader(_file))
            {
                while (reader.Read())
                {
                    if (reader.Name.Equals("plugin") && reader.NodeType != XmlNodeType.EndElement)
                        getPlugin(reader);
                    else if (reader.Name.Equals("runner"))
                        getRunner(reader);
                    else if (reader.Name.Equals("ignore_category") && reader.NodeType != XmlNodeType.EndElement)
                        getCategory(reader);
                    else if (reader.Name.Equals("test_assembly"))
                        getAssembly(reader);
                    else if (reader.Name.Equals("test") && reader.NodeType != XmlNodeType.EndElement)
                        getTest(reader);
                    else if (reader.Name.Equals("member") && reader.NodeType != XmlNodeType.EndElement)
                        getMember(reader);
                    else if (reader.Name.Equals("namespace") && reader.NodeType != XmlNodeType.EndElement)
                        getNamespace(reader);
                }
            }
            IsValid = true;
        }

        private void getNamespace(XmlTextReader reader)
        {
            reader.Read();
            if (!_currentAssembly.Namespaces.Contains(reader.Value))
                _currentAssembly.AddNamespace(reader.Value);
        }

        private void getMember(XmlTextReader reader)
        {
            reader.Read();
            if (!_currentAssembly.Members.Contains(reader.Value))
                _currentAssembly.AddMember(reader.Value);
        }

        private void getAssembly(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                _currentRunner.AddAssembly(new AssemblyOptions(reader.GetAttribute("name")).HasBeenVerified(reader.GetAttribute("verified") == "true"));
            else if (reader.NodeType == XmlNodeType.EndElement)
                _currentRunner.AddAssembly(_currentAssembly);
            else
                _currentAssembly = new AssemblyOptions(reader.GetAttribute("name")).HasBeenVerified(reader.GetAttribute("verified") == "true");
        }

        private void getTest(XmlTextReader reader)
        {
            reader.Read();
            if (!_currentAssembly.Tests.Contains(reader.Value))
                _currentAssembly.AddTest(reader.Value);
        }

        private void getCategory(XmlTextReader reader)
        {
            reader.Read();
            if (!_currentRunner.Categories.Contains(reader.Value))
                _currentRunner.AddCategory(reader.Value);
        }

        private void getRunner(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
            {
                var id = reader.GetAttribute("id");
                if (_options.TestRuns.Count(x => x.ID.Equals(id)) == 0)
                    _options.AddTestRun(new RunnerOptions(id));
            }
            else if (reader.NodeType == XmlNodeType.EndElement)
            {
                if (_options.TestRuns.Count(x => x.ID.Equals(_currentRunner.ID)) == 0)
                    _options.AddTestRun(_currentRunner);
            }
            else
            {
                var id = reader.GetAttribute("id");
                _currentRunner = _options.TestRuns.FirstOrDefault(x => x.ID.Equals(id));
                if (_currentRunner == null)
                    _currentRunner = new RunnerOptions(id);
            }
        }

        private void getPlugin(XmlTextReader reader)
        {
            var type = reader.GetAttribute("type");
            reader.Read();
            var assembly = reader.Value;
            if (_plugins.FirstOrDefault(x => x.Assembly.Equals(assembly) && x.Type.Equals(type)) == null)
                _plugins.Add(new Plugin(assembly, type));
        }

        private void openDocument()
        {
            try
            {
                _xml = new XmlDocument();
                _xml.Load(_file);
            }
            catch (Exception ex)
            {
                _xml = null;
                Console.WriteLine("Could not load run options: \"{0}\"", ex.Message);
            }
        }

        //private void parsePlugins()
        //{
        //    var nodes = _xml.SelectNodes("run/plugin");
        //    foreach (XmlNode node in nodes)
        //    {
        //        XmlNode typeNode = node.Attributes.GetNamedItem("type");
        //        string type = null;
        //        if (typeNode != null)
        //            type = typeNode.InnerXml;
        //        var assembly = node.InnerXml;
        //        _plugins.Add(new Plugin(assembly, type));
        //    }
        //}

        //private void parseOptions()
        //{
        //    _options = new RunOptions();
        //    var nodes = _xml.SelectNodes("run/runner");
        //    foreach (XmlNode node in nodes)
        //    {
        //        XmlNode idNode = node.Attributes.GetNamedItem("id");
        //        string id = null;
        //        if (idNode != null)
        //            id = idNode.InnerXml;
        //        var runner = new RunnerOptions(id);
        //        runner.AddCategories(getCategories(node));
        //        runner.AddAssemblies(getAssemblies(node));
        //        _options.AddTestRun(runner);
        //    }
        //}

        //private string[] getCategories(XmlNode parent)
        //{
        //    var categories = new List<string>();
        //    var nodes = parent.SelectNodes("categories/ignore_category");
        //    foreach (XmlNode node in nodes)
        //        categories.Add(node.InnerXml);
        //    return categories.ToArray();
        //}

        //private AssemblyOptions[] getAssemblies(XmlNode parent)
        //{
        //    var assemblies = new List<AssemblyOptions>();
        //    var nodes = parent.SelectNodes("test_assembly");
        //    foreach (XmlNode node in nodes)
        //    {
        //        XmlNode nameNode = node.Attributes.GetNamedItem("name");
        //        string name = null;
        //        if (nameNode != null)
        //            name = nameNode.InnerXml;
        //        var assembly = new AssemblyOptions(name);
        //        assembly.AddTests(getTests(node));
        //        assembly.AddMembers(getMembers(node));
        //        assembly.AddNamespaces(getNamespaces(node));
        //        assemblies.Add(assembly);
        //    }
        //    return assemblies.ToArray();
        //}

        //private string[] getTests(XmlNode parent)
        //{
        //    return getStringList(parent, "tests/test");
        //}

        //private string[] getMembers(XmlNode parent)
        //{
        //    return getStringList(parent, "members/member");
        //}

        //private string[] getNamespaces(XmlNode parent)
        //{
        //    return getStringList(parent, "namespaces/namespace");
        //}

        //private string[] getStringList(XmlNode parent, string xpath)
        //{
        //    var list = new List<string>();
        //    var nodes = parent.SelectNodes(xpath);
        //    foreach (XmlNode node in nodes)
        //        list.Add(node.InnerXml);
        //    return list.ToArray();
        //}
    }
}
