using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.BuildRunners
{
    public interface IAssemblyChangedProvider
    {
        bool HasChanged(string oldFile, string newFile);
    }
}
