using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace AutoTest.TestRunners.MbUnit
{
    class BinPathLocator
    {
        public string Locate()
        {
            var text = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mbunit.config"));
            var start = text.IndexOf("<bin_path>");
            if (start == -1)
                return null;
            start += "<bin_path>".Length;
            var end = text.IndexOf("</bin_path>");
            if (end == -1 || start >= end)
                return null;
            return text.Substring(start, end - start);
        }
    }
}
