using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AutoTest.VM.FileSystem;

namespace AutoTest.VM
{
    public class ConfigBuilder
    {
        private string _file;
        private IFSProxy _fsProxy;
        private StringBuilder _builder;

        public ConfigBuilder(string file, IFSProxy fsProxy)
        {
            _file = file;
            _fsProxy = fsProxy;
            _builder = new StringBuilder();
        }

        public bool IsConfigured()
        {
            return _fsProxy.FileExists(_file);
        }

        public void BuildConfiguration()
        {
            _builder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            _builder.AppendLine("<configuration>");
            appendBuildExecutable(_builder);
            _builder.AppendLine("\t<WhenWatchingSolutionBuildSolution>true</WhenWatchingSolutionBuildSolution>");
            _builder.AppendLine("\t<IgnoreFile>.gitignore</IgnoreFile>");
            _builder.AppendLine("\t<Debugging>false</Debugging>");
            _builder.AppendLine("</configuration>");
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        private void appendBuildExecutable(StringBuilder builder)
        {
            var buildExecutable = "";
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
                buildExecutable = "/usr/bin/xbuild";
            else
                buildExecutable = getMSBuildPath();
            if (buildExecutable == null)
                return;
            _builder.AppendLine(string.Format("\t<BuildExecutable>{0}</BuildExecutable>", buildExecutable));
        }

        private string getMSBuildPath()
        {
            string buildExecutable = null;
            var lastVersion = new Version();
            var system = _fsProxy.GetSpecialFolder(Environment.SpecialFolder.System);
            if (!_fsProxy.DirectoryExists(system))
                return buildExecutable;
            var frameworkPath = Path.Combine(Path.GetDirectoryName(system), @"Microsoft.NET\Framework");
            var frameworks = _fsProxy.GetFoldersFrom(frameworkPath, "*");
            if (frameworks != null)
            {
                foreach (var framework in frameworks)
                {
                    var version = getFrameworkVersion(framework);
                    if (version > lastVersion)
                    {
                        var file = Path.Combine(framework, "MSBuild.exe");
                        if (_fsProxy.FileExists(file))
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

        private void appendMSTestRunner(StringBuilder _builder)
        {
            if (pickMSTestFromProgramFiles())
                return;
            if (pickMSTestFromx86ProgramFiles())
                return;

            _builder.AppendLine("\t<MSTestRunner></MSTestRunner>");
        }

        private bool pickMSTestFromProgramFiles()
        {
            var programFiles = _fsProxy.GetSpecialFolder(Environment.SpecialFolder.ProgramFiles);
            if (!_fsProxy.DirectoryExists(programFiles))
                return false;
            var mstest = getMSTestExecutableFromFolder(programFiles);
            if (mstest == null)
                return false;
            _builder.AppendLine(string.Format("\t<MSTestRunner>{0}</MSTestRunner>", mstest));
            return true;
        }

        private bool pickMSTestFromx86ProgramFiles()
        {
            var programFiles = getX86ProgramFiles();
            if (!_fsProxy.DirectoryExists(programFiles))
                return false;
            var mstest = getMSTestExecutableFromFolder(programFiles);
            if (mstest == null)
                return false;
            _builder.AppendLine(string.Format("\t<MSTestRunner>{0}</MSTestRunner>", mstest));
            return true;
        }

        private string getX86ProgramFiles()
        {
            var programFiles = _fsProxy.GetSpecialFolder(Environment.SpecialFolder.ProgramFiles);
            if (programFiles == null)
                return "";
            return programFiles + " (x86)";

        }
        
        private string getMSTestExecutableFromFolder(string path)
        {
            string mstest = null;
            double lastVersion = 0;
            var folders = _fsProxy.GetFoldersFrom(path, "*visual studio*");
            if (folders != null)
            {
                foreach (var folder in folders)
                {
                    var file = Path.Combine(folder, @"Common7\IDE\MSTest.exe");
                    if (!_fsProxy.FileExists(file))
                        continue;

                    double version;
                    var versionString = getVSVersionString(folder);
                    if (double.TryParse(versionString, out version))
                    {
                        if (version > lastVersion)
                        {
                            lastVersion = version;
                            mstest = file;
                        }
                    }
                }
            }
            return mstest;
        }

        private string getVSVersionString(string folder)
        {
            var start = folder.LastIndexOf(" ");
            if (start < 2)
                return "";
            return folder.Substring(start, folder.Length - start).Replace(".", ",");
        }

        private void appendNUnitTestRunner(StringBuilder _builder)
        {
            if (pickMNUnitFromProgramFiles())
                return;
            if (pickNUnitFromx86ProgramFiles())
                return;

            _builder.AppendLine("\t<NUnitTestRunner></NUnitTestRunner>");
        }

        private bool pickMNUnitFromProgramFiles()
        {
            var programFiles = _fsProxy.GetSpecialFolder(Environment.SpecialFolder.ProgramFiles);
            if (!_fsProxy.DirectoryExists(programFiles))
                return false;
            var nunit = geNUnitExecutableFromFolder(programFiles);
            if (nunit == null)
                return false;
            _builder.AppendLine(string.Format("\t<NUnitTestRunner>{0}</NUnitTestRunner>", nunit));
            return true;
        }

        private bool pickNUnitFromx86ProgramFiles()
        {
            var programFiles = getX86ProgramFiles();
            if (!_fsProxy.DirectoryExists(programFiles))
                return false;
            var nunit = geNUnitExecutableFromFolder(programFiles);
            if (nunit == null)
                return false;
            _builder.AppendLine(string.Format("\t<NUnitTestRunner>{0}</NUnitTestRunner>", nunit));
            return true;
        }

        private string geNUnitExecutableFromFolder(string path)
        {
            string nunit = null;
            var folders = _fsProxy.GetFoldersFrom(path, "*nunit*");
            if (folders != null)
            {
                foreach (var folder in folders)
                {
                    var file = locateNUnitConsole(folder);
                    if (file == null)
                        continue;
                    nunit = file;
                }
            }
            return nunit;
        }

        private string locateNUnitConsole(string folder)
        {
            var files = _fsProxy.GetFilesFrom(folder, "nunit-console.exe");
            if (files == null || files.Length == 0)
                return null;
            Array.Sort(files);
            return files[files.Length - 1];
        }

        public bool ContainsValidNUnitRunner(string xml)
        {
            var nodeStart = "<NUnitTestRunner>";
            var nodeEnd = "</NUnitTestRunner>";
            var start = xml.IndexOf(nodeStart);
            if (start == -1)
                return false;
            start += nodeStart.Length;
            var end = xml.IndexOf(nodeEnd, start);
            if (end == -1 || start >= end)
                return false;
            var file = xml.Substring(start, end - start);
            return _fsProxy.FileExists(file);
        }

        public string GenerateLocalConfig(string path)
        {
            var nunit = locateNUnitConsole(path);
            if (nunit == null)
                return null;

            var builder = new StringBuilder();
            builder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            builder.AppendLine("<configuration>");
            builder.AppendLine(string.Format("\t<NUnitTestRunner>{0}</NUnitTestRunner>", nunit));
            builder.AppendLine("</configuration>");
            return builder.ToString();
        }
    }
}
