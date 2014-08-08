using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Client.Config
{
    public class IgnoreFile
    {
        public string File { get; set; }
        public string ContentPath { get; set; }
        public string Content { get; set; }
        public bool WriteContent { get; set; }

        public IgnoreFile()
        {
            File = "";
            ContentPath = "";
            Content = "";
            WriteContent = false;
        }
    }
}
