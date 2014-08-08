using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using System.IO;
using AutoTest.Core.FileSystem;

namespace AutoTest.Core.TestRunners.TestRunners
{
    public class MSTestRunConfigParser
    {
        private IConfiguration _configuration;
        private IFileSystemService _fs;

        public MSTestRunConfigParser(IConfiguration configuration, IFileSystemService fs)
        {
            _configuration = configuration;
            _fs = fs;
        }

        public string GetConfig()
        {
            var filename = getRunLocalFileName();
            if (filename == null)
                return null;
            var rootPath = Path.GetDirectoryName(_configuration.WatchPath);
            var configFile = Path.Combine(rootPath, filename);
            if (!_fs.FileExists(configFile))
                return null;
            return configFile;
        }

        private string getRunLocalFileName()
        {
            var vsmdi = getVSMDIContent();
            if (vsmdi == null)
                return null;
            var startNode = "storage=\"";
            var start = vsmdi.IndexOf(startNode);
            if (start == -1)
                return null;
            start += startNode.Length;
            var end = vsmdi.IndexOf('"', start);
            if (end == -1)
                return null;
            return vsmdi.Substring(start, end - start);
        }

        private string getVSMDIContent()
        {
            if (!_fs.FileExists(_configuration.WatchPath))
                return null;
            var file = _configuration.WatchPath.Replace(Path.GetExtension(_configuration.WatchPath), ".vsmdi");
            if (!_fs.FileExists(file))
                return null;
            return _fs.ReadFileAsText(file);
        }
    }
}
