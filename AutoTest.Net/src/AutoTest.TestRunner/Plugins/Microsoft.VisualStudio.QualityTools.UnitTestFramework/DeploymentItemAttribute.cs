using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public sealed class DeploymentItemAttribute : Attribute
    {
        public string Path { get; private set; }
        public string OutputDirectory { get; private set; }

        public DeploymentItemAttribute(string path)
        {
            Path = path;
            OutputDirectory = Environment.CurrentDirectory;
        }

        public DeploymentItemAttribute(string path, string outputDirectory)
        {
            Path = path;
            OutputDirectory = outputDirectory;
        }
    }
}
