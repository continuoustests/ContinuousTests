using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using AutoTest.Core.Configuration;

namespace AutoTest.Core.FileSystem
{
    public class WatchValidator : IWatchValidator
    {
		private IConfiguration _configuration;
        private ICustomIgnoreProvider[] _ignoreProviders;
		private string[] _defaultIgnores = new string[17];

        public WatchValidator(IConfiguration configuration, ICustomIgnoreProvider[] ignoreProviders)
		{
			_configuration = configuration;
            _ignoreProviders = ignoreProviders;
			_defaultIgnores[0] = "bin/Debug";
			_defaultIgnores[1] = "bin/Release";
			_defaultIgnores[2] = "bin/AutoTest.Net";
            _defaultIgnores[3] = "bin/AutoTest.NET";
			_defaultIgnores[4] = "bin/x86";
			_defaultIgnores[5] = "obj/Debug";
			_defaultIgnores[6] = "obj/Release";
			_defaultIgnores[7] = "obj/x86";
			_defaultIgnores[8] = "*.FileListAbsolute.txt";
			_defaultIgnores[9] = "*.FilesWrittenAbsolute.txt";
			_defaultIgnores[10] = "*.suo";
            _defaultIgnores[11] = "*.UnmanagedRegistration.cache";
			_defaultIgnores[12] = "*.swp";
			_defaultIgnores[13] = "*.swx";
			_defaultIgnores[14] = "*~";
            _defaultIgnores[15] = "*_rltm_build_fl_*";
            _defaultIgnores[16] = "*_mm_cache.bin";
		}
		
        public bool ShouldPublish(string filePath)
        {
			if (_configuration.ShouldUseBinaryChangeIgnoreLists)
                return useBinaryChangeList(filePath);
            return useDefaultIgnoreList(filePath);
        }

        private bool useBinaryChangeList(string filePath)
        {
            if (!filePath.ToLower().EndsWith(".dll") && !filePath.ToLower().EndsWith(".exe"))
                return false;
            if (filePath.ToLower().EndsWith(".vshost.exe"))
                return false;
            foreach (var provider in _ignoreProviders)
                if (!provider.ShouldPublish(filePath))
                    return false;
            return true;
        }

        private bool useDefaultIgnoreList(string filePath)
        {
            filePath = filePath.Replace("\\", "/");
            if (match(filePath, _defaultIgnores))
                return false;
            if (matchCustomOutputPath(filePath))
                return false;
            if (_configuration.CustomOutputPath != null && _configuration.CustomOutputPath.Length > 0)
            {
                var pattern = _configuration.CustomOutputPath.Replace('\\', '/');
                if (match(filePath, new string[] { pattern }))
                    return false;
            }
            if (match(filePath, _configuration.WatchIgnoreList))
                return false;
            foreach (var provider in _ignoreProviders)
                if (!provider.ShouldPublish(filePath))
                    return false;
            return true;
        }
		
		public string GetIgnorePatterns()
		{
			var list = "";
			foreach (var pattern in _configuration.WatchIgnoreList)
				list += (list.Length == 0 ? "" : "|") + pattern;
			if (_configuration.CustomOutputPath != null && _configuration.CustomOutputPath.Length > 0)
				list += (list.Length == 0 ? "" : "|") + _configuration.CustomOutputPath.Replace('\\', '/');
			return list;
		}

        private bool matchCustomOutputPath(string filePath)
        {
            if (_configuration.CustomOutputPath != null && _configuration.CustomOutputPath.Length > 0)
            {
                var pattern = _configuration.CustomOutputPath.Replace('\\', '/');
                if (match(filePath, new string[] { pattern }))
                    return true;
            }
            return false;
        }

		
		private bool match(string stringToMatch, string[] patterns)
		{
            var match = getFromEnvironment(stringToMatch);
			foreach (var patter in patterns)
			{
                var environmentPattern = getFromEnvironment(patter);
                if (environmentPattern.EndsWith(Path.DirectorySeparatorChar.ToString()))
				{
                    if (match.Contains(environmentPattern))
						return true;
				}
                if (match.EndsWith(environmentPattern))
					return true;
                if (match.Contains(string.Format("{0}{1}{0}", Path.DirectorySeparatorChar, environmentPattern)))
					return true;
                if (matchStringToGlobUsingGlobishMatching(match, environmentPattern))
					return true;
			}
			return false;
		}

        private string getFromEnvironment(string pattern)
        {
            if (OS.IsUnix)
                return pattern;
            return pattern.ToLower();
        }

        private bool contains(string path, string stringToSearchFor)
        {
            return path.IndexOf(stringToSearchFor) >= 0;
        }
			
	    private bool matchStringToGlobUsingGlobishMatching(string stringToMatch, string pattern)
	    {
	        string regExPattern = Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".");
	        var regex = new Regex(regExPattern, RegexOptions.Singleline);
			return regex.IsMatch(stringToMatch);
		}
    }
}
