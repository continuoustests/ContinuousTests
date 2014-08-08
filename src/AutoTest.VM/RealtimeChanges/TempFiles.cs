using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.VM.RealtimeChanges
{
    class TempFiles
    {
        public TempFile Solution { get; private set; }
        public List<TempFile> Files { get; private set; }

        public TempFiles(TempFile solution, List<TempFile> files)
        {
            Solution = solution;
            Files = files;
        }
    }

    class TempFile
    {
        public string Parent { get; private set; }
        public string Tempfile { get; private set; }
        public string Original { get; private set; }

        public TempFile(string parent, string tempFile, string original)
        {
            Parent = parent;
            Tempfile = tempFile;
            Original = original;
        }
    }
}
