using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.TestRunners;
using AutoTest.Core.TestRunners.TestRunners;

namespace AutoTest.Core.Caching.Projects
{
    public class ProjectDocument
    {
        private bool _isReadFromFile = false;
        private ProjectType _type;
        private string _assemblyname;
        private string _outputPath;
        private string _framework;
        private string _vsVersion;
        private List<string> _references = new List<string>();
        private List<string> _referencedBy = new List<string>();
		private List<string> _binaryReferences = new List<string>();
        private List<ProjectFile> _files = new List<ProjectFile>();
		private bool _requiresRebuild = false;

        public bool IsReadFromFile { get { return _isReadFromFile; } }
        public ProjectType Type { get { return _type; } }
        public string DefaultNamespace { get; private set; }
        public string AssemblyName { get { return _assemblyname; } }
        public string OutputPath { get { return _outputPath; } }
        public string Framework { get { return _framework; } }
        public string ProductVersion { get { return _vsVersion; } }
        public string[] References { get { return _references.ToArray(); } }
        public string[] ReferencedBy { get { return _referencedBy.ToArray(); } }
		public string[] BinaryReferences { get { return _binaryReferences.ToArray(); } }
        public ProjectFile[] Files { get { return _files.ToArray(); } }
		public bool RequiresRebuild { get { return _requiresRebuild; } }

        public ProjectDocument(ProjectType type)
        {
            _type = type;
        }

        public void AddReference(string reference)
        {
            _references.Add(reference);
        }

        public void AddReference(string[] references)
        {
            _references.AddRange(references);
        }

        public void RemoveReference(string reference)
        {
            _references.Remove(reference);
        }

        public void AddReferencedBy(string reference)
        {
            _referencedBy.Add(reference);
        }

        public void AddReferencedBy(string[] references)
        {
            _referencedBy.AddRange(references);
        }

        public void RemoveReferencedBy(string reference)
        {
            _referencedBy.Remove(reference);
        }
		
		public void AddBinaryReference(string reference)
		{
			_binaryReferences.Add(reference);
		}
		
		public void RemoveBinaryReference(string reference)
		{
			_binaryReferences.Remove(reference);
		}

        public void AddFile(ProjectFile file)
        {
            _files.Add(file);
        }

        public void HasBeenReadFromFile()
        {
            _isReadFromFile = true;
        }

        public bool IsReferencedBy(string reference)
        {
            return _referencedBy.Contains(reference);
        }

        public bool IsReferencing(string reference)
        {
            return _references.Contains(reference);
        }

        public void SetDefaultNamespace(string ns)
        {
            DefaultNamespace = ns;
        }

        public void SetAssemblyName(string assemblyName)
        {
            _assemblyname = assemblyName;
        }

        public void SetOutputPath(string outputPath)
        {
            _outputPath = outputPath;
        }

        public void SetFramework(string version)
        {
            _framework = version;
        }

        public void SetVSVersion(string version)
        {
            _vsVersion = version;
        }
		
		public void RebuildOnNextRun()
		{
			_requiresRebuild = true;
		}

        internal void HasBeenBuilt()
        {
            _requiresRebuild = false;
        }
    }
}
