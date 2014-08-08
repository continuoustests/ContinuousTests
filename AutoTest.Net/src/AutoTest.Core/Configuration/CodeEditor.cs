using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Configuration
{
    public class CodeEditor
    {
        public string Executable { get; private set; }
        public string Arguments { get; private set; }

        public CodeEditor(string executable, string arguments)
        {
            Executable = executable;
            Arguments = arguments;
        }
    }
}
