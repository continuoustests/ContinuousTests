using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.FileSystem
{
    class NullIgnoreProvider : ICustomIgnoreProvider
    {
        public bool ShouldPublish(string filename)
        {
            return true;
        }
    }
}
