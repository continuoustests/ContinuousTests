using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace AutoTest.Core.Caching.Projects
{
    public class ProjectReferenceParser
    {
        public IEnumerable<string> GetAllBinaryReferences(string projectFile)
        {
            DebugLog.Debug.WriteDebug("Getting binary references for " + projectFile);
            var references = new List<string>();
            scanDocumentFor(projectFile,
                (XmlReader reader, XmlPath path) => {
                    if (path.Path.Equals("/Project/ItemGroup/Reference"))
                        references.Add(reader.GetAttribute("Include"));
                });
            return references;
        }

        public IEnumerable<string> GetAllProjectReferences(string file)
        {
            DebugLog.Debug.WriteDebug("Getting project references for " + file);
            var references = new List<string>();
            scanDocumentFor(file,
                (XmlReader reader, XmlPath path) =>
                {
                    if (path.Path.Equals("/Project/ItemGroup/ProjectReference"))
                        references.Add(reader.GetAttribute("Include"));
                });
            return references;
        }

        private void scanDocumentFor(string projectFile, Action<XmlReader, XmlPath> handler)
        {
            if (!File.Exists(projectFile))
                return;
            using (var reader = XmlReader.Create(projectFile))
            {
                var path = new XmlPath();
                while (reader.Read())
                {
                    if (popEndElement(reader, path))
                        continue;
                    if (reader.Name.Length == 0)
                        continue;
                    path.Push(reader.Name);

                    handler.Invoke(reader, path);

                    if (reader.IsEmptyElement)
                        path.Pop();
                }
            }
        }

        private bool popEndElement(XmlReader reader, XmlPath path)
        {
            if (!reader.IsStartElement() && !reader.HasValue)
            {
                path.Pop();
                return true;
            }
            return false;
        }
    }

    class XmlPath
    {
        private List<string> _nodes = new List<string>();

        public string Path
        {
            get
            {
                var fullPath = "";
                foreach (var node in _nodes)
                    fullPath += "/" + node;
                return fullPath;
            }
        }

        public void Push(string node)
        {
            _nodes.Add(node);
        }

        public void Pop()
        {
            try
            {
                _nodes.RemoveAt(_nodes.Count - 1);
            }
            catch (Exception ex)
            {
                DebugLog.Debug.WriteException(ex);
            }
        }
    }
}
