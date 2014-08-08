using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.FileSystem
{
    public interface ICustomIgnoreProvider
    {
        bool ShouldPublish(string filename);
    }
}
