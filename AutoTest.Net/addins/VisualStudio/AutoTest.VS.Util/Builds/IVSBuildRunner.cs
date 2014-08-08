using System;
using System.Collections.Generic;
namespace AutoTest.VS.Util.Builds
{
    public interface IVSBuildRunner
    {
        bool Build();
        bool Build(IEnumerable<string> projects);
    }
}
