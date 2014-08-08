using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using System.IO;
using AutoTest.Core.DebugLog;

namespace AutoTest.VM
{
    class DefaultConfigFileLocator : ILocateWriteLocation
    {
        private string _logFile;
        private bool _isGlobal;

        public DefaultConfigFileLocator(string logprefix, bool isGlobal)
        {
            _logFile = Path.Combine(getPath(), logprefix + "_debug.log");
            _isGlobal = isGlobal;
        }

        public string GetLogfile()
        {
            return _logFile;
        }

        public string GetConfigurationFile()
        {   
            return Path.Combine(getPath(), "AutoTest.config");
        }

        private string getPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (_isGlobal)
                appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var mm = Path.Combine(appData, "MightyMoose");
            if (!Directory.Exists(mm))
                Directory.CreateDirectory(mm);
            return mm;
        }
    }
}
