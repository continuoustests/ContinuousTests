using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.FileSystem;

namespace AutoTest.VM.FileSystem
{
    class IgnoreProvider : ICustomIgnoreProvider
    {
        public bool ShouldPublish(string filename)
        {
            if (filename.ToLower().EndsWith(".mm.dll"))
                return false;
            if (filename.ToLower().EndsWith(".mm.exe"))
                return false;
            return true;
        }
    }
}
