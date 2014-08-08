using System;
using System.IO;
using AutoTest.Core.FileSystem;
using AutoTest.Core.DebugLog;
using AutoTest.Core.Messaging;
using System.Reflection;
namespace AutoTest.Core.Configuration
{
	public class DefaultConfigurationLocator : ILocateWriteLocation
	{
        public string GetLogfile()
        {
            return Path.Combine(getPath(), "debug.log");
        }

		public string GetConfigurationFile()
		{
			return Path.Combine(getPath(), "AutoTest.config");
		}
		
		private string getPath()
		{
			return PathParsing.GetRootDirectory();
		}
	}

    public class LocalAppDataConfigurationLocator : ILocateWriteLocation
    {
        private string _templateConfig;

        public LocalAppDataConfigurationLocator(string templateConfig)
        {
            _templateConfig = templateConfig;
        }

        public string GetLogfile()
        {
            return Path.Combine(getPath(), "debug.log");
        }

        public string GetConfigurationFile()
        {
            var config = Path.Combine(getPath(), "AutoTest.config");
            if (!File.Exists(config))
            {
                if (!Directory.Exists(Path.GetDirectoryName(config)))
                    Directory.CreateDirectory(Path.GetDirectoryName(config));

                var template = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _templateConfig);
                if (File.Exists(template))
                    File.Copy(template, config);
            }
            return config;
        }

        private string getPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var atDir = Path.Combine(appData, "AutoTest.Net");
            if (!Directory.Exists(atDir))
                Directory.CreateDirectory(atDir);
            return atDir;
        }
    }
}

