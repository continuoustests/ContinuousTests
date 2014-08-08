using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTest.Core.Caching.Projects
{
    public class Project : IRecord
    {
        public string Key { get; private set; }
        public ProjectDocument Value { get; private set; }

        public Project(string key, ProjectDocument value)
        {
            Key = key;
            Value = value;
        }

        public void Reload()
        {
            Value = new ProjectDocument(Value.Type);
        }

        public string GetAssembly(string customOutputPath)
        {
            if (Key == null)
                return "";
            if (Value == null)
                return "";
			var outputPath = Value.OutputPath;
            if (outputPath == null)
                return "";
			if (customOutputPath != null && customOutputPath.Length > 0)
				outputPath = customOutputPath;
            if (!Directory.Exists(outputPath) || !Path.IsPathRooted(outputPath))
            {
                var projectPath = Path.GetDirectoryName(Key);
                if (projectPath == null)
                    return "";
                outputPath = Path.Combine(projectPath, outputPath);
            }
            return Path.Combine(outputPath, Value.AssemblyName);
        }
    }
}
