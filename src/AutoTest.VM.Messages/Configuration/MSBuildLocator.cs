using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTest.VM.Messages.Configuration
{
    public class MSBuildLocator : AutoTest.VM.Messages.Configuration.IMSBuildLocator
    {
        public string GetBuildExecutable()
        {
            var buildExecutable = "";
            var lastVersion = new Version();
            var system = Environment.GetFolderPath(Environment.SpecialFolder.System);
            if (!Directory.Exists(system))
                return null;
            var frameworkPath = Path.Combine(Path.GetDirectoryName(system), @"Microsoft.NET\Framework");
            var frameworks = Directory.GetDirectories(frameworkPath, "*", SearchOption.TopDirectoryOnly);
            if (frameworks != null)
            {
                foreach (var framework in frameworks)
                {
                    var version = getFrameworkVersion(framework);
                    if (version > lastVersion)
                    {
                        var file = Path.Combine(framework, "MSBuild.exe");
                        if (File.Exists(file))
                        {
                            lastVersion = version;
                            buildExecutable = file;
                        }
                    }
                }
            }
            return buildExecutable;
        }

        private Version getFrameworkVersion(string framework)
        {
            var directory = Path.GetFileName(framework);
            Version version;
            try
            {
                return new Version(directory.Substring(1, directory.Length - 1));
            }
            catch
            {
                version = new Version();
            }
            return version;
        }
    }
}
