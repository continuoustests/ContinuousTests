using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Caching
{
    public enum FileType
    {
        Compile,
        Resource,
        None
    }

    public class ProjectFile
    {
        private int i = 3;
        public string File { get; private set; }
        public FileType Type { get; private set; }
        public string Project { get; private set; }

        public ProjectFile(string file, FileType type, string project)
        {
            File = file;
            Type = type;
            Project = project;
        }
    }
}
