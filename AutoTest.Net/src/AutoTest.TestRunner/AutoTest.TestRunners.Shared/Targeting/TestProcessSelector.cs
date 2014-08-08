using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using System.Reflection;
using System.IO;

namespace AutoTest.TestRunners.Shared.Targeting
{
    public class TestProcessSelector
    {
        public string Get(string testAssembly)
        {
            using (var locator = Reflect.On(testAssembly))
            {
                return Get(locator.GetPlatform(), locator.GetTargetFramework());
            }
        }

        public string Get(Platform platform, Version version)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var platformString = getPlatformString(platform);
            var frameworkString = getFrameworkString(version);
            return Path.Combine(path, string.Format("AutoTest.TestRunner{0}{1}.exe", platformString, frameworkString));
        }

        private string getPlatformString(Platform platform)
        {
            if (platform == Platform.x86)
                return ".x86";
            return "";
        }

        private string getFrameworkString(Version version)
        {
            if (version.Major > 3)
                return string.Format(".v{0}.{1}", version.Major, version.Minor);
            return "";
        }
    }
}
