using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTest.Messages.FileStorage
{
    public class ConfigurationLocator
    {
        public string GetConfiguration(string watchToken)
        {
            if (watchToken == null)
                return null;
            var localFile = Path.Combine(getWatchPath(watchToken), "AutoTest.config");
            var file = new PathTranslator(watchToken).Translate(localFile);
            if (file == null)
                file = "";
            if (File.Exists(file))
                return file;
            if (File.Exists(localFile))
                return localFile;
            return file;
        }

        private string getWatchPath(string watchToken)
        {
            if (File.Exists(watchToken))
                return Path.GetDirectoryName(watchToken);
            return watchToken;
        }
    }
}
