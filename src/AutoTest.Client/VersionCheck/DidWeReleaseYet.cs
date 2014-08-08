using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Client.Logging;
using System.Xml;
using System.IO;
using System.Reflection;

namespace AutoTest.Client.VersionCheck
{
    public class DidWeReleaseYet
    {
        public string Version { get; private set; }
        public string ReleaseNotes { get; private set; }

        public DidWeReleaseYet(Func<string> releaseXmlFetcher)
        {
            Version = null;
            ReleaseNotes = "";
            try
            {
                using (var textReader = new StringReader(releaseXmlFetcher()))
                {
                    using (var xmlReader = XmlReader.Create(textReader))
                    {
                        while (xmlReader.Read())
                        {
                            if (!xmlReader.IsStartElement())
                                continue;

                            if (xmlReader.Name.Equals("version"))
                                Version = xmlReader.ReadString();
                            else if (xmlReader.Name.Equals("information"))
                                ReleaseNotes = xmlReader.ReadString().Replace("{0}", Environment.NewLine);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public bool Released()
        {
            if (Version == null)
                return false;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build) != Version;
        }
    }
}
