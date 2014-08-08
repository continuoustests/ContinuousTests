using System;
namespace AutoTest.Core.FileSystem
{
    public interface IWatchPathLocator
    {
        string Locate(string path);
    }
}
