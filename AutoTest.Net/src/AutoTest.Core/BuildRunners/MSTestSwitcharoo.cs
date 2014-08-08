using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutoTest.Core.BuildRunners
{
    public class MSTestSwitcharoo
    {
        private const string SIMPLE_REFERENCE =
            "<Reference Include=\"Microsoft.VisualStudio.QualityTools.UnitTestFramework,";
        private const string NORMAL_REFERENCE =
            "<Reference Include=\"Microsoft.VisualStudio.QualityTools.UnitTestFramework\"";

        private PlatformID _platform;
        private string _implementationPath;

        public MSTestSwitcharoo(PlatformID platform, string implementationPath)
        {
            _platform = platform;
            _implementationPath = implementationPath;
        }

        public bool IsGuyInCloset(string project)
        {
            if (_platform != PlatformID.Unix && _platform != PlatformID.MacOSX)
                return false;
            return project.Contains(SIMPLE_REFERENCE) ||
                   project.Contains(NORMAL_REFERENCE);
        }

        public string PerformSwitch(string projectContent)
        {
            var sb = new StringBuilder();
            var lines = projectContent.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (line.Contains(SIMPLE_REFERENCE) || line.Contains(NORMAL_REFERENCE))
                    sb.AppendLine(getOurImplementation());
                else
                    sb.AppendLine(line);
            }
            return sb.ToString();
        }

        private string getOurImplementation()
        {
            return "\t<Reference Include=\"Worst.Testing.Framework.Ever\">" + Environment.NewLine +
                   "\t\t<HintPath>" + Path.Combine(_implementationPath, "Worst.Testing.Framework.Ever.dll") + "</HintPath>" + Environment.NewLine +
                   "\t</Reference>";
        }
    }
}
