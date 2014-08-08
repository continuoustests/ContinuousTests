using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutoTest.VM
{
    public class ProfilerLogFileNameParser
    {
        private string _file;
        public ProfilerLogFileNameParser(string file) {
            _file = file;
        }

        public int GetProcessID() {
            var name = Path.GetFileName(_file);
            var pid = name.Substring("mm_output_".Length, name.IndexOf("_", "mm_output_".Length) - "mm_output_".Length);
            return int.Parse(pid);
        }
    }
}
