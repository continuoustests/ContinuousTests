using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Client.Config
{
    public class VersionConfig
    {
        public string Path { get; set; }
        public string Framework { get; set; }

        public VersionConfig()
        {
            Path = "";
            Framework = "";
        }
    }
}
